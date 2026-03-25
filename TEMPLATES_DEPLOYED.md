# Checklist Templates Deployed

All 10 checklist templates have been successfully deployed and seeded to the production database.

## Available Templates

### Rental Inspection Templates
1. **Pre-Rental Inspection - 4-Wheeler** (Type: PreRental)
2. **Post-Rental Inspection - 4-Wheeler** (Type: PostRental)
3. **Pre-Rental Inspection - Side-by-Side** (Type: PreRental)
4. **Post-Rental Inspection - Side-by-Side** (Type: PostRental)

### Daily Inspection Templates
5. **Daily Inspection** (Type: DailyInspection)
6. **Complete Pre-Trip Safety Check** (Type: PreRental) - 20 comprehensive items

### Maintenance Templates - 4-Wheeler
7. **50-Hour Maintenance - 4-Wheeler** (Type: DailyInspection)
   - 10 maintenance tasks including oil change, air filter, chain/belt, lubrication
8. **100-Hour Maintenance - 4-Wheeler** (Type: DailyInspection)
   - 16 comprehensive tasks including spark plug, coolant, valve clearance, brake fluid

### Maintenance Templates - Side-by-Side
9. **50-Hour Maintenance - Side-by-Side** (Type: DailyInspection)
   - 12 tasks including CVT belt service, brake inspection, safety checks
10. **100-Hour Maintenance - Side-by-Side** (Type: DailyInspection)
    - 20 detailed tasks including differential fluid, shock absorbers, A-arms, 4WD testing

## Accessing Templates

Templates are accessible via:
- **Web App**: https://offroad-vehicle-web.azurewebsites.net/checklists
- **API**: https://brian-app-apim.azure-api.net/vehicles/checklist-templates
- **Direct API**: https://offroad-vehicle-api.azurewebsites.net/api/checklist-templates

## Reseeding Templates

To reseed templates with latest changes:
```bash
curl -X POST "https://offroad-vehicle-api.azurewebsites.net/api/setup/seed-templates?code=YOUR_FUNCTION_KEY"
```

Last seeded: March 25, 2026 - 10 templates
