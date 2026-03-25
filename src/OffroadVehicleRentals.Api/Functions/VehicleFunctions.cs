using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using OffroadVehicleRentals.Api.Data;
using OffroadVehicleRentals.Shared.Models;
using System.Net;
using System.Text.Json;

namespace OffroadVehicleRentals.Api.Functions;

public class VehicleFunctions
{
    private readonly VehicleRentalContext _context;

    public VehicleFunctions(VehicleRentalContext context)
    {
        _context = context;
    }

    [Function("GetVehicles")]
    public async Task<HttpResponseData> GetVehicles(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vehicles")] HttpRequestData req)
    {
        var vehicles = await _context.Vehicles
            .Include(v => v.MaintenanceRecords)
            .Include(v => v.RepairRecords)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(vehicles);
        return response;
    }

    [Function("GetVehicle")]
    public async Task<HttpResponseData> GetVehicle(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vehicles/{id}")] HttpRequestData req,
        int id)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.MaintenanceRecords)
            .Include(v => v.RepairRecords)
            .Include(v => v.Rentals)
            .FirstOrDefaultAsync(v => v.Id == id);

        var response = vehicle == null
            ? req.CreateResponse(HttpStatusCode.NotFound)
            : req.CreateResponse(HttpStatusCode.OK);

        if (vehicle != null)
        {
            await response.WriteAsJsonAsync(vehicle);
        }

        return response;
    }

    [Function("CreateVehicle")]
    public async Task<HttpResponseData> CreateVehicle(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "vehicles")] HttpRequestData req)
    {
        var vehicle = await JsonSerializer.DeserializeAsync<Vehicle>(req.Body);

        if (vehicle == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid vehicle data");
            return badRequest;
        }

        vehicle.CreatedDate = DateTime.UtcNow;
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(vehicle);
        return response;
    }

    [Function("UpdateVehicle")]
    public async Task<HttpResponseData> UpdateVehicle(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "vehicles/{id}")] HttpRequestData req,
        int id)
    {
        var existingVehicle = await _context.Vehicles.FindAsync(id);
        if (existingVehicle == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var vehicle = await JsonSerializer.DeserializeAsync<Vehicle>(req.Body);
        if (vehicle == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid vehicle data");
            return badRequest;
        }

        existingVehicle.VehicleNumber = vehicle.VehicleNumber;
        existingVehicle.Make = vehicle.Make;
        existingVehicle.Model = vehicle.Model;
        existingVehicle.Year = vehicle.Year;
        existingVehicle.Type = vehicle.Type;
        existingVehicle.Status = vehicle.Status;
        existingVehicle.CurrentHours = vehicle.CurrentHours;
        existingVehicle.CurrentMileage = vehicle.CurrentMileage;
        existingVehicle.LastMaintenanceDate = vehicle.LastMaintenanceDate;
        existingVehicle.NextMaintenanceHours = vehicle.NextMaintenanceHours;
        existingVehicle.NextMaintenanceMileage = vehicle.NextMaintenanceMileage;
        existingVehicle.Notes = vehicle.Notes;
        existingVehicle.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(existingVehicle);
        return response;
    }

    [Function("DeleteVehicle")]
    public async Task<HttpResponseData> DeleteVehicle(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "vehicles/{id}")] HttpRequestData req,
        int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        return req.CreateResponse(HttpStatusCode.NoContent);
    }

    [Function("GetAvailableVehicles")]
    public async Task<HttpResponseData> GetAvailableVehicles(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vehicles/available")] HttpRequestData req)
    {
        var vehicles = await _context.Vehicles
            .Where(v => v.Status == VehicleStatus.Available)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(vehicles);
        return response;
    }
}
