using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using OffroadVehicleRentals.Api.Data;
using OffroadVehicleRentals.Shared.Models;
using System.Net;
using System.Text.Json;

namespace OffroadVehicleRentals.Api.Functions;

public class ChecklistFunctions
{
    private readonly VehicleRentalContext _context;

    public ChecklistFunctions(VehicleRentalContext context)
    {
        _context = context;
    }

    // Checklist Templates
    [Function("GetChecklistTemplates")]
    public async Task<HttpResponseData> GetChecklistTemplates(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "checklist-templates")] HttpRequestData req)
    {
        try
        {
            var templates = await _context.ChecklistTemplates
                .Include(t => t.TemplateItems)
                .Where(t => t.IsActive)
                .ToListAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
            var json = JsonSerializer.Serialize(templates, jsonOptions);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(json);
            return response;
        }
        catch (Exception ex)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }

    [Function("GetChecklistTemplate")]
    public async Task<HttpResponseData> GetChecklistTemplate(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "checklist-templates/{id}")] HttpRequestData req,
        int id)
    {
        var template = await _context.ChecklistTemplates
            .Include(t => t.TemplateItems)
            .FirstOrDefaultAsync(t => t.Id == id);

        var response = template == null
            ? req.CreateResponse(HttpStatusCode.NotFound)
            : req.CreateResponse(HttpStatusCode.OK);

        if (template != null)
        {
            await response.WriteAsJsonAsync(template);
        }

        return response;
    }

    [Function("CreateChecklistTemplate")]
    public async Task<HttpResponseData> CreateChecklistTemplate(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "checklist-templates")] HttpRequestData req)
    {
        var template = await JsonSerializer.DeserializeAsync<ChecklistTemplate>(req.Body);

        if (template == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid template data");
            return badRequest;
        }

        template.CreatedDate = DateTime.UtcNow;
        _context.ChecklistTemplates.Add(template);
        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(template);
        return response;
    }

    // Checklist Items
    [Function("GetChecklistItems")]
    public async Task<HttpResponseData> GetChecklistItems(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "checklist-items")] HttpRequestData req)
    {
        var items = await _context.ChecklistItems
            .Include(i => i.Vehicle)
            .Include(i => i.Rental)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(items);
        return response;
    }

    [Function("GetRentalChecklistItems")]
    public async Task<HttpResponseData> GetRentalChecklistItems(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "rentals/{rentalId}/checklist-items")] HttpRequestData req,
        int rentalId)
    {
        var items = await _context.ChecklistItems
            .Where(i => i.RentalId == rentalId)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(items);
        return response;
    }

    [Function("GetVehicleChecklistItems")]
    public async Task<HttpResponseData> GetVehicleChecklistItems(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vehicles/{vehicleId}/checklist-items")] HttpRequestData req,
        int vehicleId)
    {
        var items = await _context.ChecklistItems
            .Where(i => i.VehicleId == vehicleId)
            .OrderByDescending(i => i.CreatedDate)
            .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(items);
        return response;
    }

    [Function("CreateChecklistItem")]
    public async Task<HttpResponseData> CreateChecklistItem(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "checklist-items")] HttpRequestData req)
    {
        var item = await JsonSerializer.DeserializeAsync<ChecklistItem>(req.Body);

        if (item == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid checklist item data");
            return badRequest;
        }

        item.CreatedDate = DateTime.UtcNow;
        _context.ChecklistItems.Add(item);
        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(item);
        return response;
    }

    [Function("CreateChecklistItemsFromTemplate")]
    public async Task<HttpResponseData> CreateChecklistItemsFromTemplate(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "checklist-items/from-template")] HttpRequestData req)
    {
        var requestBody = await JsonSerializer.DeserializeAsync<CreateFromTemplateRequest>(req.Body);

        if (requestBody == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid request data");
            return badRequest;
        }

        var template = await _context.ChecklistTemplates
            .Include(t => t.TemplateItems)
            .FirstOrDefaultAsync(t => t.Id == requestBody.TemplateId);

        if (template == null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("Template not found");
            return notFound;
        }

        var items = new List<ChecklistItem>();
        foreach (var templateItem in template.TemplateItems.OrderBy(i => i.SortOrder))
        {
            var item = new ChecklistItem
            {
                RentalId = requestBody.RentalId,
                VehicleId = requestBody.VehicleId,
                Type = template.Type,
                ItemText = templateItem.ItemText,
                IsCompleted = false,
                CreatedDate = DateTime.UtcNow
            };
            items.Add(item);
            _context.ChecklistItems.Add(item);
        }

        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(items);
        return response;
    }

    [Function("UpdateChecklistItem")]
    public async Task<HttpResponseData> UpdateChecklistItem(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "checklist-items/{id}")] HttpRequestData req,
        int id)
    {
        var existingItem = await _context.ChecklistItems.FindAsync(id);
        if (existingItem == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var item = await JsonSerializer.DeserializeAsync<ChecklistItem>(req.Body);
        if (item == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid checklist item data");
            return badRequest;
        }

        existingItem.ItemText = item.ItemText;
        existingItem.IsCompleted = item.IsCompleted;
        existingItem.CompletedDate = item.IsCompleted ? (item.CompletedDate ?? DateTime.UtcNow) : null;
        existingItem.CompletedBy = item.CompletedBy;
        existingItem.Notes = item.Notes;

        await _context.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(existingItem);
        return response;
    }

    [Function("DeleteChecklistItem")]
    public async Task<HttpResponseData> DeleteChecklistItem(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "checklist-items/{id}")] HttpRequestData req,
        int id)
    {
        var item = await _context.ChecklistItems.FindAsync(id);
        if (item == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        _context.ChecklistItems.Remove(item);
        await _context.SaveChangesAsync();

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}

public class CreateFromTemplateRequest
{
    public int TemplateId { get; set; }
    public int? RentalId { get; set; }
    public int? VehicleId { get; set; }
}
