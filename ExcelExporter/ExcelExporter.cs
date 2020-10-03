using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelExporter
{
    class ExcelExporter
    {

        private enum EIDType
        {
            None,
            Int,
            String,
        }

        private const string DATA_CLASS_NAME = "{0}Data";

        private const string DATA_CLASS = @"
        class {0}
        {{
            {1}
        }}
        ";


        private const string CS_FILE = @"
        using System;
        using System.Collections.Generic;
        namespace Table
        {{
            // name
            class {0}Table 
            {{
                // TableData
                {1} 
                
                // Container
                {2} 
            }}
        }}
        ";


        private const string DESERIALIZE_FUNC = @"
        private void Deserialize()
        {{
            



        }}
        ";


        


        public void Export()
        {
            string path = @"D:\git repository\ExcelExporter\ExcelExporter\Test.xlsx";

            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            Excel.Application excel = new Excel.Application();
            var workbook = excel.Workbooks.Open(path);
            if (workbook == null)
                return;

            Excel.Worksheet worksheet = null;

            try
            {

                // one based
                worksheet = workbook.Sheets[1] as Excel.Worksheet;
                Excel.Range v = worksheet.Cells[1, 1];

                var rowCount = worksheet.UsedRange.Rows.Count;
                var colCount = worksheet.UsedRange.Columns.Count;

                string fields = "";

                List<Type> fieldTypes = new List<Type>();
                List<string> fieldNames = new List<string>();

                

                for (int i = 1; i <= colCount; ++i)
                {
                    var fieldName = worksheet.Cells[1, i].Value;
                    var @type = worksheet.Cells[2, i].Value;

                    fields += "public " + @type + " " + fieldName + ";\n";
                    fieldNames.Add(fieldName);
                    switch (type)
                    {
                        case "string":
                            fieldTypes.Add(typeof(string));
                            break;
                        case "int":
                            fieldTypes.Add(typeof(string));
                            break;
                    }
                }           
                

                string dataClassName = string.Format(DATA_CLASS_NAME, fileName);

                var dataClass = string.Format(
                    DATA_CLASS,
                    dataClassName,
                    fields
                    );


                // data class definition 
                Dictionary<dynamic,dynamic> values = new Dictionary<dynamic,dynamic>();

                for (int i = 3; i <= rowCount; ++i)
                {
                    dynamic b = new ExpandoObject();

                    for (int j = 1; j <= colCount; ++j)
                    {
                        Excel.Range cell = worksheet.Cells[i, j];
                        Console.WriteLine(cell.Value);

                        ((IDictionary<string, Object>)b).Add(fieldNames[j-1].ToString(), cell.Value);
                    }
                    values.Add(((IDictionary<string, Object>)b).FirstOrDefault(),b);

                }

                var jsonFile = Newtonsoft.Json.JsonConvert.SerializeObject(values);
                System.IO.File.WriteAllText(@"D:\git repository\ExcelExporter\ExcelExporter\Test.json", jsonFile);

                var containerTypeCell = worksheet.Cells[2, 1].Value as string;
                switch (containerTypeCell.ToLower())
                {
                    case "string":
                        
                        break;

                    case "int":
                        break;

                    default:
                        throw new Exception("not defined container type. " + containerTypeCell);
                }

                var container = 
                    string.Format(
                        "Dictionary<{0},{1}> Container = new Dictionary<{0},{1}>();\n",
                        containerTypeCell,
                        dataClassName);

                string file = string.Format(
                    CS_FILE,
                    fileName,
                    dataClass,
                    container
                    );

                file = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(file).GetRoot().NormalizeWhitespace().ToFullString();
                Console.WriteLine(file);

                var t = new { Name = "a" };
                Console.WriteLine(t.GetType());

                Type d = typeof(Dictionary<,>);
                Type constructed = d.MakeGenericType(typeof(int),t.GetType());

                var inst = Activator.CreateInstance(constructed);


                System.IO.File.WriteAllText(@"D:\git repository\ExcelExporter\ExcelExporter\Test.cs", file);

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
