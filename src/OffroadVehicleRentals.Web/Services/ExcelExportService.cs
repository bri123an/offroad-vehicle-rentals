using ClosedXML.Excel;
using OffroadVehicleRentals.Shared.Models;

namespace OffroadVehicleRentals.Web.Services;

public class ExcelExportService
{
    public byte[] ExportMaintenanceRecords(List<MaintenanceRecord> records)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Maintenance Records");

        // Headers
        var headers = new[] { "Date", "Vehicle", "Type", "Description", "Hours", "Mileage", "Cost", "Performed By", "Notes" };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        // Data rows
        for (int row = 0; row < records.Count; row++)
        {
            var record = records[row];
            int r = row + 2;

            worksheet.Cell(r, 1).Value = record.MaintenanceDate.ToLocalTime().ToString("d");
            worksheet.Cell(r, 2).Value = record.Vehicle?.VehicleNumber ?? "";
            worksheet.Cell(r, 3).Value = record.MaintenanceType;
            worksheet.Cell(r, 4).Value = record.Description ?? "";
            worksheet.Cell(r, 5).Value = (double)record.HoursAtMaintenance;
            worksheet.Cell(r, 5).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(r, 6).Value = (double)record.MileageAtMaintenance;
            worksheet.Cell(r, 6).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(r, 7).Value = record.Cost.HasValue ? (double)record.Cost.Value : 0;
            worksheet.Cell(r, 7).Style.NumberFormat.Format = "$#,##0.00";
            worksheet.Cell(r, 8).Value = record.PerformedBy ?? "";
            worksheet.Cell(r, 9).Value = record.Notes ?? "";
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportRepairRecords(List<RepairRecord> records)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Repair Records");

        // Headers
        var headers = new[] { "Date", "Vehicle", "Issue", "Repair", "Cost", "Warranty", "Performed By", "Notes" };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        // Data rows
        for (int row = 0; row < records.Count; row++)
        {
            var record = records[row];
            int r = row + 2;

            worksheet.Cell(r, 1).Value = record.RepairDate.ToLocalTime().ToString("d");
            worksheet.Cell(r, 2).Value = record.Vehicle?.VehicleNumber ?? "";
            worksheet.Cell(r, 3).Value = record.IssueDescription;
            worksheet.Cell(r, 4).Value = record.RepairDescription;
            worksheet.Cell(r, 5).Value = record.Cost.HasValue ? (double)record.Cost.Value : 0;
            worksheet.Cell(r, 5).Style.NumberFormat.Format = "$#,##0.00";
            worksheet.Cell(r, 6).Value = record.IsWarrantyClaim ? "Yes" : "No";
            worksheet.Cell(r, 7).Value = record.PerformedBy ?? "";
            worksheet.Cell(r, 8).Value = record.Notes ?? "";
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

