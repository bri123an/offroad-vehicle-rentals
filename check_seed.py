#!/usr/bin/env python3
import pymssql

conn = pymssql.connect(
    server='offroad-vehicle-sql.database.windows.net',
    user='sqladmin',
    password='OffRoad2024!Secure',
    database='OffroadVehicleRentals',
    tds_version='7.4'
)
cursor = conn.cursor()

print('=' * 60)
print('Database Contents Summary')
print('=' * 60)
print()

tables = [
    ('Vehicles', 'SELECT COUNT(*) FROM Vehicles'),
    ('Rentals', 'SELECT COUNT(*) FROM Rentals'),
    ('MaintenanceRecords', 'SELECT COUNT(*) FROM MaintenanceRecords'),
    ('RepairRecords', 'SELECT COUNT(*) FROM RepairRecords'),
    ('ChecklistTemplates', 'SELECT COUNT(*) FROM ChecklistTemplates'),
    ('ChecklistTemplateItems', 'SELECT COUNT(*) FROM ChecklistTemplateItems'),
    ('ChecklistItems', 'SELECT COUNT(*) FROM ChecklistItems'),
]

for name, query in tables:
    cursor.execute(query)
    count = cursor.fetchone()[0]
    print(f'{name}: {count} records')

print()
print('-' * 60)
print('Sample Vehicles:')
cursor.execute('SELECT TOP 5 VehicleNumber, Make, Model, Year FROM Vehicles')
for row in cursor.fetchall():
    print(f'  {row[0]}: {row[1]} {row[2]} ({row[3]})')

print()
print('Sample Rentals:')
cursor.execute('SELECT TOP 5 CustomerName, StartDate, EndDate FROM Rentals')
for row in cursor.fetchall():
    print(f'  {row[0]}: {row[1]} - {row[2]}')

cursor.close()
conn.close()

