-- Azure SQL Database Schema for Offroad Vehicle Rentals
-- Run this script on your Azure SQL Database

-- Create Vehicles table
CREATE TABLE Vehicles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VehicleNumber NVARCHAR(50) NOT NULL UNIQUE,
    Make NVARCHAR(100) NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    Year INT NOT NULL,
    Type INT NOT NULL,
    Status INT NOT NULL,
    CurrentHours DECIMAL(10,2) NOT NULL DEFAULT 0,
    CurrentMileage DECIMAL(10,2) NOT NULL DEFAULT 0,
    LastMaintenanceDate DATETIME2 NULL,
    NextMaintenanceHours DECIMAL(10,2) NULL,
    NextMaintenanceMileage DECIMAL(10,2) NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL
);

-- Create Rentals table
CREATE TABLE Rentals (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VehicleId INT NOT NULL,
    CustomerName NVARCHAR(200) NOT NULL,
    CustomerPhone NVARCHAR(20) NULL,
    CustomerEmail NVARCHAR(200) NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    StartHours DECIMAL(10,2) NOT NULL,
    EndHours DECIMAL(10,2) NULL,
    StartMileage DECIMAL(10,2) NOT NULL,
    EndMileage DECIMAL(10,2) NULL,
    PreRentalChecklistCompleted BIT NOT NULL DEFAULT 0,
    PostRentalChecklistCompleted BIT NOT NULL DEFAULT 0,
    Notes NVARCHAR(MAX) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT FK_Rentals_Vehicles FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id)
);

-- Create MaintenanceRecords table
CREATE TABLE MaintenanceRecords (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VehicleId INT NOT NULL,
    MaintenanceDate DATETIME2 NOT NULL,
    MaintenanceType NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    HoursAtMaintenance DECIMAL(10,2) NOT NULL,
    MileageAtMaintenance DECIMAL(10,2) NOT NULL,
    Cost DECIMAL(10,2) NULL,
    PerformedBy NVARCHAR(200) NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT FK_MaintenanceRecords_Vehicles FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id) ON DELETE CASCADE
);

-- Create RepairRecords table
CREATE TABLE RepairRecords (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VehicleId INT NOT NULL,
    RepairDate DATETIME2 NOT NULL,
    IssueDescription NVARCHAR(MAX) NOT NULL,
    RepairDescription NVARCHAR(MAX) NOT NULL,
    Cost DECIMAL(10,2) NULL,
    PerformedBy NVARCHAR(200) NULL,
    IsWarrantyClaim BIT NOT NULL DEFAULT 0,
    Notes NVARCHAR(MAX) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,
    CONSTRAINT FK_RepairRecords_Vehicles FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id) ON DELETE CASCADE
);

-- Create ChecklistTemplates table
CREATE TABLE ChecklistTemplates (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Type INT NOT NULL,
    VehicleType INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL
);

-- Create ChecklistTemplateItems table
CREATE TABLE ChecklistTemplateItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ChecklistTemplateId INT NOT NULL,
    ItemText NVARCHAR(500) NOT NULL,
    SortOrder INT NOT NULL,
    IsRequired BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_ChecklistTemplateItems_Templates FOREIGN KEY (ChecklistTemplateId) REFERENCES ChecklistTemplates(Id) ON DELETE CASCADE
);

-- Create ChecklistItems table
CREATE TABLE ChecklistItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RentalId INT NULL,
    VehicleId INT NULL,
    Type INT NOT NULL,
    ItemText NVARCHAR(500) NOT NULL,
    IsCompleted BIT NOT NULL DEFAULT 0,
    CompletedDate DATETIME2 NULL,
    CompletedBy NVARCHAR(200) NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_ChecklistItems_Rentals FOREIGN KEY (RentalId) REFERENCES Rentals(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ChecklistItems_Vehicles FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id)
);

-- Create indexes for better performance
CREATE INDEX IX_Rentals_VehicleId ON Rentals(VehicleId);
CREATE INDEX IX_Rentals_StartDate ON Rentals(StartDate);
CREATE INDEX IX_MaintenanceRecords_VehicleId ON MaintenanceRecords(VehicleId);
CREATE INDEX IX_RepairRecords_VehicleId ON RepairRecords(VehicleId);
CREATE INDEX IX_ChecklistItems_RentalId ON ChecklistItems(RentalId);
CREATE INDEX IX_ChecklistItems_VehicleId ON ChecklistItems(VehicleId);

-- Insert sample checklist templates
INSERT INTO ChecklistTemplates (Name, Type, VehicleType, IsActive) VALUES
('Daily 4-Wheeler Inspection', 0, 0, 1),
('Daily Side-by-Side Inspection', 0, 1, 1),
('Pre-Rental 4-Wheeler Checklist', 1, 0, 1),
('Pre-Rental Side-by-Side Checklist', 1, 1, 1),
('Post-Rental 4-Wheeler Checklist', 2, 0, 1),
('Post-Rental Side-by-Side Checklist', 2, 1, 1);

-- Insert sample checklist items for 4-Wheeler Daily Inspection
INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, SortOrder, IsRequired) VALUES
(1, 'Check tire pressure and condition', 1, 1),
(1, 'Check brake function', 2, 1),
(1, 'Check throttle operation', 3, 1),
(1, 'Inspect for fluid leaks', 4, 1),
(1, 'Check lights and signals', 5, 1),
(1, 'Verify kill switch operation', 6, 1),
(1, 'Check chain/belt tension', 7, 1),
(1, 'Inspect air filter', 8, 0);

-- Insert sample checklist items for Side-by-Side Daily Inspection
INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, SortOrder, IsRequired) VALUES
(2, 'Check tire pressure and condition', 1, 1),
(2, 'Check brake function', 2, 1),
(2, 'Test steering', 3, 1),
(2, 'Inspect for fluid leaks', 4, 1),
(2, 'Check lights and signals', 5, 1),
(2, 'Verify seat belts function', 6, 1),
(2, 'Check doors and latches', 7, 1),
(2, 'Test horn', 8, 1),
(2, 'Check windshield condition', 9, 0);
