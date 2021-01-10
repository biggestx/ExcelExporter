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
        using ZeroFormatter.Formatters;
        namespace Table
        {{
            /*
            {0} // main class name
            {1} // data class name
            {2} // data fields
            {3} // container type
            {4} // text file path
            {5} // resolver
            */


            // class
            
            public class {0} 
            {{
                // data
                [ZeroFormattable]
                public class {1}
                {{
                    {2}
                }}

                // Container
                public {3} Container = new {3}();

                public void Deserialize()
                {{
#if true == false
                    {5}
#endif
                    string path = {4};
                    var load = System.IO.File.ReadAllBytes(path);
                    Container = ZeroFormatterSerializer.Deserialize<{3}>(load);

                }}
#region
#if true
                public void MakeSerializedFile(string txt)
                {{
                    try
                    {{
                        var container = Newtonsoft.Json.JsonConvert.DeserializeObject<{3}>(txt);
                        var bytes = ZeroFormatterSerializer.Serialize(container);
                        System.IO.File.WriteAllBytes({4}, bytes);
                        
                    }}
                    catch(Exception e)
                    {{
                        Console.WriteLine(e.Message);
                    }}

                    Deserialize();
                }}
#endif
#endregion

            }}
        


        }}       

        ";


        public void Export(string path)
        {

            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            string className = fileName + "Table";
            string dataName = fileName + "Data";

            string jsonPath = path + fileName + ".json";
            string csPath = path + fileName + ".cs";
            

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

                    fields += 
                        $"[Index({i-1})]" +
                        "public virtual " + @type + " " + fieldName +
                        "{get;set;}" + "\n";
                    fieldNames.Add(fieldName);
                    switch (type)
                    {
                        case "string":
                            fieldTypes.Add(typeof(string));
                            break; 
                        case "int":
                            fieldTypes.Add(typeof(int));
                            break;
                        case "float":
                            fieldTypes.Add(typeof(float));
                            break;
                    }
                }           
                
                // data class definition 
                Dictionary<dynamic,dynamic> values = new Dictionary<dynamic,dynamic>();

                for (int i = 3; i <= rowCount; ++i)
                {
                    dynamic b = new ExpandoObject();
                    for (int j = 1; j <= colCount; ++j)
                    {
                        Excel.Range cell = worksheet.Cells[i, j];
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
                        else if (valueType == typeof(float))
                        {
                            ((IDictionary<string, Object>)b).Add(fieldNames[j - 1].ToString(), (float)cell.Value);
                        }

                    }
                    values.Add(((IDictionary<string, Object>)b).FirstOrDefault().Value,b);

                }

                var jsonFile = Newtonsoft.Json.JsonConvert.SerializeObject(values);
                System.IO.File.WriteAllText(jsonPath, jsonFile);


                var containerTypeCell = worksheet.Cells[2, 1].Value as string;

                var container = 
                    string.Format(
                        "Dictionary<{0},{1}>\n",
                        containerTypeCell,
                        dataName);

                const string QUOTE = "\"";
                string file = string.Format(
                    CS_FILE,
                    className,
                    dataName,
                    fields,
                    container,
                    "@" + QUOTE + path +"Test.byte" + QUOTE,
                    $"Formatter.RegisterDictionary<DefaultResolver, int, {dataName}>();"
                    );

                file = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(file).GetRoot().NormalizeWhitespace().ToFullString();

                // temporary.
                // fix me : lines in #if keyword are not parsed by CSharpSyntaxTree.ParseText
                file = file.Replace("#if true", "#if EE_GENERATED");

                // compile to export json 
                System.CodeDom.Compiler.CodeDomProvider codeDom = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp");
                System.CodeDom.Compiler.CompilerParameters cparams = new System.CodeDom.Compiler.CompilerParameters();
                cparams.GenerateInMemory = true;
                cparams.ReferencedAssemblies.Add(@"D:\git repository\ExcelExporter\packages\ZeroFormatter.1.6.4\lib\net45\ZeroFormatter.dll");
                cparams.ReferencedAssemblies.Add(@"D:\git repository\ExcelExporter\packages\ZeroFormatter.Interfaces.1.6.4\lib\net45\ZeroFormatter.Interfaces.dll");
                cparams.ReferencedAssemblies.Add(@"D:\git repository\ExcelExporter\packages\Newtonsoft.Json.12.0.3\lib\net45\Newtonsoft.Json.dll");
                cparams.CompilerOptions += "-define:EE_GENERATED";

                System.CodeDom.Compiler.CompilerResults results = codeDom.CompileAssemblyFromSource(cparams, file);
                if (results.Errors.Count > 0)
                {
                    foreach (var err in results.Errors)
                    {
                        Console.WriteLine(err.ToString());
                    }
                    return;
                }
                Type myType = results.CompiledAssembly.GetType("Table.TestTable");
                object myObject = Activator.CreateInstance(myType);
                MethodInfo mi = myObject.GetType().GetMethod("MakeSerializedFile");
                MethodInfo deserializeMethod = myObject.GetType().GetMethod("Deserialize");

                // TODO 
                // 1. json 보내기
                // 2. 받은 json으로 Deserialization
                // 3. ZeroFormatter로 Serialization
                // 4. Unity 에서 사용할 수 있게 using newtonsoft, deserialization 메서드 제거
                mi.Invoke(myObject,new object[] { jsonFile, });
                deserializeMethod.Invoke(myObject, new object[] { });

                //Console.WriteLine(file);

                System.IO.File.WriteAllText(csPath, file);
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
