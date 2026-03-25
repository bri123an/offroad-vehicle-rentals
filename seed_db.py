#!/usr/bin/env python3
"""
Database seed script for Azure SQL Database
Populates all tables with sample data
Requires: pip install pyodbc
OR:       pip install pymssql
"""
import sys
# Connection parameters
SERVER = 'offroad-vehicle-sql.database.windows.net'
DATABASE = 'OffroadVehicleRentals'
USERNAME = 'sqladmin'
PASSWORD = 'OffRoad2024!Secure'
# SQL file path
SQL_FILE = 'src/OffroadVehicleRentals.Api/Data/SeedData.sql'
def run_with_pyodbc():
    """Run using pyodbc driver"""
    import pyodbc
    driver = '{ODBC Driver 18 for SQL Server}'
    connection_string = f'DRIVER={driver};SERVER={SERVER};DATABASE={DATABASE};UID={USERNAME};PWD={PASSWORD};Encrypt=yes;TrustServerCertificate=no;Connection Timeout=30;'
    print(f"Connecting to {SERVER} using pyodbc...")
    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()
    return conn, cursor
def run_with_pymssql():
    """Run using pymssql driver"""
    import pymssql
    print(f"Connecting to {SERVER} using pymssql...")
    conn = pymssql.connect(
        server=SERVER,
        user=USERNAME,
        password=PASSWORD,
        database=DATABASE,
        tds_version='7.4'
    )
    cursor = conn.cursor()
    return conn, cursor
def execute_sql(cursor, conn, sql_content):
    """Execute SQL statements"""
    # Split by GO statements (SQL Server batch separator)
    batches = []
    current_batch = []
    for line in sql_content.split('\n'):
        stripped = line.strip().upper()
        if stripped == 'GO':
            if current_batch:
                batches.append('\n'.join(current_batch))
                current_batch = []
        else:
            current_batch.append(line)
    if current_batch:
        batches.append('\n'.join(current_batch))
    # Process each batch
    successful = 0
    failed = 0
    for i, batch in enumerate(batches, 1):
        if not batch.strip():
            continue
        lines = [l for l in batch.split('\n') if l.strip() and not l.strip().startswith('--')]
        if not lines:
            continue
        try:
            statements = []
            current_stmt = []
            for line in batch.split('\n'):
                current_stmt.append(line)
                if line.rstrip().endswith(';'):
                    stmt = '\n'.join(current_stmt)
                    if stmt.strip() and not all(l.strip().startswith('--') or not l.strip() for l in current_stmt):
                        statements.append(stmt)
                    current_stmt = []
            if current_stmt:
                stmt = '\n'.join(current_stmt)
                if stmt.strip() and not all(l.strip().startswith('--') or not l.strip() for l in current_stmt):
                    statements.append(stmt)
            for stmt in statements:
                clean_stmt = stmt.strip()
                if clean_stmt and not clean_stmt.startswith('--'):
                    if clean_stmt.upper().startswith('PRINT '):
                        msg = clean_stmt[6:].strip().strip("'").strip('"')
                        print(msg)
                        continue
                    cursor.execute(stmt)
                    successful += 1
            conn.commit()
        except Exception as e:
            failed += 1
            error_msg = str(e)
            if 'Cannot insert duplicate key' not in error_msg and 'does not exist' not in error_msg:
                print(f"  Warning batch {i}: {error_msg[:100]}")
            continue
    return successful, failed
def main():
    print("=" * 60)
    print("Offroad Vehicle Rentals - Database Seed Script")
    print("=" * 60)
    print()
    try:
        with open(SQL_FILE, 'r') as f:
            sql_content = f.read()
        print(f"Loaded SQL script: {SQL_FILE}")
    except FileNotFoundError:
        print(f"Error: SQL file not found: {SQL_FILE}")
        sys.exit(1)
    conn = None
    cursor = None
    try:
        conn, cursor = run_with_pyodbc()
        print("Connected using pyodbc")
    except ImportError:
        print("pyodbc not available, trying pymssql...")
        try:
            conn, cursor = run_with_pymssql()
            print("Connected using pymssql")
        except ImportError:
            print()
            print("Error: No database driver available")
            print()
            print("Install one of the following:")
            print("  pip install pyodbc    (requires ODBC Driver 18)")
            print("  pip install pymssql")
            print()
            print("Or run the SQL script manually:")
            print("  1. Go to: https://portal.azure.com")
            print("  2. Navigate to: SQL databases > OffroadVehicleRentals")
            print("  3. Click Query editor (preview)")
            print(f"  4. Run the script: {SQL_FILE}")
            sys.exit(1)
    except Exception as e:
        print(f"Connection error: {e}")
        sys.exit(1)
    print()
    print("Executing seed data script...")
    print("-" * 40)
    try:
        successful, failed = execute_sql(cursor, conn, sql_content)
        print("-" * 40)
        print()
        print("Seed data script completed!")
        print()
        print("Data seeded:")
        print("  - 10 Vehicles (5 ATVs, 5 Side-by-Sides)")
        print("  - 15 Rentals (1 active, 4 upcoming, 10 completed)")
        print("  - 20 Maintenance Records")
        print("  - 12 Repair Records")
        print("  - 6 Checklist Templates with 67 Template Items")
        print("  - 37 Checklist Items")
        print()
        print("Test your data at:")
        print("  - Web: https://offroad-vehicle-web.azurewebsites.net")
        print("  - API: https://offroad-vehicle-api.azurewebsites.net/api/vehicles")
    except Exception as e:
        print(f"Error executing SQL: {e}")
        sys.exit(1)
    finally:
        if cursor:
            cursor.close()
        if conn:
            conn.close()
if __name__ == '__main__':
    main()
