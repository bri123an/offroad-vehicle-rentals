# Offroad Vehicle Rentals - Public API Endpoints

## Base URL
```
https://brian-app-apim.azure-api.net/vehicles
```

**Note:** All endpoints are publicly accessible without authentication through Azure API Management.

---

## Vehicles

### Get All Vehicles
```http
GET /vehicles/vehicles
```

**Response:** Array of vehicle objects
```json
[
  {
    "Id": 1,
    "VehicleNumber": "ATV-001",
    "Make": "Polaris",
    "Model": "Sportsman 570",
    "Year": 2023,
    "Type": 0,
    "Status": 0,
    "CurrentHours": 45.50,
    "CurrentMileage": 0.00,
    "NextMaintenanceHours": 100.00,
    "CreatedDate": "2026-03-25T04:54:53.1767224Z"
  }
]
```

### Get Vehicle by ID
```http
GET /vehicles/vehicles/{id}
```

### Get Available Vehicles
```http
GET /vehicles/vehicles/available
```

### Create Vehicle
```http
POST /vehicles/vehicles
Content-Type: application/json

{
  "vehicleNumber": "ATV-004",
  "make": "Yamaha",
  "model": "Grizzly 700",
  "year": 2023,
  "type": 0,
  "status": 0,
  "currentHours": 0,
  "currentMileage": 0,
  "nextMaintenanceHours": 50
}
```

### Update Vehicle
```http
PUT /vehicles/vehicles/{id}
Content-Type: application/json
```

### Delete Vehicle
```http
DELETE /vehicles/vehicles/{id}
```

---

## Rentals

### Get All Rentals
```http
GET /vehicles/rentals
```

### Get Rental by ID
```http
GET /vehicles/rentals/{id}
```

### Get Upcoming Rentals
```http
GET /vehicles/rentals/upcoming
```

### Get Active Rentals
```http
GET /vehicles/rentals/active
```

### Create Rental
```http
POST /vehicles/rentals
Content-Type: application/json

{
  "vehicleId": 1,
  "customerName": "John Smith",
  "customerPhone": "555-0100",
  "customerEmail": "john@example.com",
  "startDate": "2026-03-26T08:00:00Z",
  "endDate": "2026-03-26T16:00:00Z",
  "status": 1
}
```

### Update Rental
```http
PUT /vehicles/rentals/{id}
Content-Type: application/json
```

### Delete Rental
```http
DELETE /vehicles/rentals/{id}
```

---

## Maintenance

### Get All Maintenance Records
```http
GET /vehicles/maintenance
```

### Get Maintenance Records for Vehicle
```http
GET /vehicles/vehicles/{vehicleId}/maintenance
```

### Create Maintenance Record
```http
POST /vehicles/maintenance
Content-Type: application/json

{
  "vehicleId": 1,
  "maintenanceType": "Oil Change",
  "description": "Regular oil and filter change",
  "performedDate": "2026-03-25T10:00:00Z",
  "cost": 75.00,
  "performedBy": "Mechanic Name"
}
```

### Update Maintenance Record
```http
PUT /vehicles/maintenance/{id}
Content-Type: application/json
```

### Delete Maintenance Record
```http
DELETE /vehicles/maintenance/{id}
```

---

## Repairs

### Get All Repair Records
```http
GET /vehicles/repairs
```

### Get Repair Records for Vehicle
```http
GET /vehicles/vehicles/{vehicleId}/repairs
```

### Create Repair Record
```http
POST /vehicles/repairs
Content-Type: application/json

{
  "vehicleId": 1,
  "damageDescription": "Broken headlight",
  "repairDescription": "Replaced headlight assembly",
  "reportedDate": "2026-03-25T09:00:00Z",
  "repairedDate": "2026-03-25T14:00:00Z",
  "cost": 125.00,
  "repairedBy": "Mechanic Name"
}
```

### Update Repair Record
```http
PUT /vehicles/repairs/{id}
Content-Type: application/json
```

### Delete Repair Record
```http
DELETE /vehicles/repairs/{id}
```

---

## Checklist Templates

### Get All Checklist Templates
```http
GET /vehicles/checklist-templates
```

**Response:** Array of templates with items
```json
[
  {
    "Id": 1,
    "Name": "Pre-Rental Inspection - 4-Wheeler",
    "Type": 1,
    "VehicleType": 0,
    "IsActive": true,
    "TemplateItems": [
      {
        "Id": 1,
        "ItemText": "Check tire pressure and condition",
        "SortOrder": 1,
        "IsRequired": true
      }
    ]
  }
]
```

### Get Checklist Template by ID
```http
GET /vehicles/checklist-templates/{id}
```

---

## Checklist Items

### Get Checklist Items for Rental
```http
GET /vehicles/rentals/{rentalId}/checklist-items
```

### Get Checklist Items for Vehicle
```http
GET /vehicles/vehicles/{vehicleId}/checklist-items
```

### Create Checklist Items from Template
```http
POST /vehicles/checklist-items/from-template
Content-Type: application/json

{
  "templateId": 1,
  "rentalId": 1,
  "vehicleId": null
}
```

### Update Checklist Item
```http
PUT /vehicles/checklist-items/{id}
Content-Type: application/json

{
  "isCompleted": true,
  "completedBy": "Staff Name",
  "notes": "All checks passed"
}
```

---

## Enums

### VehicleType
- `0` = FourWheeler
- `1` = SideBySide

### VehicleStatus
- `0` = Available
- `1` = Reserved
- `2` = InUse
- `3` = InMaintenance
- `4` = InRepair
- `5` = OutOfService

### ChecklistType
- `0` = DailyInspection
- `1` = PreRental
- `2` = PostRental

### RentalStatus
- `0` = Reserved
- `1` = Active
- `2` = Completed
- `3` = Cancelled

---

## Examples

### cURL Example: Get All Vehicles
```bash
curl https://brian-app-apim.azure-api.net/vehicles/vehicles
```

### cURL Example: Create a Vehicle
```bash
curl -X POST https://brian-app-apim.azure-api.net/vehicles/vehicles \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleNumber": "ATV-005",
    "make": "Honda",
    "model": "Rancher 420",
    "year": 2024,
    "type": 0,
    "status": 0,
    "currentHours": 0,
    "currentMileage": 0
  }'
```

### JavaScript Example
```javascript
// Get all vehicles
fetch('https://brian-app-apim.azure-api.net/vehicles/vehicles')
  .then(response => response.json())
  .then(data => console.log(data));

// Create a rental
fetch('https://brian-app-apim.azure-api.net/vehicles/rentals', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    vehicleId: 1,
    customerName: 'Jane Doe',
    customerPhone: '555-0200',
    startDate: '2026-03-27T08:00:00Z',
    endDate: '2026-03-27T16:00:00Z',
    status: 1
  })
})
.then(response => response.json())
.then(data => console.log(data));
```

---

## Web Application
The management web application is available at:
```
https://offroad-vehicle-web.azurewebsites.net
```

Features:
- Dashboard with fleet overview
- Vehicle management
- Rental scheduling
- Maintenance tracking
- Repair records
- Daily and rental checklists
