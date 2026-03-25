#!/usr/bin/env python3
"""
Database seed script for Azure SQL Database - Direct execution
Populates all tables with sample data using Python/pymssql
"""

import pymssql
from datetime import datetime, timedelta

# Connection parameters
SERVER = 'offroad-vehicle-sql.database.windows.net'
DATABASE = 'OffroadVehicleRentals'
USERNAME = 'sqladmin'
PASSWORD = 'OffRoad2024!Secure'

def main():
    print("=" * 60)
    print("Offroad Vehicle Rentals - Database Seed Script (Direct)")
    print("=" * 60)
    print()

    conn = pymssql.connect(
        server=SERVER,
        user=USERNAME,
        password=PASSWORD,
        database=DATABASE,
        tds_version='7.4'
    )
    cursor = conn.cursor()
    print("Connected to database")
    print()

    # Clear existing data in reverse dependency order
    print("Clearing existing data...")
    tables_to_clear = [
        'ChecklistItems',
        'ChecklistTemplateItems', 
        'ChecklistTemplates',
        'RepairRecords',
        'MaintenanceRecords',
        'Rentals',
        'Vehicles'
    ]
    
    for table in tables_to_clear:
        try:
            cursor.execute(f'DELETE FROM {table}')
            conn.commit()
            print(f"  Cleared {table}")
        except Exception as e:
            print(f"  Warning clearing {table}: {e}")
    
    print()
    
    # Helper function
    now = datetime.utcnow()
    
    # =============================================
    # SEED VEHICLES
    # =============================================
    print("Seeding Vehicles...")
    vehicles = [
        ('ATV-001', 'Honda', 'TRX450R', 2024, 0, 0, 125.5, 850.0, now - timedelta(days=24), 200.0, 1000.0, 'Recently serviced, runs excellent'),
        ('ATV-002', 'Yamaha', 'YFZ450R', 2023, 0, 0, 210.0, 1250.5, now - timedelta(days=38), 300.0, 1500.0, 'Competition-ready'),
        ('ATV-003', 'Polaris', 'Scrambler XP 1000', 2024, 0, 1, 85.0, 520.0, now - timedelta(days=15), 150.0, 750.0, 'Reserved for weekend rental'),
        ('ATV-004', 'Can-Am', 'Outlander 850', 2023, 0, 2, 340.0, 2100.0, now - timedelta(days=64), 400.0, 2500.0, 'Currently in use - Trail ride package'),
        ('ATV-005', 'Suzuki', 'KingQuad 750', 2022, 0, 3, 450.0, 2800.0, now - timedelta(days=100), 500.0, 3000.0, 'In for routine maintenance'),
        ('SXS-001', 'Polaris', 'RZR XP Turbo S', 2024, 1, 0, 95.0, 620.0, now - timedelta(days=20), 150.0, 800.0, 'Top-of-the-line performance'),
        ('SXS-002', 'Can-Am', 'Maverick X3', 2024, 1, 0, 180.0, 1150.0, now - timedelta(days=33), 250.0, 1400.0, 'Turbo charged, perfect for dunes'),
        ('SXS-003', 'Honda', 'Talon 1000R', 2023, 1, 1, 150.0, 980.0, now - timedelta(days=13), 200.0, 1200.0, 'Family-friendly side-by-side'),
        ('SXS-004', 'Yamaha', 'YXZ1000R SS', 2023, 1, 4, 280.0, 1780.0, now - timedelta(days=74), 350.0, 2000.0, 'Awaiting parts for clutch repair'),
        ('SXS-005', 'Kawasaki', 'Teryx KRX 1000', 2022, 1, 5, 520.0, 3200.0, now - timedelta(days=144), 600.0, 3500.0, 'Engine needs rebuild - out of service'),
    ]
    
    for v in vehicles:
        cursor.execute('''
            INSERT INTO Vehicles (VehicleNumber, Make, Model, Year, Type, Status, CurrentHours, CurrentMileage, 
                                 LastMaintenanceDate, NextMaintenanceHours, NextMaintenanceMileage, Notes, CreatedDate)
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
        ''', (v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], now))
    conn.commit()
    print(f"  Added {len(vehicles)} vehicles")
    
    # Get vehicle IDs
    cursor.execute('SELECT Id, VehicleNumber FROM Vehicles')
    vehicle_ids = {row[1]: row[0] for row in cursor.fetchall()}
    
    # =============================================
    # SEED RENTALS
    # =============================================
    print("Seeding Rentals...")
    rentals = [
        # Active rental
        (vehicle_ids['ATV-004'], 'Mike Thompson', '555-0101', 'mike.thompson@email.com', 
         now - timedelta(days=1), now + timedelta(days=1), 335.0, None, 2080.0, None, 1, 0, 'Trail ride package - 3 day rental'),
        # Upcoming rentals
        (vehicle_ids['ATV-003'], 'Sarah Johnson', '555-0102', 'sarah.j@email.com',
         now + timedelta(days=3), now + timedelta(days=4), 85.0, None, 520.0, None, 0, 0, 'Weekend rental - first time rider'),
        (vehicle_ids['SXS-003'], 'Johnson Family', '555-0103', 'family@johnson.com',
         now + timedelta(days=4), now + timedelta(days=5), 150.0, None, 980.0, None, 0, 0, 'Family outing - 4 passengers'),
        (vehicle_ids['ATV-001'], 'Trail Blazers Club', '555-0104', 'info@trailblazers.org',
         now + timedelta(days=11), now + timedelta(days=13), 125.5, None, 850.0, None, 0, 0, 'Club event - experienced riders'),
        (vehicle_ids['SXS-001'], 'Adventure Tours Inc', '555-0105', 'bookings@adventuretours.com',
         now + timedelta(days=16), now + timedelta(days=18), 95.0, None, 620.0, None, 0, 0, 'Tour group rental'),
        # Completed rentals
        (vehicle_ids['ATV-001'], 'John Smith', '555-0201', 'john.smith@email.com',
         now - timedelta(days=10), now - timedelta(days=10), 115.5, 125.5, 780.0, 850.0, 1, 1, 'Day rental - returned on time'),
        (vehicle_ids['ATV-002'], 'Desert Riders LLC', '555-0202', 'rentals@desertriders.com',
         now - timedelta(days=15), now - timedelta(days=13), 185.0, 210.0, 1100.0, 1250.5, 1, 1, 'Corporate event rental'),
        (vehicle_ids['SXS-001'], 'Emily Davis', '555-0203', 'emily.d@email.com',
         now - timedelta(days=17), now - timedelta(days=17), 80.0, 95.0, 520.0, 620.0, 1, 1, 'Birthday party rental'),
        (vehicle_ids['SXS-002'], 'Robert Wilson', '555-0204', 'rwilson@email.com',
         now - timedelta(days=24), now - timedelta(days=22), 140.0, 180.0, 900.0, 1150.0, 1, 1, 'Dune trip - experienced rider'),
        (vehicle_ids['SXS-003'], 'Martinez Family', '555-0205', 'martinez@family.com',
         now - timedelta(days=33), now - timedelta(days=32), 130.0, 150.0, 900.0, 980.0, 1, 1, 'Family camping trip'),
    ]
    
    for r in rentals:
        cursor.execute('''
            INSERT INTO Rentals (VehicleId, CustomerName, CustomerPhone, CustomerEmail, StartDate, EndDate,
                                StartHours, EndHours, StartMileage, EndMileage, PreRentalChecklistCompleted,
                                PostRentalChecklistCompleted, Notes, CreatedDate)
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
        ''', (r[0], r[1], r[2], r[3], r[4], r[5], r[6], r[7], r[8], r[9], r[10], r[11], r[12], now))
    conn.commit()
    print(f"  Added {len(rentals)} rentals")
    
    # =============================================
    # SEED MAINTENANCE RECORDS
    # =============================================
    print("Seeding Maintenance Records...")
    maintenance = [
        (vehicle_ids['ATV-001'], now - timedelta(days=24), 'Oil Change', 'Regular oil and filter change', 115.0, 750.0, 45.00, 'Mike Tech', 'Used synthetic oil'),
        (vehicle_ids['ATV-001'], now - timedelta(days=84), 'Air Filter', 'Air filter replacement', 90.0, 550.0, 25.00, 'Mike Tech', None),
        (vehicle_ids['ATV-002'], now - timedelta(days=38), 'Full Service', 'Complete service - oil, filter, plugs, brakes', 180.0, 1100.0, 185.00, 'John Mechanic', 'Ready for competition'),
        (vehicle_ids['ATV-002'], now - timedelta(days=125), 'Oil Change', 'Regular oil change', 130.0, 750.0, 45.00, 'Mike Tech', None),
        (vehicle_ids['ATV-003'], now - timedelta(days=15), 'Inspection', 'Pre-season inspection', 80.0, 500.0, 50.00, 'John Mechanic', 'All systems go'),
        (vehicle_ids['ATV-004'], now - timedelta(days=64), 'Full Service', 'Oil change, air filter, spark plug', 300.0, 1900.0, 150.00, 'Mike Tech', 'Adjusted carburetor'),
        (vehicle_ids['ATV-004'], now - timedelta(days=171), 'Brake Service', 'Front and rear brake pads replacement', 220.0, 1400.0, 120.00, 'John Mechanic', None),
        (vehicle_ids['ATV-005'], now, 'Major Service', '500-hour major service in progress', 450.0, 2800.0, 350.00, 'John Mechanic', 'Replacing belt, spark plugs, full fluid change'),
        (vehicle_ids['ATV-005'], now - timedelta(days=100), 'Oil Change', 'Regular oil change', 380.0, 2400.0, 45.00, 'Mike Tech', None),
        (vehicle_ids['SXS-001'], now - timedelta(days=20), 'Tire Rotation', 'Rotated tires, checked pressure', 85.0, 580.0, 30.00, 'Mike Tech', 'All tires in good condition'),
        (vehicle_ids['SXS-001'], now - timedelta(days=74), 'Oil Change', 'Full synthetic oil change', 50.0, 350.0, 65.00, 'Mike Tech', 'Turbocharged engine requires premium oil'),
        (vehicle_ids['SXS-002'], now - timedelta(days=33), 'Full Service', 'Oil, filters, belt inspection', 160.0, 1050.0, 175.00, 'John Mechanic', 'Belt shows minimal wear'),
        (vehicle_ids['SXS-002'], now - timedelta(days=114), 'Alignment', 'Front end alignment', 110.0, 720.0, 85.00, 'Alignment Pro Shop', 'Adjusted toe and camber'),
        (vehicle_ids['SXS-003'], now - timedelta(days=13), 'Inspection', 'Pre-rental inspection', 145.0, 960.0, 40.00, 'Mike Tech', 'Ready for family rental'),
        (vehicle_ids['SXS-003'], now - timedelta(days=59), 'Oil Change', 'Regular oil change', 115.0, 780.0, 55.00, 'Mike Tech', None),
        (vehicle_ids['SXS-004'], now - timedelta(days=74), 'Oil Change', 'Oil change before clutch issue', 250.0, 1600.0, 55.00, 'Mike Tech', 'Noted clutch slipping during test'),
        (vehicle_ids['SXS-005'], now - timedelta(days=144), 'Full Service', 'Last service before engine issues', 500.0, 3100.0, 200.00, 'John Mechanic', 'Engine noise detected - needs further diagnosis'),
        (vehicle_ids['SXS-005'], now - timedelta(days=222), 'Oil Change', 'Regular maintenance', 420.0, 2650.0, 55.00, 'Mike Tech', None),
        (vehicle_ids['ATV-001'], now - timedelta(days=114), 'Brake Inspection', 'Brake pad inspection', 75.0, 450.0, 25.00, 'Mike Tech', 'Pads at 60% - good for another season'),
        (vehicle_ids['ATV-002'], now - timedelta(days=197), 'Suspension Service', 'Shock absorber service', 100.0, 600.0, 180.00, 'Suspension Specialists', 'Rebuilt front shocks'),
    ]
    
    for m in maintenance:
        cursor.execute('''
            INSERT INTO MaintenanceRecords (VehicleId, MaintenanceDate, MaintenanceType, Description,
                                           HoursAtMaintenance, MileageAtMaintenance, Cost, PerformedBy, Notes, CreatedDate)
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
        ''', (m[0], m[1], m[2], m[3], m[4], m[5], m[6], m[7], m[8], now))
    conn.commit()
    print(f"  Added {len(maintenance)} maintenance records")
    
    # =============================================
    # SEED REPAIR RECORDS
    # =============================================
    print("Seeding Repair Records...")
    repairs = [
        (vehicle_ids['ATV-001'], now - timedelta(days=130), 'Throttle cable fraying', 'Replaced throttle cable assembly', 85.00, 'Mike Tech', 0, 'Customer reported sluggish throttle response'),
        (vehicle_ids['ATV-002'], now - timedelta(days=79), 'Rear brake not engaging', 'Replaced rear brake caliper', 220.00, 'John Mechanic', 0, 'Caliper seized due to dirt ingress'),
        (vehicle_ids['ATV-004'], now - timedelta(days=186), 'Engine overheating', 'Replaced thermostat and coolant flush', 145.00, 'John Mechanic', 0, 'Overheating during extended rides'),
        (vehicle_ids['ATV-005'], now - timedelta(days=166), 'Starter motor failure', 'Replaced starter motor', 310.00, 'Mike Tech', 1, 'Warranty claim approved - under 1 year old'),
        (vehicle_ids['SXS-001'], now - timedelta(days=95), 'Windshield crack', 'Replaced windshield', 180.00, 'Glass Repair Co', 0, 'Rock damage during trail ride'),
        (vehicle_ids['SXS-002'], now - timedelta(days=69), 'Drive belt squealing', 'Replaced CVT belt', 250.00, 'John Mechanic', 0, 'Belt worn prematurely due to aggressive riding'),
        (vehicle_ids['SXS-002'], now - timedelta(days=151), 'Door latch broken', 'Replaced passenger door latch', 95.00, 'Mike Tech', 1, 'Manufacturing defect - warranty claim'),
        (vehicle_ids['SXS-003'], now - timedelta(days=115), 'Headlight not working', 'Replaced headlight bulb and checked wiring', 45.00, 'Mike Tech', 0, 'Bulb burned out, wiring intact'),
        (vehicle_ids['SXS-004'], now - timedelta(days=5), 'Clutch slipping', 'Awaiting clutch parts - repair in progress', 0.00, 'John Mechanic', 0, 'Parts ordered - ETA 3-5 days'),
        (vehicle_ids['SXS-004'], now - timedelta(days=232), 'Steering vibration', 'Replaced tie rod ends', 165.00, 'Alignment Pro Shop', 0, 'Excessive play in steering'),
        (vehicle_ids['SXS-005'], now - timedelta(days=52), 'Engine knocking', 'Diagnosis complete - engine rebuild needed', 150.00, 'John Mechanic', 0, 'Quote: $3500 for complete rebuild. Awaiting approval.'),
        (vehicle_ids['SXS-005'], now - timedelta(days=191), 'Exhaust leak', 'Replaced exhaust gasket', 75.00, 'Mike Tech', 0, 'Minor exhaust noise fixed'),
    ]
    
    for r in repairs:
        cursor.execute('''
            INSERT INTO RepairRecords (VehicleId, RepairDate, IssueDescription, RepairDescription,
                                      Cost, PerformedBy, IsWarrantyClaim, Notes, CreatedDate)
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
        ''', (r[0], r[1], r[2], r[3], r[4], r[5], r[6], r[7], now))
    conn.commit()
    print(f"  Added {len(repairs)} repair records")
    
    # =============================================
    # SEED CHECKLIST TEMPLATES
    # =============================================
    print("Seeding Checklist Templates...")
    templates = [
        ('Daily 4-Wheeler Inspection', 0, 0, 1),
        ('Daily Side-by-Side Inspection', 0, 1, 1),
        ('Pre-Rental 4-Wheeler Checklist', 1, 0, 1),
        ('Pre-Rental Side-by-Side Checklist', 1, 1, 1),
        ('Post-Rental 4-Wheeler Checklist', 2, 0, 1),
        ('Post-Rental Side-by-Side Checklist', 2, 1, 1),
    ]
    
    for t in templates:
        cursor.execute('''
            INSERT INTO ChecklistTemplates (Name, Type, VehicleType, IsActive, CreatedDate)
            VALUES (%s, %s, %s, %s, %s)
        ''', (t[0], t[1], t[2], t[3], now))
    conn.commit()
    print(f"  Added {len(templates)} checklist templates")
    
    # Get template IDs
    cursor.execute('SELECT Id, Name FROM ChecklistTemplates')
    template_ids = {row[1]: row[0] for row in cursor.fetchall()}
    
    # =============================================
    # SEED CHECKLIST TEMPLATE ITEMS
    # =============================================
    print("Seeding Checklist Template Items...")
    template_items = [
        # Daily 4-Wheeler (Template 1)
        (template_ids['Daily 4-Wheeler Inspection'], 'Check tire pressure and condition', 1, 1),
        (template_ids['Daily 4-Wheeler Inspection'], 'Check brake function - front and rear', 2, 1),
        (template_ids['Daily 4-Wheeler Inspection'], 'Check throttle operation and return', 3, 1),
        (template_ids['Daily 4-Wheeler Inspection'], 'Inspect for fluid leaks (oil, coolant, fuel)', 4, 1),
        (template_ids['Daily 4-Wheeler Inspection'], 'Check lights and signals', 5, 1),
        (template_ids['Daily 4-Wheeler Inspection'], 'Verify kill switch operation', 6, 1),
        (template_ids['Daily 4-Wheeler Inspection'], 'Check chain/belt tension', 7, 1),
        (template_ids['Daily 4-Wheeler Inspection'], 'Inspect air filter condition', 8, 0),
        (template_ids['Daily 4-Wheeler Inspection'], 'Check oil level', 9, 1),
        (template_ids['Daily 4-Wheeler Inspection'], 'Test steering responsiveness', 10, 1),
        # Daily Side-by-Side (Template 2)
        (template_ids['Daily Side-by-Side Inspection'], 'Check tire pressure and condition (all 4)', 1, 1),
        (template_ids['Daily Side-by-Side Inspection'], 'Check brake function', 2, 1),
        (template_ids['Daily Side-by-Side Inspection'], 'Test steering and play', 3, 1),
        (template_ids['Daily Side-by-Side Inspection'], 'Inspect for fluid leaks', 4, 1),
        (template_ids['Daily Side-by-Side Inspection'], 'Check all lights and signals', 5, 1),
        (template_ids['Daily Side-by-Side Inspection'], 'Verify seat belts function properly', 6, 1),
        (template_ids['Daily Side-by-Side Inspection'], 'Check doors and latches', 7, 1),
        (template_ids['Daily Side-by-Side Inspection'], 'Test horn', 8, 1),
        (template_ids['Daily Side-by-Side Inspection'], 'Check windshield condition', 9, 0),
        (template_ids['Daily Side-by-Side Inspection'], 'Inspect roll cage and safety equipment', 10, 1),
        (template_ids['Daily Side-by-Side Inspection'], 'Check oil and coolant levels', 11, 1),
        (template_ids['Daily Side-by-Side Inspection'], 'Test 4WD engagement (if applicable)', 12, 0),
        # Pre-Rental 4-Wheeler (Template 3)
        (template_ids['Pre-Rental 4-Wheeler Checklist'], 'Verify customer ID and signed waiver', 1, 1),
        (template_ids['Pre-Rental 4-Wheeler Checklist'], 'Record starting hours and mileage', 2, 1),
        (template_ids['Pre-Rental 4-Wheeler Checklist'], 'Complete daily inspection checklist', 3, 1),
        (template_ids['Pre-Rental 4-Wheeler Checklist'], 'Demonstrate controls to customer', 4, 1),
        (template_ids['Pre-Rental 4-Wheeler Checklist'], 'Explain safety rules and trail boundaries', 5, 1),
        (template_ids['Pre-Rental 4-Wheeler Checklist'], 'Provide safety gear (helmet, goggles)', 6, 1),
        (template_ids['Pre-Rental 4-Wheeler Checklist'], 'Take pre-rental photos', 7, 1),
        (template_ids['Pre-Rental 4-Wheeler Checklist'], 'Customer test ride in designated area', 8, 0),
        (template_ids['Pre-Rental 4-Wheeler Checklist'], 'Provide emergency contact information', 9, 1),
        (template_ids['Pre-Rental 4-Wheeler Checklist'], 'Record fuel level', 10, 1),
        # Pre-Rental Side-by-Side (Template 4)
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Verify customer ID and signed waiver', 1, 1),
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Verify driver license', 2, 1),
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Record starting hours and mileage', 3, 1),
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Complete daily inspection checklist', 4, 1),
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Demonstrate all controls to customer', 5, 1),
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Explain safety rules and trail boundaries', 6, 1),
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Provide safety gear for all passengers', 7, 1),
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Take pre-rental photos (all angles)', 8, 1),
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Customer test drive in designated area', 9, 0),
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Provide emergency contact information', 10, 1),
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Record fuel level', 11, 1),
        (template_ids['Pre-Rental Side-by-Side Checklist'], 'Count and record number of passengers', 12, 1),
        # Post-Rental 4-Wheeler (Template 5)
        (template_ids['Post-Rental 4-Wheeler Checklist'], 'Record ending hours and mileage', 1, 1),
        (template_ids['Post-Rental 4-Wheeler Checklist'], 'Inspect for new damage', 2, 1),
        (template_ids['Post-Rental 4-Wheeler Checklist'], 'Take post-rental photos', 3, 1),
        (template_ids['Post-Rental 4-Wheeler Checklist'], 'Check brake function', 4, 1),
        (template_ids['Post-Rental 4-Wheeler Checklist'], 'Inspect tires for damage', 5, 1),
        (template_ids['Post-Rental 4-Wheeler Checklist'], 'Check fluid levels', 6, 0),
        (template_ids['Post-Rental 4-Wheeler Checklist'], 'Record fuel level', 7, 1),
        (template_ids['Post-Rental 4-Wheeler Checklist'], 'Collect safety gear', 8, 1),
        (template_ids['Post-Rental 4-Wheeler Checklist'], 'Document any customer-reported issues', 9, 1),
        (template_ids['Post-Rental 4-Wheeler Checklist'], 'Schedule maintenance if hours threshold reached', 10, 0),
        # Post-Rental Side-by-Side (Template 6)
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Record ending hours and mileage', 1, 1),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Inspect exterior for new damage', 2, 1),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Take post-rental photos (all angles)', 3, 1),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Check brake function', 4, 1),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Inspect all tires for damage', 5, 1),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Check doors and latches', 6, 1),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Inspect windshield condition', 7, 0),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Check fluid levels', 8, 0),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Record fuel level', 9, 1),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Collect all safety gear', 10, 1),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Document any customer-reported issues', 11, 1),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Check interior condition and cleanliness', 12, 0),
        (template_ids['Post-Rental Side-by-Side Checklist'], 'Schedule maintenance if hours threshold reached', 13, 0),
    ]
    
    for ti in template_items:
        cursor.execute('''
            INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, SortOrder, IsRequired)
            VALUES (%s, %s, %s, %s)
        ''', (ti[0], ti[1], ti[2], ti[3]))
    conn.commit()
    print(f"  Added {len(template_items)} checklist template items")
    
    # =============================================
    # SEED CHECKLIST ITEMS (for rentals)
    # =============================================
    print("Seeding Checklist Items...")
    
    # Get rental IDs
    cursor.execute('SELECT Id, CustomerName FROM Rentals')
    rental_ids = {row[1]: row[0] for row in cursor.fetchall()}
    
    checklist_items = [
        # Pre-rental items for Mike Thompson (active rental)
        (rental_ids['Mike Thompson'], vehicle_ids['ATV-004'], 1, 'Verify customer ID and signed waiver', 1, now - timedelta(hours=20), 'Staff Member', 'ID verified'),
        (rental_ids['Mike Thompson'], vehicle_ids['ATV-004'], 1, 'Record starting hours and mileage', 1, now - timedelta(hours=19), 'Staff Member', 'Hours: 335.0, Miles: 2080.0'),
        (rental_ids['Mike Thompson'], vehicle_ids['ATV-004'], 1, 'Complete daily inspection checklist', 1, now - timedelta(hours=18), 'Staff Member', 'All systems go'),
        (rental_ids['Mike Thompson'], vehicle_ids['ATV-004'], 1, 'Demonstrate controls to customer', 1, now - timedelta(hours=17), 'Staff Member', None),
        (rental_ids['Mike Thompson'], vehicle_ids['ATV-004'], 1, 'Provide safety gear (helmet, goggles)', 1, now - timedelta(hours=16), 'Staff Member', 'Helmet #15, Goggles #8'),
        # Pre-rental items for John Smith (completed)
        (rental_ids['John Smith'], vehicle_ids['ATV-001'], 1, 'Verify customer ID and signed waiver', 1, now - timedelta(days=10, hours=9), 'Staff Member', None),
        (rental_ids['John Smith'], vehicle_ids['ATV-001'], 1, 'Record starting hours and mileage', 1, now - timedelta(days=10, hours=8), 'Staff Member', 'Hours: 115.5, Miles: 780.0'),
        (rental_ids['John Smith'], vehicle_ids['ATV-001'], 1, 'Provide safety gear (helmet, goggles)', 1, now - timedelta(days=10, hours=7), 'Staff Member', None),
        # Post-rental items for John Smith (completed)
        (rental_ids['John Smith'], vehicle_ids['ATV-001'], 2, 'Record ending hours and mileage', 1, now - timedelta(days=10, hours=1), 'Staff Member', 'Hours: 125.5, Miles: 850.0'),
        (rental_ids['John Smith'], vehicle_ids['ATV-001'], 2, 'Inspect for new damage', 1, now - timedelta(days=10), 'Staff Member', 'No new damage'),
        (rental_ids['John Smith'], vehicle_ids['ATV-001'], 2, 'Collect safety gear', 1, now - timedelta(days=10), 'Staff Member', 'All returned'),
        # Daily inspection for ATV-001
        (None, vehicle_ids['ATV-001'], 0, 'Check tire pressure and condition', 1, now - timedelta(hours=5), 'Morning Shift', '32 PSI all around'),
        (None, vehicle_ids['ATV-001'], 0, 'Check brake function - front and rear', 1, now - timedelta(hours=4), 'Morning Shift', 'Good'),
        (None, vehicle_ids['ATV-001'], 0, 'Check oil level', 1, now - timedelta(hours=3), 'Morning Shift', 'Full'),
        # Daily inspection for SXS-001
        (None, vehicle_ids['SXS-001'], 0, 'Check tire pressure and condition (all 4)', 1, now - timedelta(hours=5), 'Morning Shift', '28 PSI all around'),
        (None, vehicle_ids['SXS-001'], 0, 'Verify seat belts function properly', 1, now - timedelta(hours=4), 'Morning Shift', 'All working'),
        (None, vehicle_ids['SXS-001'], 0, 'Test horn', 1, now - timedelta(hours=3), 'Morning Shift', 'Working'),
    ]
    
    for ci in checklist_items:
        cursor.execute('''
            INSERT INTO ChecklistItems (RentalId, VehicleId, Type, ItemText, IsCompleted, CompletedDate, CompletedBy, Notes, CreatedDate)
            VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
        ''', (ci[0], ci[1], ci[2], ci[3], ci[4], ci[5], ci[6], ci[7], now))
    conn.commit()
    print(f"  Added {len(checklist_items)} checklist items")
    
    # =============================================
    # SUMMARY
    # =============================================
    print()
    print("=" * 60)
    print("Seed completed successfully!")
    print("=" * 60)
    print()
    
    # Verify counts
    tables = ['Vehicles', 'Rentals', 'MaintenanceRecords', 'RepairRecords', 'ChecklistTemplates', 'ChecklistTemplateItems', 'ChecklistItems']
    for table in tables:
        cursor.execute(f'SELECT COUNT(*) FROM {table}')
        count = cursor.fetchone()[0]
        print(f"  {table}: {count} records")
    
    print()
    print("Test your data at:")
    print("  - Web: https://offroad-vehicle-web.azurewebsites.net")
    print("  - API: https://offroad-vehicle-api.azurewebsites.net/api/vehicles")
    
    cursor.close()
    conn.close()

if __name__ == '__main__':
    main()

