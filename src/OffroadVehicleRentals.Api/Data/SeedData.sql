-- Seed Data Script for Offroad Vehicle Rentals
-- Run this script AFTER DbInitializer.sql to populate all tables with sample data

-- =============================================
-- CLEAR EXISTING DATA (in correct order due to foreign keys)
-- =============================================
DELETE FROM ChecklistItems;
DELETE FROM ChecklistTemplateItems;
DELETE FROM ChecklistTemplates;
DELETE FROM RepairRecords;
DELETE FROM MaintenanceRecords;
DELETE FROM Rentals;
DELETE FROM Vehicles;

-- Reset identity columns
DBCC CHECKIDENT ('Vehicles', RESEED, 0);
DBCC CHECKIDENT ('Rentals', RESEED, 0);
DBCC CHECKIDENT ('MaintenanceRecords', RESEED, 0);
DBCC CHECKIDENT ('RepairRecords', RESEED, 0);
DBCC CHECKIDENT ('ChecklistTemplates', RESEED, 0);
DBCC CHECKIDENT ('ChecklistTemplateItems', RESEED, 0);
DBCC CHECKIDENT ('ChecklistItems', RESEED, 0);

-- =============================================
-- VEHICLES (10 sample vehicles)
-- =============================================
-- Type: 0 = FourWheeler, 1 = SideBySide
-- Status: 0 = Available, 1 = Reserved, 2 = InUse, 3 = InMaintenance, 4 = InRepair, 5 = OutOfService

INSERT INTO Vehicles (VehicleNumber, Make, Model, Year, Type, Status, CurrentHours, CurrentMileage, LastMaintenanceDate, NextMaintenanceHours, NextMaintenanceMileage, Notes, CreatedDate) VALUES
('ATV-001', 'Honda', 'TRX450R', 2024, 0, 0, 125.5, 850.0, '2026-03-01', 200.0, 1000.0, 'Recently serviced, runs excellent', GETUTCDATE()),
('ATV-002', 'Yamaha', 'YFZ450R', 2023, 0, 0, 210.0, 1250.5, '2026-02-15', 300.0, 1500.0, 'Competition-ready', GETUTCDATE()),
('ATV-003', 'Polaris', 'Scrambler XP 1000', 2024, 0, 1, 85.0, 520.0, '2026-03-10', 150.0, 750.0, 'Reserved for weekend rental', GETUTCDATE()),
('ATV-004', 'Can-Am', 'Outlander 850', 2023, 0, 2, 340.0, 2100.0, '2026-01-20', 400.0, 2500.0, 'Currently in use - Trail ride package', GETUTCDATE()),
('ATV-005', 'Suzuki', 'KingQuad 750', 2022, 0, 3, 450.0, 2800.0, '2025-12-15', 500.0, 3000.0, 'In for routine maintenance', GETUTCDATE()),
('SXS-001', 'Polaris', 'RZR XP Turbo S', 2024, 1, 0, 95.0, 620.0, '2026-03-05', 150.0, 800.0, 'Top-of-the-line performance', GETUTCDATE()),
('SXS-002', 'Can-Am', 'Maverick X3', 2024, 1, 0, 180.0, 1150.0, '2026-02-20', 250.0, 1400.0, 'Turbo charged, perfect for dunes', GETUTCDATE()),
('SXS-003', 'Honda', 'Talon 1000R', 2023, 1, 1, 150.0, 980.0, '2026-03-12', 200.0, 1200.0, 'Family-friendly side-by-side', GETUTCDATE()),
('SXS-004', 'Yamaha', 'YXZ1000R SS', 2023, 1, 4, 280.0, 1780.0, '2026-01-10', 350.0, 2000.0, 'Awaiting parts for clutch repair', GETUTCDATE()),
('SXS-005', 'Kawasaki', 'Teryx KRX 1000', 2022, 1, 5, 520.0, 3200.0, '2025-11-01', 600.0, 3500.0, 'Engine needs rebuild - out of service', GETUTCDATE());

-- =============================================
-- RENTALS (15 sample rentals across different states)
-- =============================================

-- Active rentals (currently ongoing)
INSERT INTO Rentals (VehicleId, CustomerName, CustomerPhone, CustomerEmail, StartDate, EndDate, StartHours, EndHours, StartMileage, EndMileage, PreRentalChecklistCompleted, PostRentalChecklistCompleted, Notes, CreatedDate) VALUES
(4, 'Mike Thompson', '555-0101', 'mike.thompson@email.com', '2026-03-24 08:00:00', '2026-03-26 18:00:00', 335.0, NULL, 2080.0, NULL, 1, 0, 'Trail ride package - 3 day rental', GETUTCDATE());

-- Reserved/upcoming rentals
INSERT INTO Rentals (VehicleId, CustomerName, CustomerPhone, CustomerEmail, StartDate, EndDate, StartHours, EndHours, StartMileage, EndMileage, PreRentalChecklistCompleted, PostRentalChecklistCompleted, Notes, CreatedDate) VALUES
(3, 'Sarah Johnson', '555-0102', 'sarah.j@email.com', '2026-03-28 09:00:00', '2026-03-29 17:00:00', 85.0, NULL, 520.0, NULL, 0, 0, 'Weekend rental - first time rider', GETUTCDATE()),
(8, 'Johnson Family', '555-0103', 'family@johnson.com', '2026-03-29 08:00:00', '2026-03-30 18:00:00', 150.0, NULL, 980.0, NULL, 0, 0, 'Family outing - 4 passengers', GETUTCDATE()),
(1, 'Trail Blazers Club', '555-0104', 'info@trailblazers.org', '2026-04-05 07:00:00', '2026-04-07 19:00:00', 125.5, NULL, 850.0, NULL, 0, 0, 'Club event - experienced riders', GETUTCDATE()),
(6, 'Adventure Tours Inc', '555-0105', 'bookings@adventuretours.com', '2026-04-10 06:00:00', '2026-04-12 20:00:00', 95.0, NULL, 620.0, NULL, 0, 0, 'Tour group rental', GETUTCDATE());

-- Completed rentals (past)
INSERT INTO Rentals (VehicleId, CustomerName, CustomerPhone, CustomerEmail, StartDate, EndDate, StartHours, EndHours, StartMileage, EndMileage, PreRentalChecklistCompleted, PostRentalChecklistCompleted, Notes, CreatedDate) VALUES
(1, 'John Smith', '555-0201', 'john.smith@email.com', '2026-03-15 09:00:00', '2026-03-15 17:00:00', 115.5, 125.5, 780.0, 850.0, 1, 1, 'Day rental - returned on time', DATEADD(day, -10, GETUTCDATE())),
(2, 'Desert Riders LLC', '555-0202', 'rentals@desertriders.com', '2026-03-10 08:00:00', '2026-03-12 18:00:00', 185.0, 210.0, 1100.0, 1250.5, 1, 1, 'Corporate event rental', DATEADD(day, -15, GETUTCDATE())),
(6, 'Emily Davis', '555-0203', 'emily.d@email.com', '2026-03-08 10:00:00', '2026-03-08 16:00:00', 80.0, 95.0, 520.0, 620.0, 1, 1, 'Birthday party rental', DATEADD(day, -17, GETUTCDATE())),
(7, 'Robert Wilson', '555-0204', 'rwilson@email.com', '2026-03-01 07:00:00', '2026-03-03 19:00:00', 140.0, 180.0, 900.0, 1150.0, 1, 1, 'Dune trip - experienced rider', DATEADD(day, -24, GETUTCDATE())),
(8, 'Martinez Family', '555-0205', 'martinez@family.com', '2026-02-20 09:00:00', '2026-02-21 17:00:00', 130.0, 150.0, 900.0, 980.0, 1, 1, 'Family camping trip', DATEADD(day, -33, GETUTCDATE())),
(1, 'Chris Anderson', '555-0206', 'c.anderson@email.com', '2026-02-14 08:00:00', '2026-02-14 18:00:00', 100.0, 115.5, 700.0, 780.0, 1, 1, 'Valentines day special rental', DATEADD(day, -39, GETUTCDATE())),
(2, 'Trail Masters Inc', '555-0207', 'info@trailmasters.com', '2026-02-05 06:00:00', '2026-02-08 20:00:00', 150.0, 185.0, 850.0, 1100.0, 1, 1, 'Extended tour - professional guides', DATEADD(day, -48, GETUTCDATE())),
(6, 'Weekend Warriors', '555-0208', 'bookings@weekendwarriors.net', '2026-01-25 07:00:00', '2026-01-26 19:00:00', 55.0, 80.0, 380.0, 520.0, 1, 1, 'Group rental - 2 vehicles', DATEADD(day, -59, GETUTCDATE())),
(7, 'Alex Turner', '555-0209', 'alex.t@email.com', '2026-01-15 09:00:00', '2026-01-17 17:00:00', 100.0, 140.0, 650.0, 900.0, 1, 1, 'Mountain trail adventure', DATEADD(day, -69, GETUTCDATE())),
(8, 'Corporate Events Co', '555-0210', 'events@corporate.com', '2026-01-05 08:00:00', '2026-01-07 18:00:00', 90.0, 130.0, 780.0, 900.0, 1, 1, 'Team building event', DATEADD(day, -79, GETUTCDATE()));

-- =============================================
-- MAINTENANCE RECORDS (20 sample records)
-- =============================================
INSERT INTO MaintenanceRecords (VehicleId, MaintenanceDate, MaintenanceType, Description, HoursAtMaintenance, MileageAtMaintenance, Cost, PerformedBy, Notes, CreatedDate) VALUES
-- ATV-001
(1, '2026-03-01', 'Oil Change', 'Regular oil and filter change', 115.0, 750.0, 45.00, 'Mike Tech', 'Used synthetic oil', GETUTCDATE()),
(1, '2026-01-15', 'Air Filter', 'Air filter replacement', 90.0, 550.0, 25.00, 'Mike Tech', NULL, DATEADD(day, -69, GETUTCDATE())),
-- ATV-002
(2, '2026-02-15', 'Full Service', 'Complete service - oil, filter, plugs, brakes', 180.0, 1100.0, 185.00, 'John Mechanic', 'Ready for competition', GETUTCDATE()),
(2, '2025-11-20', 'Oil Change', 'Regular oil change', 130.0, 750.0, 45.00, 'Mike Tech', NULL, DATEADD(day, -125, GETUTCDATE())),
-- ATV-003
(3, '2026-03-10', 'Inspection', 'Pre-season inspection', 80.0, 500.0, 50.00, 'John Mechanic', 'All systems go', GETUTCDATE()),
-- ATV-004
(4, '2026-01-20', 'Full Service', 'Oil change, air filter, spark plug', 300.0, 1900.0, 150.00, 'Mike Tech', 'Adjusted carburetor', DATEADD(day, -64, GETUTCDATE())),
(4, '2025-10-05', 'Brake Service', 'Front and rear brake pads replacement', 220.0, 1400.0, 120.00, 'John Mechanic', NULL, DATEADD(day, -171, GETUTCDATE())),
-- ATV-005 (currently in maintenance)
(5, '2026-03-25', 'Major Service', '500-hour major service in progress', 450.0, 2800.0, 350.00, 'John Mechanic', 'Replacing belt, spark plugs, full fluid change', GETUTCDATE()),
(5, '2025-12-15', 'Oil Change', 'Regular oil change', 380.0, 2400.0, 45.00, 'Mike Tech', NULL, DATEADD(day, -100, GETUTCDATE())),
-- SXS-001
(6, '2026-03-05', 'Tire Rotation', 'Rotated tires, checked pressure', 85.0, 580.0, 30.00, 'Mike Tech', 'All tires in good condition', GETUTCDATE()),
(6, '2026-01-10', 'Oil Change', 'Full synthetic oil change', 50.0, 350.0, 65.00, 'Mike Tech', 'Turbocharged engine requires premium oil', DATEADD(day, -74, GETUTCDATE())),
-- SXS-002
(7, '2026-02-20', 'Full Service', 'Oil, filters, belt inspection', 160.0, 1050.0, 175.00, 'John Mechanic', 'Belt shows minimal wear', GETUTCDATE()),
(7, '2025-12-01', 'Alignment', 'Front end alignment', 110.0, 720.0, 85.00, 'Alignment Pro Shop', 'Adjusted toe and camber', DATEADD(day, -114, GETUTCDATE())),
-- SXS-003
(8, '2026-03-12', 'Inspection', 'Pre-rental inspection', 145.0, 960.0, 40.00, 'Mike Tech', 'Ready for family rental', GETUTCDATE()),
(8, '2026-01-25', 'Oil Change', 'Regular oil change', 115.0, 780.0, 55.00, 'Mike Tech', NULL, DATEADD(day, -59, GETUTCDATE())),
-- SXS-004
(9, '2026-01-10', 'Oil Change', 'Oil change before clutch issue', 250.0, 1600.0, 55.00, 'Mike Tech', 'Noted clutch slipping during test', DATEADD(day, -74, GETUTCDATE())),
-- SXS-005
(10, '2025-11-01', 'Full Service', 'Last service before engine issues', 500.0, 3100.0, 200.00, 'John Mechanic', 'Engine noise detected - needs further diagnosis', DATEADD(day, -144, GETUTCDATE())),
(10, '2025-08-15', 'Oil Change', 'Regular maintenance', 420.0, 2650.0, 55.00, 'Mike Tech', NULL, DATEADD(day, -222, GETUTCDATE())),
-- Additional records
(1, '2025-12-01', 'Brake Inspection', 'Brake pad inspection', 75.0, 450.0, 25.00, 'Mike Tech', 'Pads at 60% - good for another season', DATEADD(day, -114, GETUTCDATE())),
(2, '2025-09-10', 'Suspension Service', 'Shock absorber service', 100.0, 600.0, 180.00, 'Suspension Specialists', 'Rebuilt front shocks', DATEADD(day, -197, GETUTCDATE()));

-- =============================================
-- REPAIR RECORDS (12 sample records)
-- =============================================
INSERT INTO RepairRecords (VehicleId, RepairDate, IssueDescription, RepairDescription, Cost, PerformedBy, IsWarrantyClaim, Notes, CreatedDate) VALUES
-- ATV-001
(1, '2025-11-15', 'Throttle cable fraying', 'Replaced throttle cable assembly', 85.00, 'Mike Tech', 0, 'Customer reported sluggish throttle response', DATEADD(day, -130, GETUTCDATE())),
-- ATV-002
(2, '2026-01-05', 'Rear brake not engaging', 'Replaced rear brake caliper', 220.00, 'John Mechanic', 0, 'Caliper seized due to dirt ingress', DATEADD(day, -79, GETUTCDATE())),
-- ATV-004
(4, '2025-09-20', 'Engine overheating', 'Replaced thermostat and coolant flush', 145.00, 'John Mechanic', 0, 'Overheating during extended rides', DATEADD(day, -186, GETUTCDATE())),
-- ATV-005
(5, '2025-10-10', 'Starter motor failure', 'Replaced starter motor', 310.00, 'Mike Tech', 1, 'Warranty claim approved - under 1 year old', DATEADD(day, -166, GETUTCDATE())),
-- SXS-001
(6, '2025-12-20', 'Windshield crack', 'Replaced windshield', 180.00, 'Glass Repair Co', 0, 'Rock damage during trail ride', DATEADD(day, -95, GETUTCDATE())),
-- SXS-002
(7, '2026-01-15', 'Drive belt squealing', 'Replaced CVT belt', 250.00, 'John Mechanic', 0, 'Belt worn prematurely due to aggressive riding', DATEADD(day, -69, GETUTCDATE())),
(7, '2025-10-25', 'Door latch broken', 'Replaced passenger door latch', 95.00, 'Mike Tech', 1, 'Manufacturing defect - warranty claim', DATEADD(day, -151, GETUTCDATE())),
-- SXS-003
(8, '2025-11-30', 'Headlight not working', 'Replaced headlight bulb and checked wiring', 45.00, 'Mike Tech', 0, 'Bulb burned out, wiring intact', DATEADD(day, -115, GETUTCDATE())),
-- SXS-004 (currently in repair)
(9, '2026-03-20', 'Clutch slipping', 'Awaiting clutch parts - repair in progress', 0.00, 'John Mechanic', 0, 'Parts ordered - ETA 3-5 days', GETUTCDATE()),
(9, '2025-08-05', 'Steering vibration', 'Replaced tie rod ends', 165.00, 'Alignment Pro Shop', 0, 'Excessive play in steering', DATEADD(day, -232, GETUTCDATE())),
-- SXS-005
(10, '2026-02-01', 'Engine knocking', 'Diagnosis complete - engine rebuild needed', 150.00, 'John Mechanic', 0, 'Quote: $3500 for complete rebuild. Awaiting approval.', DATEADD(day, -52, GETUTCDATE())),
(10, '2025-09-15', 'Exhaust leak', 'Replaced exhaust gasket', 75.00, 'Mike Tech', 0, 'Minor exhaust noise fixed', DATEADD(day, -191, GETUTCDATE()));

-- =============================================
-- CHECKLIST TEMPLATES (6 templates with items)
-- =============================================

-- Template 1: Daily 4-Wheeler Inspection
INSERT INTO ChecklistTemplates (Name, Type, VehicleType, IsActive, CreatedDate) VALUES
('Daily 4-Wheeler Inspection', 0, 0, 1, GETUTCDATE());

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, SortOrder, IsRequired) VALUES
(1, 'Check tire pressure and condition', 1, 1),
(1, 'Check brake function - front and rear', 2, 1),
(1, 'Check throttle operation and return', 3, 1),
(1, 'Inspect for fluid leaks (oil, coolant, fuel)', 4, 1),
(1, 'Check lights and signals', 5, 1),
(1, 'Verify kill switch operation', 6, 1),
(1, 'Check chain/belt tension', 7, 1),
(1, 'Inspect air filter condition', 8, 0),
(1, 'Check oil level', 9, 1),
(1, 'Test steering responsiveness', 10, 1);

-- Template 2: Daily Side-by-Side Inspection
INSERT INTO ChecklistTemplates (Name, Type, VehicleType, IsActive, CreatedDate) VALUES
('Daily Side-by-Side Inspection', 0, 1, 1, GETUTCDATE());

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, SortOrder, IsRequired) VALUES
(2, 'Check tire pressure and condition (all 4)', 1, 1),
(2, 'Check brake function', 2, 1),
(2, 'Test steering and play', 3, 1),
(2, 'Inspect for fluid leaks', 4, 1),
(2, 'Check all lights and signals', 5, 1),
(2, 'Verify seat belts function properly', 6, 1),
(2, 'Check doors and latches', 7, 1),
(2, 'Test horn', 8, 1),
(2, 'Check windshield condition', 9, 0),
(2, 'Inspect roll cage and safety equipment', 10, 1),
(2, 'Check oil and coolant levels', 11, 1),
(2, 'Test 4WD engagement (if applicable)', 12, 0);

-- Template 3: Pre-Rental 4-Wheeler Checklist
INSERT INTO ChecklistTemplates (Name, Type, VehicleType, IsActive, CreatedDate) VALUES
('Pre-Rental 4-Wheeler Checklist', 1, 0, 1, GETUTCDATE());

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, SortOrder, IsRequired) VALUES
(3, 'Verify customer ID and signed waiver', 1, 1),
(3, 'Record starting hours and mileage', 2, 1),
(3, 'Complete daily inspection checklist', 3, 1),
(3, 'Demonstrate controls to customer', 4, 1),
(3, 'Explain safety rules and trail boundaries', 5, 1),
(3, 'Provide safety gear (helmet, goggles)', 6, 1),
(3, 'Take pre-rental photos', 7, 1),
(3, 'Customer test ride in designated area', 8, 0),
(3, 'Provide emergency contact information', 9, 1),
(3, 'Record fuel level', 10, 1);

-- Template 4: Pre-Rental Side-by-Side Checklist
INSERT INTO ChecklistTemplates (Name, Type, VehicleType, IsActive, CreatedDate) VALUES
('Pre-Rental Side-by-Side Checklist', 1, 1, 1, GETUTCDATE());

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, SortOrder, IsRequired) VALUES
(4, 'Verify customer ID and signed waiver', 1, 1),
(4, 'Verify driver license', 2, 1),
(4, 'Record starting hours and mileage', 3, 1),
(4, 'Complete daily inspection checklist', 4, 1),
(4, 'Demonstrate all controls to customer', 5, 1),
(4, 'Explain safety rules and trail boundaries', 6, 1),
(4, 'Provide safety gear for all passengers', 7, 1),
(4, 'Take pre-rental photos (all angles)', 8, 1),
(4, 'Customer test drive in designated area', 9, 0),
(4, 'Provide emergency contact information', 10, 1),
(4, 'Record fuel level', 11, 1),
(4, 'Count and record number of passengers', 12, 1);

-- Template 5: Post-Rental 4-Wheeler Checklist
INSERT INTO ChecklistTemplates (Name, Type, VehicleType, IsActive, CreatedDate) VALUES
('Post-Rental 4-Wheeler Checklist', 2, 0, 1, GETUTCDATE());

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, SortOrder, IsRequired) VALUES
(5, 'Record ending hours and mileage', 1, 1),
(5, 'Inspect for new damage', 2, 1),
(5, 'Take post-rental photos', 3, 1),
(5, 'Check brake function', 4, 1),
(5, 'Inspect tires for damage', 5, 1),
(5, 'Check fluid levels', 6, 0),
(5, 'Record fuel level', 7, 1),
(5, 'Collect safety gear', 8, 1),
(5, 'Document any customer-reported issues', 9, 1),
(5, 'Schedule maintenance if hours threshold reached', 10, 0);

-- Template 6: Post-Rental Side-by-Side Checklist
INSERT INTO ChecklistTemplates (Name, Type, VehicleType, IsActive, CreatedDate) VALUES
('Post-Rental Side-by-Side Checklist', 2, 1, 1, GETUTCDATE());

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, SortOrder, IsRequired) VALUES
(6, 'Record ending hours and mileage', 1, 1),
(6, 'Inspect exterior for new damage', 2, 1),
(6, 'Take post-rental photos (all angles)', 3, 1),
(6, 'Check brake function', 4, 1),
(6, 'Inspect all tires for damage', 5, 1),
(6, 'Check doors and latches', 6, 1),
(6, 'Inspect windshield condition', 7, 0),
(6, 'Check fluid levels', 8, 0),
(6, 'Record fuel level', 9, 1),
(6, 'Collect all safety gear', 10, 1),
(6, 'Document any customer-reported issues', 11, 1),
(6, 'Check interior condition and cleanliness', 12, 0),
(6, 'Schedule maintenance if hours threshold reached', 13, 0);

-- =============================================
-- CHECKLIST ITEMS (sample items for active/completed rentals)
-- =============================================

-- Pre-rental checklist items for rental ID 1 (active - Mike Thompson)
INSERT INTO ChecklistItems (RentalId, VehicleId, Type, ItemText, IsCompleted, CompletedDate, CompletedBy, Notes, CreatedDate) VALUES
(1, 4, 1, 'Verify customer ID and signed waiver', 1, '2026-03-24 07:45:00', 'Staff Member', 'ID verified: DL#12345678', GETUTCDATE()),
(1, 4, 1, 'Record starting hours and mileage', 1, '2026-03-24 07:50:00', 'Staff Member', 'Hours: 335.0, Miles: 2080.0', GETUTCDATE()),
(1, 4, 1, 'Complete daily inspection checklist', 1, '2026-03-24 07:55:00', 'Staff Member', 'All systems go', GETUTCDATE()),
(1, 4, 1, 'Demonstrate controls to customer', 1, '2026-03-24 08:00:00', 'Staff Member', NULL, GETUTCDATE()),
(1, 4, 1, 'Explain safety rules and trail boundaries', 1, '2026-03-24 08:05:00', 'Staff Member', 'Customer acknowledged rules', GETUTCDATE()),
(1, 4, 1, 'Provide safety gear (helmet, goggles)', 1, '2026-03-24 08:10:00', 'Staff Member', 'Helmet #15, Goggles #8', GETUTCDATE()),
(1, 4, 1, 'Take pre-rental photos', 1, '2026-03-24 08:15:00', 'Staff Member', '6 photos taken', GETUTCDATE()),
(1, 4, 1, 'Provide emergency contact information', 1, '2026-03-24 08:20:00', 'Staff Member', 'Card provided', GETUTCDATE()),
(1, 4, 1, 'Record fuel level', 1, '2026-03-24 08:25:00', 'Staff Member', 'Full tank', GETUTCDATE());

-- Completed pre-rental items for past rental (ID 6 - John Smith)
INSERT INTO ChecklistItems (RentalId, VehicleId, Type, ItemText, IsCompleted, CompletedDate, CompletedBy, Notes, CreatedDate) VALUES
(6, 1, 1, 'Verify customer ID and signed waiver', 1, '2026-03-15 08:30:00', 'Staff Member', NULL, DATEADD(day, -10, GETUTCDATE())),
(6, 1, 1, 'Record starting hours and mileage', 1, '2026-03-15 08:35:00', 'Staff Member', 'Hours: 115.5, Miles: 780.0', DATEADD(day, -10, GETUTCDATE())),
(6, 1, 1, 'Complete daily inspection checklist', 1, '2026-03-15 08:40:00', 'Staff Member', NULL, DATEADD(day, -10, GETUTCDATE())),
(6, 1, 1, 'Demonstrate controls to customer', 1, '2026-03-15 08:45:00', 'Staff Member', NULL, DATEADD(day, -10, GETUTCDATE())),
(6, 1, 1, 'Provide safety gear (helmet, goggles)', 1, '2026-03-15 08:50:00', 'Staff Member', NULL, DATEADD(day, -10, GETUTCDATE())),
(6, 1, 1, 'Take pre-rental photos', 1, '2026-03-15 08:55:00', 'Staff Member', NULL, DATEADD(day, -10, GETUTCDATE())),
(6, 1, 1, 'Record fuel level', 1, '2026-03-15 09:00:00', 'Staff Member', 'Full', DATEADD(day, -10, GETUTCDATE()));

-- Post-rental items for past rental (ID 6 - John Smith)  
INSERT INTO ChecklistItems (RentalId, VehicleId, Type, ItemText, IsCompleted, CompletedDate, CompletedBy, Notes, CreatedDate) VALUES
(6, 1, 2, 'Record ending hours and mileage', 1, '2026-03-15 17:00:00', 'Staff Member', 'Hours: 125.5, Miles: 850.0', DATEADD(day, -10, GETUTCDATE())),
(6, 1, 2, 'Inspect for new damage', 1, '2026-03-15 17:05:00', 'Staff Member', 'No new damage', DATEADD(day, -10, GETUTCDATE())),
(6, 1, 2, 'Take post-rental photos', 1, '2026-03-15 17:10:00', 'Staff Member', '6 photos', DATEADD(day, -10, GETUTCDATE())),
(6, 1, 2, 'Check brake function', 1, '2026-03-15 17:15:00', 'Staff Member', 'Good', DATEADD(day, -10, GETUTCDATE())),
(6, 1, 2, 'Inspect tires for damage', 1, '2026-03-15 17:20:00', 'Staff Member', 'Good condition', DATEADD(day, -10, GETUTCDATE())),
(6, 1, 2, 'Record fuel level', 1, '2026-03-15 17:25:00', 'Staff Member', '3/4 tank', DATEADD(day, -10, GETUTCDATE())),
(6, 1, 2, 'Collect safety gear', 1, '2026-03-15 17:30:00', 'Staff Member', 'All returned', DATEADD(day, -10, GETUTCDATE())),
(6, 1, 2, 'Document any customer-reported issues', 1, '2026-03-15 17:35:00', 'Staff Member', 'No issues reported', DATEADD(day, -10, GETUTCDATE()));

-- Daily inspection items for vehicle (standalone inspection not tied to rental)
INSERT INTO ChecklistItems (RentalId, VehicleId, Type, ItemText, IsCompleted, CompletedDate, CompletedBy, Notes, CreatedDate) VALUES
(NULL, 1, 0, 'Check tire pressure and condition', 1, '2026-03-25 07:00:00', 'Morning Shift', '32 PSI all around', GETUTCDATE()),
(NULL, 1, 0, 'Check brake function - front and rear', 1, '2026-03-25 07:05:00', 'Morning Shift', 'Good', GETUTCDATE()),
(NULL, 1, 0, 'Check throttle operation and return', 1, '2026-03-25 07:10:00', 'Morning Shift', 'Smooth', GETUTCDATE()),
(NULL, 1, 0, 'Inspect for fluid leaks', 1, '2026-03-25 07:15:00', 'Morning Shift', 'None found', GETUTCDATE()),
(NULL, 1, 0, 'Check lights and signals', 1, '2026-03-25 07:20:00', 'Morning Shift', 'All working', GETUTCDATE()),
(NULL, 1, 0, 'Check oil level', 1, '2026-03-25 07:25:00', 'Morning Shift', 'Full', GETUTCDATE());

-- Daily inspection for another vehicle
INSERT INTO ChecklistItems (RentalId, VehicleId, Type, ItemText, IsCompleted, CompletedDate, CompletedBy, Notes, CreatedDate) VALUES
(NULL, 6, 0, 'Check tire pressure and condition (all 4)', 1, '2026-03-25 07:30:00', 'Morning Shift', '28 PSI all around', GETUTCDATE()),
(NULL, 6, 0, 'Check brake function', 1, '2026-03-25 07:35:00', 'Morning Shift', 'Good', GETUTCDATE()),
(NULL, 6, 0, 'Test steering and play', 1, '2026-03-25 07:40:00', 'Morning Shift', 'No play', GETUTCDATE()),
(NULL, 6, 0, 'Verify seat belts function properly', 1, '2026-03-25 07:45:00', 'Morning Shift', 'All working', GETUTCDATE()),
(NULL, 6, 0, 'Check doors and latches', 1, '2026-03-25 07:50:00', 'Morning Shift', 'Secure', GETUTCDATE()),
(NULL, 6, 0, 'Test horn', 1, '2026-03-25 07:55:00', 'Morning Shift', 'Working', GETUTCDATE());

PRINT '✅ Seed data inserted successfully!';
PRINT '';
PRINT 'Summary:';
PRINT '  - 10 Vehicles (5 ATVs, 5 Side-by-Sides)';
PRINT '  - 15 Rentals (1 active, 4 upcoming, 10 completed)';
PRINT '  - 20 Maintenance Records';
PRINT '  - 12 Repair Records';
PRINT '  - 6 Checklist Templates with 67 Template Items';
PRINT '  - 37 Checklist Items';

