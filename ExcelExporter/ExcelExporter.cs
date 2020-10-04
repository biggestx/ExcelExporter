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

        private const string CS_FILE = @"
        using System;
        using System.Collections.Generic;
        using ZeroFormatter;

        namespace Table
        {{
            // class
            [ZeroFormattable]
            [Serializable]
            class {0} 
            {{
                // data
                class {1}
                {{
                    {2}
                }}

                // Container
                {3} 

                private void Deserialize()
                {{
                    //string path = {4};
                    //var load = File.ReadAllText(path);
                    //Container = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int,{1}>>(load);
                    //Console.WriteLine(data);
                }}  
                private void Print()
                {{
                    Console.WriteLine({4});
                }}
            }}
        


        }}       

        ";

        [ZeroFormatter.ZeroFormattable]
        public class TEMP
        {
            [ZeroFormatter.Index(0)]
            public int ID;

            [ZeroFormatter.Index(1)]
            public int Power;

            [ZeroFormatter.Index(2)]
            public string Desc;
        }


        public void Test()  
        {
            var load = File.ReadAllText(@"D:\git repository\ExcelExporter\ExcelExporter\Test.json");
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int,TEMP>>(load);
            Console.WriteLine(data);

        }


        public void Export()
        {
            //Test();
            
            string path = @"D:\git repository\ExcelExporter\ExcelExporter\";

            string excelPath = path + "Test.xlsx";

            string fileName = System.IO.Path.GetFileNameWithoutExtension(excelPath);

            string className = fileName + "Table";
            string dataName = fileName + "Data";

            string jsonPath = path + fileName + ".json";
            string csPath = path + fileName + ".cs";


            Excel.Application excel = new Excel.Application();
            var workbook = excel.Workbooks.Open(excelPath);
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
                            fieldTypes.Add(typeof(int));
                            break;
                    }
                }           
                
                // data class definition 
                Dictionary<dynamic,dynamic> values = new Dictionary<dynamic,dynamic>();

                for (int i = 3; i <= rowCount; ++i)
                {
                    dynamic b = new ExpandoObject();
                    System.ComponentModel.TypeDescriptor.AddAttributes(b, new ZeroFormatter.ZeroFormattableAttribute());
                    for (int j = 1; j <= colCount; ++j)
                    {
                        Excel.Range cell = worksheet.Cells[i, j];
                        Console.WriteLine(cell.Value);

                        Type valueType = fieldTypes[j - 1];

                        if (valueType == typeof(string))
                        {
                            ((IDictionary<string, Object>)b).Add(fieldNames[j - 1].ToString(), cell.Value);
                        }
                        else if (valueType == typeof(int))
                        {
                            // todo converting
                            // string / double -> int
                            ((IDictionary<string, Object>)b).Add(fieldNames[j - 1].ToString(), (int)cell.Value); 
                        }


                    }
                    values.Add(((IDictionary<string, Object>)b).FirstOrDefault().Value,b);

                }

                //var jsonFile = ZeroFormatter.ZeroFormatterSerializer.Serialize(values);
                //System.IO.File.WriteAllBytes(jsonPath, jsonFile);


                var containerTypeCell = worksheet.Cells[2, 1].Value as string;

                var container = 
                    string.Format(
                        "Dictionary<{0},{1}> Container = new Dictionary<{0},{1}>();\n",
                        containerTypeCell,
                        fileName+"Data");

                const string QUOTE = "\"";
                string file = string.Format(
                    CS_FILE,
                    className,
                    dataName,
                    fields,
                    container,
                    "@" + QUOTE + path + QUOTE
                    );

                file = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(file).GetRoot().NormalizeWhitespace().ToFullString();
                Console.WriteLine(file);

                System.IO.File.WriteAllText(csPath, file);


                // compile to export json 
                System.CodeDom.Compiler.CodeDomProvider codeDom = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp");
                System.CodeDom.Compiler.CompilerParameters cparams = new System.CodeDom.Compiler.CompilerParameters();
                cparams.GenerateInMemory = true;
                cparams.ReferencedAssemblies.Add(@"D:\git repository\ExcelExporter\packages\ZeroFormatter.1.6.4\lib\net45\ZeroFormatter.dll");
                cparams.ReferencedAssemblies.Add(@"D:\git repository\ExcelExporter\packages\ZeroFormatter.Interfaces.1.6.4\lib\net45\ZeroFormatter.Interfaces.dll");
                System.CodeDom.Compiler.CompilerResults results = codeDom.CompileAssemblyFromSource(cparams, file);
                if (results.Errors.Count > 0)
                {
                    foreach (var err in results.Errors)
                    {
                        Console.WriteLine(err.ToString());
                    }
                    return;
                }
                Type myType = results.CompiledAssembly.GetType("Test.MyClass");
                object myObject = Activator.CreateInstance(myType);
                MethodInfo mi = myObject.GetType().GetMethod("PrintName");
                mi.Invoke(myObject, null);
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
