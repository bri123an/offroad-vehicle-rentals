#!/usr/bin/env python3
"""
Database initialization script for Azure SQL Database
Requires: pip install pyodbc
"""

import pyodbc
import sys

# Connection parameters
server = 'offroad-vehicle-sql.database.windows.net'
database = 'OffroadVehicleRentals'
username = 'sqladmin'
password = 'OffRoad2024!Secure'
driver = '{ODBC Driver 18 for SQL Server}'

# Read SQL script
sql_file = 'src/OffroadVehicleRentals.Api/Data/DbInitializer.sql'

try:
    # Read the SQL file
    with open(sql_file, 'r') as file:
        sql_script = file.read()

    # Split into individual statements (by GO or semicolon)
    statements = []
    current_statement = []

    for line in sql_script.split('\n'):
        if line.strip().upper() == 'GO' or line.strip() == '':
            if current_statement:
                statements.append('\n'.join(current_statement))
                current_statement = []
        else:
            current_statement.append(line)

    if current_statement:
        statements.append('\n'.join(current_statement))

    # Connect to database
    print(f"Connecting to {server}...")
    connection_string = f'DRIVER={driver};SERVER={server};DATABASE={database};UID={username};PWD={password};Encrypt=yes;TrustServerCertificate=no;Connection Timeout=30;'

    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()

    print(f"Connected successfully!")
    print(f"Executing SQL script with {len(statements)} statements...")

    # Execute each statement
    for i, statement in enumerate(statements, 1):
        if statement.strip():
            try:
                print(f"Executing statement {i}...", end=' ')
                cursor.execute(statement)
                conn.commit()
                print("✓")
            except Exception as e:
                print(f"✗ Error: {e}")
                # Continue with other statements even if one fails

    print("\n✅ Database initialization completed!")
    print("\nYou can now access your application at:")
    print("  - Web: https://offroad-vehicle-web.azurewebsites.net")
    print("  - API: https://offroad-vehicle-api.azurewebsites.net/api/vehicles")

    cursor.close()
    conn.close()

except FileNotFoundError:
    print(f"❌ Error: SQL file not found: {sql_file}")
    sys.exit(1)
except ImportError:
    print("❌ Error: pyodbc not installed")
    print("Install it with: pip install pyodbc")
    print("\nAlternatively, use Azure Portal Query Editor:")
    print("  1. Go to: https://portal.azure.com")
    print("  2. Navigate to: SQL databases > OffroadVehicleRentals")
    print("  3. Click 'Query editor (preview)'")
    print("  4. Run the SQL script manually")
    sys.exit(1)
except Exception as e:
    print(f"❌ Error: {e}")
    print("\nPlease initialize the database manually using Azure Portal:")
    print("  1. Go to: https://portal.azure.com")
    print("  2. Navigate to: SQL databases > OffroadVehicleRentals")
    print("  3. Click 'Query editor (preview)'")
    print("  4. Login with: sqladmin / OffRoad2024!Secure")
    print("  5. Run the SQL script from: src/OffroadVehicleRentals.Api/Data/DbInitializer.sql")
    sys.exit(1)
