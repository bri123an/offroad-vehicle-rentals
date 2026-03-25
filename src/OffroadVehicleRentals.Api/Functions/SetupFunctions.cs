using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using OffroadVehicleRentals.Api.Data;
using OffroadVehicleRentals.Shared.Models;
using System.Net;

namespace OffroadVehicleRentals.Api.Functions;

public class SetupFunctions
{
    private readonly VehicleRentalContext _context;

    public SetupFunctions(VehicleRentalContext context)
    {
        _context = context;
    }

    [Function("EnsureDatabase")]
    public async Task<HttpResponseData> EnsureDatabase(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "setup/ensure-database")] HttpRequestData req)
    {
        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Add sample data if empty
            if (!await _context.Vehicles.AnyAsync())
            {
                var vehicles = new List<Vehicle>
                {
                    new Vehicle
                    {
                        VehicleNumber = "ATV-001",
                        Make = "Polaris",
                        Model = "Sportsman 570",
                        Year = 2023,
                        Type = VehicleType.FourWheeler,
                        Status = VehicleStatus.Available,
                        CurrentHours = 45.5m,
                        CurrentMileage = 0,
                        NextMaintenanceHours = 100.0m,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Vehicle
                    {
                        VehicleNumber = "ATV-002",
                        Make = "Honda",
                        Model = "Rancher 420",
                        Year = 2022,
                        Type = VehicleType.FourWheeler,
                        Status = VehicleStatus.Available,
                        CurrentHours = 120.0m,
                        CurrentMileage = 0,
                        NextMaintenanceHours = 150.0m,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Vehicle
                    {
                        VehicleNumber = "SXS-001",
                        Make = "Can-Am",
                        Model = "Maverick X3",
                        Year = 2024,
                        Type = VehicleType.SideBySide,
                        Status = VehicleStatus.Available,
                        CurrentHours = 25.0m,
                        CurrentMileage = 0,
                        NextMaintenanceHours = 50.0m,
                        CreatedDate = DateTime.UtcNow
                    }
                };

                _context.Vehicles.AddRange(vehicles);
                await _context.SaveChangesAsync();
            }

            // Add checklist templates if empty
            if (!await _context.ChecklistTemplates.AnyAsync())
            {
                var templates = new List<ChecklistTemplate>
                {
                    new ChecklistTemplate
                    {
                        Name = "Pre-Rental Inspection - 4-Wheeler",
                        
                        Type = ChecklistType.PreRental,
                        VehicleType = VehicleType.FourWheeler,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        TemplateItems = new List<ChecklistTemplateItem>
                        {
                            new ChecklistTemplateItem { ItemText = "Check tire pressure and condition", SortOrder = 1, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Inspect brakes", SortOrder = 2, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Check fluid levels (oil, coolant, brake)", SortOrder = 3, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Test lights and signals", SortOrder = 4, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Inspect frame for damage", SortOrder = 5, IsRequired = true }
                        }
                    },
                    new ChecklistTemplate
                    {
                        Name = "Post-Rental Inspection - 4-Wheeler",
                        
                        Type = ChecklistType.PostRental,
                        VehicleType = VehicleType.FourWheeler,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        TemplateItems = new List<ChecklistTemplateItem>
                        {
                            new ChecklistTemplateItem { ItemText = "Document any new damage", SortOrder = 1, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Check for leaks", SortOrder = 2, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Inspect tires for wear", SortOrder = 3, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Record hour meter reading", SortOrder = 4, IsRequired = true }
                        }
                    },
                    new ChecklistTemplate
                    {
                        Name = "Pre-Rental Inspection - Side-by-Side",
                        
                        Type = ChecklistType.PreRental,
                        VehicleType = VehicleType.SideBySide,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        TemplateItems = new List<ChecklistTemplateItem>
                        {
                            new ChecklistTemplateItem { ItemText = "Check tire pressure and condition", SortOrder = 1, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Inspect seat belts and safety equipment", SortOrder = 2, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Test brakes", SortOrder = 3, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Test lights and signals", SortOrder = 4, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Inspect roll cage", SortOrder = 5, IsRequired = true }
                        }
                    },
                    new ChecklistTemplate
                    {
                        Name = "Post-Rental Inspection - Side-by-Side",
                        
                        Type = ChecklistType.PostRental,
                        VehicleType = VehicleType.SideBySide,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        TemplateItems = new List<ChecklistTemplateItem>
                        {
                            new ChecklistTemplateItem { ItemText = "Document damage", SortOrder = 1, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Check for leaks", SortOrder = 2, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Inspect tires and suspension", SortOrder = 3, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Record hour meter", SortOrder = 4, IsRequired = true }
                        }
                    },
                    new ChecklistTemplate
                    {
                        Name = "Daily Inspection",
                        
                        Type = ChecklistType.DailyInspection,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        TemplateItems = new List<ChecklistTemplateItem>
                        {
                            new ChecklistTemplateItem { ItemText = "Visual inspection", SortOrder = 1, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Check for leaks", SortOrder = 2, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Verify tire pressure", SortOrder = 3, IsRequired = true }
                        }
                    },
                    new ChecklistTemplate
                    {
                        Name = "Maintenance Checklist",
                        
                        Type = ChecklistType.DailyInspection,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        TemplateItems = new List<ChecklistTemplateItem>
                        {
                            new ChecklistTemplateItem { ItemText = "Change oil and filter", SortOrder = 1, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Replace air filter", SortOrder = 2, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Check chain/belt tension", SortOrder = 3, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Lubricate moving parts", SortOrder = 4, IsRequired = true },
                            new ChecklistTemplateItem { ItemText = "Inspect brakes", SortOrder = 5, IsRequired = true }
                        }
                    }
                };

                _context.ChecklistTemplates.AddRange(templates);
                await _context.SaveChangesAsync();
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Database ensured and sample data added successfully");
            return response;
        }
        catch (Exception ex)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}\n{ex.StackTrace}");
            return errorResponse;
        }
    }

    [Function("SeedTemplates")]
    public async Task<HttpResponseData> SeedTemplates(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "setup/seed-templates")] HttpRequestData req)
    {
        try
        {
            // Remove existing templates
            var existing = await _context.ChecklistTemplates.ToListAsync();
            _context.ChecklistTemplates.RemoveRange(existing);
            await _context.SaveChangesAsync();

            // Add checklist templates
            var templates = new List<ChecklistTemplate>
            {
                new ChecklistTemplate
                {
                    Name = "Pre-Rental Inspection - 4-Wheeler",
                    Type = ChecklistType.PreRental,
                    VehicleType = VehicleType.FourWheeler,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    TemplateItems = new List<ChecklistTemplateItem>
                    {
                        new ChecklistTemplateItem { ItemText = "Check tire pressure and condition", SortOrder = 1, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Inspect brakes", SortOrder = 2, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Check fluid levels", SortOrder = 3, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Test lights", SortOrder = 4, IsRequired = true }
                    }
                },
                new ChecklistTemplate
                {
                    Name = "Post-Rental Inspection - 4-Wheeler",
                    Type = ChecklistType.PostRental,
                    VehicleType = VehicleType.FourWheeler,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    TemplateItems = new List<ChecklistTemplateItem>
                    {
                        new ChecklistTemplateItem { ItemText = "Document damage", SortOrder = 1, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Check for leaks", SortOrder = 2, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Record hours", SortOrder = 3, IsRequired = true }
                    }
                },
                new ChecklistTemplate
                {
                    Name = "Pre-Rental Inspection - Side-by-Side",
                    Type = ChecklistType.PreRental,
                    VehicleType = VehicleType.SideBySide,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    TemplateItems = new List<ChecklistTemplateItem>
                    {
                        new ChecklistTemplateItem { ItemText = "Check tire pressure", SortOrder = 1, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Inspect seat belts", SortOrder = 2, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Test brakes", SortOrder = 3, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Inspect roll cage", SortOrder = 4, IsRequired = true }
                    }
                },
                new ChecklistTemplate
                {
                    Name = "Post-Rental Inspection - Side-by-Side",
                    Type = ChecklistType.PostRental,
                    VehicleType = VehicleType.SideBySide,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    TemplateItems = new List<ChecklistTemplateItem>
                    {
                        new ChecklistTemplateItem { ItemText = "Document damage", SortOrder = 1, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Check suspension", SortOrder = 2, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Record hours", SortOrder = 3, IsRequired = true }
                    }
                },
                new ChecklistTemplate
                {
                    Name = "Daily Inspection",
                    Type = ChecklistType.DailyInspection,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    TemplateItems = new List<ChecklistTemplateItem>
                    {
                        new ChecklistTemplateItem { ItemText = "Visual inspection", SortOrder = 1, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Check for leaks", SortOrder = 2, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Tire pressure", SortOrder = 3, IsRequired = true }
                    }
                },
                new ChecklistTemplate
                {
                    Name = "Complete Pre-Trip Safety Check",
                    Type = ChecklistType.PreRental,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    TemplateItems = new List<ChecklistTemplateItem>
                    {
                        new ChecklistTemplateItem { ItemText = "Check all tire pressures (front and rear)", SortOrder = 1, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Inspect tire tread depth and condition", SortOrder = 2, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Test front and rear brakes", SortOrder = 3, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Check brake fluid level", SortOrder = 4, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Verify engine oil level", SortOrder = 5, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Check coolant level", SortOrder = 6, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Inspect all lights (headlight, taillight, brake light)", SortOrder = 7, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Test horn and turn signals", SortOrder = 8, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Check throttle operation (smooth return)", SortOrder = 9, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Inspect steering for play or binding", SortOrder = 10, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Verify all controls function properly", SortOrder = 11, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Check for loose or missing bolts", SortOrder = 12, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Inspect frame for cracks or damage", SortOrder = 13, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Check suspension components", SortOrder = 14, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Verify fuel level and check for leaks", SortOrder = 15, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Test start/stop functionality", SortOrder = 16, IsRequired = true },
                        new ChecklistTemplateItem { ItemText = "Check battery connections", SortOrder = 17, IsRequired = false },
                        new ChecklistTemplateItem { ItemText = "Inspect air filter condition", SortOrder = 18, IsRequired = false },
                        new ChecklistTemplateItem { ItemText = "Clean vehicle exterior", SortOrder = 19, IsRequired = false },
                        new ChecklistTemplateItem { ItemText = "Document hour meter reading", SortOrder = 20, IsRequired = true }
                    }
                }
            };

            _context.ChecklistTemplates.AddRange(templates);
            await _context.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Successfully seeded {templates.Count} checklist templates");
            return response;
        }
        catch (Exception ex)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}\n{ex.StackTrace}");
            return errorResponse;
        }
    }
}
