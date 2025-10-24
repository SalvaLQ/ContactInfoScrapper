using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace AllContactInfoScrapper.Infraestructure.Files
{
    public class FileExporter
    {
        public string Destination { get; set; }

        public FileExporter(string Destination)
        {
            this.Destination = Destination;
        }
        public void ExpExportFileCSV<T>(List<T> Contacts)
        {
            using (var writer = new StreamWriter(Destination))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(Contacts);
            }
        }
        public void ExpExportFileExcel(List<Domain.ContactInfo> Contacts, List<string> Fields)
        {
            ICell cell;
            IRow row;
            int rowNum = 0;
            int cellNum = 0;
            FileStream stream;
            IWorkbook wb = new XSSFWorkbook();


            using (stream = new FileStream(Destination, FileMode.Create, FileAccess.Write))
            {
                ISheet sheet = wb.CreateSheet("AllContactInfoScrapper");
                ICreationHelper cH = wb.GetCreationHelper();
                row = sheet.CreateRow(0);

                foreach (string c in Fields)
                {
                    cell = row.CreateCell(cellNum++);
                    cell.SetCellValue(c);
                }

                foreach (var item in Contacts)
                {
                    cellNum = 0;
                    row = sheet.CreateRow(++rowNum);
                    cell = row.CreateCell(cellNum++, CellType.String);
                    cell.SetCellValue(item.Title);

                    cell = row.CreateCell(cellNum++, CellType.String);
                    cell.SetCellValue(item.Email);

                    cell = row.CreateCell(cellNum++, CellType.String);
                    cell.SetCellValue(item.Phone);

                    cell = row.CreateCell(cellNum++, CellType.String);
                    cell.SetCellValue(item.Description);

                    cell = row.CreateCell(cellNum++, CellType.String);
                    cell.SetCellValue(item.Url);                    
                }
                wb.Write(stream, false);
            }
        }
    }
}
