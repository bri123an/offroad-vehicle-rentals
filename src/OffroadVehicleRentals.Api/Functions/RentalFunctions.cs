using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using OffroadVehicleRentals.Api.Data;
using OffroadVehicleRentals.Shared.Models;
using System.Net;
using System.Text.Json;

namespace OffroadVehicleRentals.Api.Functions;

public class RentalFunctions
{
    private readonly VehicleRentalContext _context;

    public RentalFunctions(VehicleRentalContext context)
    {
        _context = context;
    }

    [Function("GetRentals")]
    public async Task<HttpResponseData> GetRentals(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "rentals")] HttpRequestData req)
    {
        var rentals = await _context.Rentals
            .Include(r => r.Vehicle)
            .Include(r => r.ChecklistItems)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(rentals);
        return response;
    }

    [Function("GetRental")]
    public async Task<HttpResponseData> GetRental(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "rentals/{id:int}")] HttpRequestData req,
        int id)
    {
        var rental = await _context.Rentals
            .Include(r => r.Vehicle)
            .Include(r => r.ChecklistItems)
            .FirstOrDefaultAsync(r => r.Id == id);

        var response = rental == null
            ? req.CreateResponse(HttpStatusCode.NotFound)
            : req.CreateResponse(HttpStatusCode.OK);

        if (rental != null)
        {
            await response.WriteAsJsonAsync(rental);
        }

        return response;
    }

    [Function("GetUpcomingRentals")]
    public async Task<HttpResponseData> GetUpcomingRentals(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "rentals/upcoming")] HttpRequestData req)
    {
        var now = DateTime.UtcNow;
        var rentals = await _context.Rentals
            .Include(r => r.Vehicle)
            .Where(r => r.StartDate >= now)
            .OrderBy(r => r.StartDate)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(rentals);
        return response;
    }

    [Function("GetActiveRentals")]
    public async Task<HttpResponseData> GetActiveRentals(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "rentals/active")] HttpRequestData req)
    {
        var now = DateTime.UtcNow;
        var rentals = await _context.Rentals
            .Include(r => r.Vehicle)
            .Where(r => r.StartDate <= now && r.EndDate >= now)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(rentals);
        return response;
    }

    [Function("CreateRental")]
    public async Task<HttpResponseData> CreateRental(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "rentals")] HttpRequestData req)
    {
        var rental = await JsonSerializer.DeserializeAsync<Rental>(req.Body);

        if (rental == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid rental data");
            return badRequest;
        }

        // Check vehicle availability
        var vehicle = await _context.Vehicles.FindAsync(rental.VehicleId);
        if (vehicle == null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("Vehicle not found");
            return notFound;
        }

        // Check for conflicting rentals
        var hasConflict = await _context.Rentals
            .AnyAsync(r => r.VehicleId == rental.VehicleId &&
                          r.Id != rental.Id &&
                          ((r.StartDate <= rental.EndDate && r.EndDate >= rental.StartDate)));

        if (hasConflict)
        {
            var conflict = req.CreateResponse(HttpStatusCode.Conflict);
            await conflict.WriteStringAsync("Vehicle is already reserved for this time period");
            return conflict;
        }

        rental.CreatedDate = DateTime.UtcNow;

        // Update vehicle status to Reserved
        vehicle.Status = VehicleStatus.Reserved;

        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(rental);
        return response;
    }

    [Function("UpdateRental")]
    public async Task<HttpResponseData> UpdateRental(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "rentals/{id:int}")] HttpRequestData req,
        int id)
    {
        var existingRental = await _context.Rentals.FindAsync(id);
        if (existingRental == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var rental = await JsonSerializer.DeserializeAsync<Rental>(req.Body);
        if (rental == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid rental data");
            return badRequest;
        }

        // Check for conflicting rentals
        var hasConflict = await _context.Rentals
            .AnyAsync(r => r.VehicleId == rental.VehicleId &&
                          r.Id != id &&
                          ((r.StartDate <= rental.EndDate && r.EndDate >= rental.StartDate)));

        if (hasConflict)
        {
            var conflict = req.CreateResponse(HttpStatusCode.Conflict);
            await conflict.WriteStringAsync("Vehicle is already reserved for this time period");
            return conflict;
        }

        existingRental.VehicleId = rental.VehicleId;
        existingRental.CustomerName = rental.CustomerName;
        existingRental.CustomerPhone = rental.CustomerPhone;
        existingRental.CustomerEmail = rental.CustomerEmail;
        existingRental.StartDate = rental.StartDate;
        existingRental.EndDate = rental.EndDate;
        existingRental.StartHours = rental.StartHours;
        existingRental.EndHours = rental.EndHours;
        existingRental.StartMileage = rental.StartMileage;
        existingRental.EndMileage = rental.EndMileage;
        existingRental.PreRentalChecklistCompleted = rental.PreRentalChecklistCompleted;
        existingRental.PostRentalChecklistCompleted = rental.PostRentalChecklistCompleted;
        existingRental.Notes = rental.Notes;
        existingRental.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(existingRental);
        return response;
    }

    [Function("DeleteRental")]
    public async Task<HttpResponseData> DeleteRental(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "rentals/{id:int}")] HttpRequestData req,
        int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        _context.Rentals.Remove(rental);
        await _context.SaveChangesAsync();

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}
