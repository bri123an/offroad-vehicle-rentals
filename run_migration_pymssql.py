#!/usr/bin/env python3
"""
Database migration script for Azure SQL Database using pymssql
"""

import pymssql
import sys

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
    print(f"\nConnecting to {server}...")

    # Connect to database
    conn = pymssql.connect(
        server=server,
        user=username,
        password=password,
        database=database,
        port=1433,
        as_dict=False
    )

    cursor = conn.cursor()
    print("✅ Connected successfully!")

    # Split SQL into individual statements
    # Remove comments and split by semicolons
    statements = []
    current_statement = []

    for line in sql_script.split('\n'):
        stripped = line.strip()

        # Skip comment lines
        if stripped.startswith('--') or not stripped:
            continue

        current_statement.append(line)

        # Check if statement ends with semicolon
        if stripped.endswith(';'):
            stmt = '\n'.join(current_statement)
            statements.append(stmt)
            current_statement = []

    # Add any remaining statement
    if current_statement:
        stmt = '\n'.join(current_statement)
        if stmt.strip():
            statements.append(stmt)

    print(f"\nExecuting {len(statements)} SQL statements...\n")

    executed = 0
    errors = 0

    for i, stmt in enumerate(statements, 1):
        if stmt.strip():
            try:
                # Execute statement
                cursor.execute(stmt)
                conn.commit()
                executed += 1

                # Print progress with statement preview
                preview = stmt.strip().split('\n')[0][:60]
                print(f"✓ [{i}/{len(statements)}] {preview}...")

            except Exception as e:
                errors += 1
                error_msg = str(e)[:100]
                print(f"✗ [{i}/{len(statements)}] Error: {error_msg}")
                # Continue with next statement

    print(f"\n{'='*60}")
    print(f"✅ Migration completed!")
    print(f"   Executed: {executed}/{len(statements)}")
    if errors > 0:
        print(f"   ⚠️  Errors: {errors}")
    print(f"{'='*60}")

    # Verify tables were created
    print("\n🔍 Verifying database objects...")
    cursor.execute("""
        SELECT TABLE_NAME
        FROM INFORMATION_SCHEMA.TABLES
        WHERE TABLE_TYPE='BASE TABLE'
        ORDER BY TABLE_NAME
    """)
    tables = cursor.fetchall()

    print(f"\n📊 Tables created ({len(tables)}):")
    for table in tables:
        print(f"  ✓ {table[0]}")

    # Check checklist templates
    cursor.execute("SELECT COUNT(*) FROM ChecklistTemplates")
    template_count = cursor.fetchone()[0]
    print(f"\n📋 Checklist templates: {template_count}")

    # Check checklist items
    cursor.execute("SELECT COUNT(*) FROM ChecklistTemplateItems")
    item_count = cursor.fetchone()[0]
    print(f"📝 Checklist items: {item_count}")

    cursor.close()
    conn.close()

    print("\n" + "="*60)
    print("🎉 Database initialized successfully!")
    print("="*60)
    print("\n🌐 Your application is ready! Access it at:")
    print(f"   Web: https://offroad-vehicle-web.azurewebsites.net")
    print(f"   API: https://offroad-vehicle-api.azurewebsites.net/api/vehicles")
    print("\n💡 Next steps:")
    print("   1. Visit the web app")
    print("   2. Add your first vehicle")
    print("   3. Create a rental")
    print("   4. Try the daily inspection checklist")

except FileNotFoundError:
    print(f"❌ Error: SQL file not found: {sql_file}")
    print("Make sure you're running this from the project root directory.")
    sys.exit(1)
except pymssql.Error as e:
    print(f"❌ Database Error: {e}")
    print("\nConnection details:")
    print(f"  Server: {server}")
    print(f"  Database: {database}")
    print(f"  Username: {username}")
    sys.exit(1)
except Exception as e:
    print(f"❌ Error: {e}")
    import traceback
    traceback.print_exc()
    sys.exit(1)
