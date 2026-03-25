using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using OffroadVehicleRentals.Api.Data;
using OffroadVehicleRentals.Shared.Models;
using System.Net;
using System.Text.Json;

namespace OffroadVehicleRentals.Api.Functions;

public class MaintenanceFunctions
{
    private readonly VehicleRentalContext _context;

    public MaintenanceFunctions(VehicleRentalContext context)
    {
        _context = context;
    }

    [Function("GetMaintenanceRecords")]
    public async Task<HttpResponseData> GetMaintenanceRecords(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "maintenance")] HttpRequestData req)
    {
        var records = await _context.MaintenanceRecords
            .Include(m => m.Vehicle)
            .OrderByDescending(m => m.MaintenanceDate)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(records);
        return response;
    }

    [Function("GetMaintenanceRecord")]
    public async Task<HttpResponseData> GetMaintenanceRecord(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "maintenance/{id}")] HttpRequestData req,
        int id)
    {
        var record = await _context.MaintenanceRecords
            .Include(m => m.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id);

        var response = record == null
            ? req.CreateResponse(HttpStatusCode.NotFound)
            : req.CreateResponse(HttpStatusCode.OK);

        if (record != null)
        {
            await response.WriteAsJsonAsync(record);
        }

        return response;
    }

    [Function("GetVehicleMaintenanceRecords")]
    public async Task<HttpResponseData> GetVehicleMaintenanceRecords(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vehicles/{vehicleId}/maintenance")] HttpRequestData req,
        int vehicleId)
    {
        var records = await _context.MaintenanceRecords
            .Where(m => m.VehicleId == vehicleId)
            .OrderByDescending(m => m.MaintenanceDate)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(records);
        return response;
    }

    [Function("CreateMaintenanceRecord")]
    public async Task<HttpResponseData> CreateMaintenanceRecord(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "maintenance")] HttpRequestData req)
    {
        var record = await JsonSerializer.DeserializeAsync<MaintenanceRecord>(req.Body);

        if (record == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid maintenance record data");
            return badRequest;
        }

        var vehicle = await _context.Vehicles.FindAsync(record.VehicleId);
        if (vehicle == null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("Vehicle not found");
            return notFound;
        }

        record.CreatedDate = DateTime.UtcNow;

        // Update vehicle's last maintenance date
        vehicle.LastMaintenanceDate = record.MaintenanceDate;
        vehicle.UpdatedDate = DateTime.UtcNow;

        _context.MaintenanceRecords.Add(record);
        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(record);
        return response;
    }

    [Function("UpdateMaintenanceRecord")]
    public async Task<HttpResponseData> UpdateMaintenanceRecord(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "maintenance/{id}")] HttpRequestData req,
        int id)
    {
        var existingRecord = await _context.MaintenanceRecords.FindAsync(id);
        if (existingRecord == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var record = await JsonSerializer.DeserializeAsync<MaintenanceRecord>(req.Body);
        if (record == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid maintenance record data");
            return badRequest;
        }

        existingRecord.MaintenanceDate = record.MaintenanceDate;
        existingRecord.MaintenanceType = record.MaintenanceType;
        existingRecord.Description = record.Description;
        existingRecord.HoursAtMaintenance = record.HoursAtMaintenance;
        existingRecord.MileageAtMaintenance = record.MileageAtMaintenance;
        existingRecord.Cost = record.Cost;
        existingRecord.PerformedBy = record.PerformedBy;
        existingRecord.Notes = record.Notes;
        existingRecord.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(existingRecord);
        return response;
    }

    [Function("DeleteMaintenanceRecord")]
    public async Task<HttpResponseData> DeleteMaintenanceRecord(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "maintenance/{id}")] HttpRequestData req,
        int id)
    {
        var record = await _context.MaintenanceRecords.FindAsync(id);
        if (record == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        _context.MaintenanceRecords.Remove(record);
        await _context.SaveChangesAsync();

        return req.CreateResponse(HttpStatusCode.NoContent);
    }

    [Function("GetRepairRecords")]
    public async Task<HttpResponseData> GetRepairRecords(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "repairs")] HttpRequestData req)
    {
        var records = await _context.RepairRecords
            .Include(r => r.Vehicle)
            .OrderByDescending(r => r.RepairDate)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(records);
        return response;
    }

    [Function("GetRepairRecord")]
    public async Task<HttpResponseData> GetRepairRecord(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "repairs/{id}")] HttpRequestData req,
        int id)
    {
        var record = await _context.RepairRecords
            .Include(r => r.Vehicle)
            .FirstOrDefaultAsync(r => r.Id == id);

        var response = record == null
            ? req.CreateResponse(HttpStatusCode.NotFound)
            : req.CreateResponse(HttpStatusCode.OK);

        if (record != null)
        {
            await response.WriteAsJsonAsync(record);
        }

        return response;
    }

    [Function("GetVehicleRepairRecords")]
    public async Task<HttpResponseData> GetVehicleRepairRecords(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vehicles/{vehicleId}/repairs")] HttpRequestData req,
        int vehicleId)
    {
        var records = await _context.RepairRecords
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.RepairDate)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(records);
        return response;
    }

    [Function("CreateRepairRecord")]
    public async Task<HttpResponseData> CreateRepairRecord(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "repairs")] HttpRequestData req)
    {
        var record = await JsonSerializer.DeserializeAsync<RepairRecord>(req.Body);

        if (record == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid repair record data");
            return badRequest;
        }

        var vehicle = await _context.Vehicles.FindAsync(record.VehicleId);
        if (vehicle == null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("Vehicle not found");
            return notFound;
        }

        record.CreatedDate = DateTime.UtcNow;

        // Update vehicle status to InRepair
        vehicle.Status = VehicleStatus.InRepair;
        vehicle.UpdatedDate = DateTime.UtcNow;

        _context.RepairRecords.Add(record);
        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(record);
        return response;
    }

    [Function("UpdateRepairRecord")]
    public async Task<HttpResponseData> UpdateRepairRecord(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "repairs/{id}")] HttpRequestData req,
        int id)
    {
        var existingRecord = await _context.RepairRecords.FindAsync(id);
        if (existingRecord == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var record = await JsonSerializer.DeserializeAsync<RepairRecord>(req.Body);
        if (record == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid repair record data");
            return badRequest;
        }

        existingRecord.RepairDate = record.RepairDate;
        existingRecord.IssueDescription = record.IssueDescription;
        existingRecord.RepairDescription = record.RepairDescription;
        existingRecord.Cost = record.Cost;
        existingRecord.PerformedBy = record.PerformedBy;
        existingRecord.IsWarrantyClaim = record.IsWarrantyClaim;
        existingRecord.Notes = record.Notes;
        existingRecord.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(existingRecord);
        return response;
    }

    [Function("DeleteRepairRecord")]
    public async Task<HttpResponseData> DeleteRepairRecord(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "repairs/{id}")] HttpRequestData req,
        int id)
    {
        var record = await _context.RepairRecords.FindAsync(id);
        if (record == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        _context.RepairRecords.Remove(record);
        await _context.SaveChangesAsync();

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}
