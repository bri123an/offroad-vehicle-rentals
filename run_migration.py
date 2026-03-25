#!/usr/bin/env python3
"""
Database migration script for Azure SQL Database
"""

import urllib.parse

# Connection parameters
server = 'offroad-vehicle-sql.database.windows.net'
database = 'OffroadVehicleRentals'
username = 'sqladmin'
password = 'OffRoad2024!Secure'

# Read SQL script
sql_file = 'src/OffroadVehicleRentals.Api/Data/DbInitializer.sql'

print(f"Reading SQL script from: {sql_file}")

try:
    with open(sql_file, 'r') as file:
        sql_script = file.read()

    print(f"SQL script loaded ({len(sql_script)} characters)")

    # Try importing pyodbc
    try:
        import pyodbc

        print(f"\nConnecting to {server}...")

        # Build connection string
        params = urllib.parse.quote_plus(
            f'DRIVER={{ODBC Driver 18 for SQL Server}};'
            f'SERVER={server};'
            f'DATABASE={database};'
            f'UID={username};'
            f'PWD={password};'
            f'Encrypt=yes;'
            f'TrustServerCertificate=no;'
            f'Connection Timeout=30;'
        )

        conn_str = f'mssql+pyodbc:///?odbc_connect={params}'

        # Simple connection string
        connection_string = (
            f'DRIVER={{ODBC Driver 18 for SQL Server}};'
            f'SERVER={server};'
            f'DATABASE={database};'
            f'UID={username};'
            f'PWD={password};'
            f'Encrypt=yes;'
            f'TrustServerCertificate=no;'
            f'Connection Timeout=30;'
        )

        conn = pyodbc.connect(connection_string)
        cursor = conn.cursor()

        print("✅ Connected successfully!")

        # Split SQL into batches (by semicolon for simple approach)
        # Better: split by GO or handle compound statements
        statements = []
        current = []

        for line in sql_script.split('\n'):
            line = line.strip()
            if line.upper().startswith('--') or not line:
                continue
            current.append(line)
            if line.endswith(';'):
                statements.append(' '.join(current))
                current = []

        if current:
            statements.append(' '.join(current))

        print(f"\nExecuting {len(statements)} SQL statements...\n")

        executed = 0
        errors = 0

        for i, stmt in enumerate(statements, 1):
            if stmt.strip():
                try:
                    cursor.execute(stmt)
                    conn.commit()
                    executed += 1
                    print(f"✓ Statement {i}/{len(statements)}")
                except Exception as e:
                    errors += 1
                    print(f"✗ Statement {i}/{len(statements)}: {str(e)[:100]}")

        print(f"\n{'='*50}")
        print(f"✅ Migration completed!")
        print(f"   Executed: {executed}")
        print(f"   Errors: {errors}")
        print(f"{'='*50}")

        # Verify tables were created
        cursor.execute("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' ORDER BY TABLE_NAME")
        tables = cursor.fetchall()

        print(f"\nTables created ({len(tables)}):")
        for table in tables:
            print(f"  - {table[0]}")

        cursor.close()
        conn.close()

        print("\n🎉 Database initialized successfully!")
        print("\nYou can now access your application at:")
        print("  🌐 Web: https://offroad-vehicle-web.azurewebsites.net")
        print("  🔗 API: https://offroad-vehicle-api.azurewebsites.net/api/vehicles")

    except ImportError:
        print("\n⚠️  pyodbc not installed")
        print("\nInstalling pyodbc...")
        import subprocess
        subprocess.run(['pip3', 'install', '--user', 'pyodbc'], check=True)
        print("\n✅ pyodbc installed! Please run this script again.")

except FileNotFoundError:
    print(f"❌ Error: SQL file not found: {sql_file}")
    print("Make sure you're running this from the project root directory.")
except Exception as e:
    print(f"❌ Error: {e}")
    import traceback
    traceback.print_exc()
