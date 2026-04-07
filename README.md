# Offroad Vehicle Rentals Management System

A comprehensive fleet management system for offroad vehicle rentals (4-wheelers and side-by-sides) built with Blazor, Azure Functions, and Azure SQL Database.

## Features

- **Vehicle Management**: Track your fleet of 4-wheelers and side-by-sides with detailed information including hours, mileage, and status
- **Rental Queue**: Schedule and manage upcoming rentals with conflict detection
- **Maintenance Tracking**: Record and monitor scheduled maintenance with hour/mileage intervals
- **Repair Management**: Track repairs and damages with warranty claim support
- **Checklist System**: Daily inspections and pre/post-rental checklists with templates

## Architecture

### Projects

- **OffroadVehicleRentals.Shared**: Domain models shared between API and Web app
- **OffroadVehicleRentals.Api**: Azure Functions API with Entity Framework Core
- **OffroadVehicleRentals.Web**: Blazor Server web application

### Technology Stack

- .NET 8.0
- Blazor Server
- Azure Functions v4 (Isolated Worker)
- Entity Framework Core 8.0
- Azure SQL Database
- Azure API Management (APIM)

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- Azure subscription
- Azure SQL Database
- Azure API Management instance

### 1. Database Setup

1. Create an Azure SQL Database
2. Run the SQL schema script located at `src/OffroadVehicleRentals.Api/Data/DbInitializer.sql`
3. Note your connection string

### 2. Azure Functions Configuration

1. Navigate to `src/OffroadVehicleRentals.Api`
2. Update `local.settings.json`:
   ```json
   {
     "Values": {
       "SqlConnectionString": "Server=tcp:YOUR_SERVER.database.windows.net,1433;Database=OffroadVehicleRentals;User ID=YOUR_USERNAME;Password=YOUR_PASSWORD;Encrypt=True;",
       "ApimBaseUrl": "https://YOUR_APIM_INSTANCE.azure-api.net",
       "ApimSubscriptionKey": "YOUR_APIM_SUBSCRIPTION_KEY"
     }
   }
   ```

### 3. Deploy Azure Functions

```bash
cd src/OffroadVehicleRentals.Api
dotnet restore
dotnet build
# Deploy to Azure Functions using your preferred method (VS Code, Visual Studio, or Azure CLI)
```

### 4. Configure Azure APIM

1. Import your Azure Functions into APIM
2. Create API endpoints for:
   - `/vehicles/*` - Vehicle management
   - `/rentals/*` - Rental management
   - `/maintenance/*` - Maintenance records
   - `/repairs/*` - Repair records
   - `/checklist-templates/*` - Checklist templates
   - `/checklist-items/*` - Checklist items

3. Configure subscription keys and policies as needed

### 5. Blazor Web App Configuration

1. Navigate to `src/OffroadVehicleRentals.Web`
2. Update `appsettings.json`:
   ```json
   {
     "ApimBaseUrl": "https://YOUR_APIM_INSTANCE.azure-api.net",
     "ApimSubscriptionKey": "YOUR_APIM_SUBSCRIPTION_KEY"
   }
   ```

### 6. Run the Application

For local development:

```bash
# Terminal 1 - Run Azure Functions locally
cd src/OffroadVehicleRentals.Api
func start

# Terminal 2 - Run Blazor app
cd src/OffroadVehicleRentals.Web
dotnet run
```

For production deployment:
- Deploy Azure Functions to Azure
- Deploy Blazor app to Azure App Service or your hosting platform of choice

## API Endpoints

### Vehicles
- `GET /vehicles` - Get all vehicles
- `GET /vehicles/{id}` - Get vehicle by ID
- `GET /vehicles/available` - Get available vehicles
- `POST /vehicles` - Create new vehicle
- `PUT /vehicles/{id}` - Update vehicle
- `DELETE /vehicles/{id}` - Delete vehicle

### Rentals
- `GET /rentals` - Get all rentals
- `GET /rentals/upcoming` - Get upcoming rentals
- `GET /rentals/active` - Get active rentals
- `GET /rentals/{id}` - Get rental by ID
- `POST /rentals` - Create new rental
- `PUT /rentals/{id}` - Update rental
- `DELETE /rentals/{id}` - Delete rental

### Maintenance
- `GET /maintenance` - Get all maintenance records
- `GET /vehicles/{vehicleId}/maintenance` - Get maintenance for vehicle
- `POST /maintenance` - Create maintenance record
- `PUT /maintenance/{id}` - Update maintenance record
- `DELETE /maintenance/{id}` - Delete maintenance record

### Repairs
- `GET /repairs` - Get all repair records
- `GET /vehicles/{vehicleId}/repairs` - Get repairs for vehicle
- `POST /repairs` - Create repair record
- `PUT /repairs/{id}` - Update repair record
- `DELETE /repairs/{id}` - Delete repair record

### Checklists
- `GET /checklist-templates` - Get all templates
- `GET /rentals/{rentalId}/checklist-items` - Get rental checklist items
- `GET /vehicles/{vehicleId}/checklist-items` - Get vehicle checklist items
- `POST /checklist-items/from-template` - Create items from template
- `PUT /checklist-items/{id}` - Update checklist item

## Database Schema

The system uses the following main entities:

- **Vehicles**: Core vehicle information and status
- **Rentals**: Customer rentals with date/time tracking
- **MaintenanceRecords**: Scheduled maintenance history
- **RepairRecords**: Damage repairs and warranty claims
- **ChecklistTemplates**: Reusable checklist templates
- **ChecklistItems**: Individual checklist completions

## Configuration Notes

### APIM Integration

The application is designed to work with your existing Azure APIM instance. Configure the APIM base URL and subscription key in both the API and Web app settings.

### Security

- Update Azure Functions authorization level as needed (currently set to `Function`)
- Configure APIM policies for rate limiting, authentication, etc.
- Use Azure Key Vault for storing secrets in production
- Enable authentication in your Blazor app for production use

## Future Enhancements

- User authentication and role-based access control
- Email notifications for upcoming rentals
- Automated maintenance reminders based on hours/mileage
- Photo uploads for damage documentation
- Reporting and analytics dashboard
- Mobile app support

## CI/CD

This project uses GitHub Actions for continuous integration and deployment:

- **CI Workflow**: Automatically builds and tests on every push/PR
- **CD Workflows**: Automatically deploys to Azure on push to main branch
  - API deploys to Azure Functions
  - Web app deploys to Azure App Service

See `.github/workflows/` for workflow configurations.

## Support

For issues or questions, please create an issue in the repository.


