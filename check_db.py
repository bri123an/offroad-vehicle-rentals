import pymssql

conn = pymssql.connect(
    server='offroad-vehicle-sql.database.windows.net',
    user='sqladmin',
    password='OffroadRental2024!',
    database='OffroadVehicleRentals'
)
cursor = conn.cursor()

# Check tables
cursor.execute("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'")
tables = cursor.fetchall()
print("Tables in database:")
for table in tables:
    print(f"  - {table[0]}")

# Check counts
cursor.execute('SELECT COUNT(*) FROM Vehicles')
print(f"\nVehicles: {cursor.fetchone()[0]}")

cursor.execute('SELECT COUNT(*) FROM ChecklistTemplates')
print(f"ChecklistTemplates: {cursor.fetchone()[0]}")

cursor.execute('SELECT COUNT(*) FROM ChecklistTemplateItems')
print(f"ChecklistTemplateItems: {cursor.fetchone()[0]}")

conn.close()
