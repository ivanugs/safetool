using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using safetool.Models;
using safetool.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using System.Drawing;

namespace safetool.Controllers
{
    public class ExportController : Controller
    {
        private readonly SafetoolContext _context;
        public ExportController(SafetoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            var data = await _context.FormSubmissions
                .Include(f => f.Device)
                .Include(f => f.Device.Area.Location)
                .ToListAsync();

            // Configurar EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {

                var worksheet = package.Workbook.Worksheets.Add("Registros");

                worksheet.Cells.Style.Font.Size = 12;

                // Crear encabezados
                worksheet.Cells[1, 1].Value = "Usuario";
                worksheet.Cells[1, 2].Value = "Nombre del Empleado";
                worksheet.Cells[1, 3].Value = "Localidad";
                worksheet.Cells[1, 4].Value = "Áreas";
                worksheet.Cells[1, 5].Value = "Modelo del equipo";
                worksheet.Cells[1, 6].Value = "Fecha de registro";

                // Aplicar negritas y color de fondo a los encabezados
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Size = 13;
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Orange);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Rellenar filas con los datos
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.EmployeeUID;
                    worksheet.Cells[row, 2].Value = item.EmployeeName;
                    worksheet.Cells[row, 3].Value = item.Device.Area.Location.Name;
                    worksheet.Cells[row, 4].Value = item.Device.Area.Name;
                    worksheet.Cells[row, 5].Value = item.Device.Model;
                    worksheet.Cells[row, 6].Value = item.CreatedAt.ToString("dd-MM-yyyyy HH:mm"); // Formato de fecha

                    row++;
                }

                // Ajustar el tamaño de las columnas
                worksheet.Cells.AutoFitColumns();

                // Guardar el archivo en un stream
                var stream = new MemoryStream(package.GetAsByteArray());

                // Retornar el archivo como descarga
                string fileName = "Registros.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(stream, contentType, fileName);
            }
        }
    }
}
