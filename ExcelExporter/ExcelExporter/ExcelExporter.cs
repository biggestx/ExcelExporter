using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelExporter
{
    class ExcelExporter
    {

        private const string DATA_CLASS = @"
        class {0}Data
        {{
            {1}
        }}
        ";


        private const string CS_FILE = @"
        using System;

        namespace Table
        {{
            class {0}Table // name
            {{
                {1} // TableData

                {2} // Container
            }}
        }}
        ";


        public void Export()
        {
            string path = @"D:\git repositories\ExcelExporter\ExcelExporter\Test.xlsx";

            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            Excel.Application excel = new Excel.Application();
            var workbook = excel.Workbooks.Open(path);
            if (workbook == null)
                return;

            Worksheet worksheet = null;

            try
            {

                // one based
                worksheet = workbook.Sheets[1] as Excel.Worksheet;
                Excel.Range v = worksheet.Cells[1, 1];

                var rowCount = worksheet.UsedRange.Rows.Count;
                var colCount = worksheet.UsedRange.Columns.Count;

                string fields = "";

                for (int i = 1; i <= colCount; ++i)
                {
                    var field = worksheet.Cells[1, i].Value;
                    var @type = worksheet.Cells[2, i].Value;

                    fields += "public " + @type + " " + field + "; ";
                }


                var dataClass = string.Format(
                    DATA_CLASS,
                    fileName,
                    fields
                    );


                for (int i = 1; i <= rowCount; ++i)
                {
                    for (int j = 1; j <= colCount; ++j)
                    {
                        Excel.Range cell = worksheet.Cells[i, j];
                        Console.WriteLine(cell.Value);
                    }
                }

                var container = "List<int> Container;\n";


                string file = string.Format(
                    CS_FILE,
                    fileName,
                    dataClass,
                    container
                    );

                Console.WriteLine(file);

                System.IO.File.WriteAllText(@"D:\git repositories\ExcelExporter\ExcelExporter\Test.cs", file);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (worksheet != null)
                    Marshal.ReleaseComObject(worksheet);

                if (workbook != null)
                {
                    workbook.Close();
                    Marshal.ReleaseComObject(workbook);
                }

                excel.Quit();
                Marshal.ReleaseComObject(excel);
            }


            

        }

        


    }
}


namespace Table
{
    class Name // {0} file name
    {

    }
}
