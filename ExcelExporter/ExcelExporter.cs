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

        private const string ZEROFORMATTER_RESOURCE_EXTENSION = ".bytes";
        private const string NAMESPACE = "Table";

        private const string BASE_CLASS_CS_FILE_NAME = "IDeserialization.cs";

        private const string BASE_CLASS_CS_FILE_ORIGIN =
        @"
        namespace {0}
        {{
            public interface ITableDeserialization
            {{
                void DeserializeFromBytes(byte[] bytes);
            }}
        }}
        ";

        private readonly string BASE_CLASS_CS_FILE
            = string.Format(BASE_CLASS_CS_FILE_ORIGIN, NAMESPACE);

        private const string CS_FILE = @"
        using System;
        using System.Collections.Generic;
        using ZeroFormatter;
        using ZeroFormatter.Formatters;
        namespace {0}
        {{
            /*
            {0} // namespace
            {1} // main class name
            {2} // data class name
            {3} // data fields
            {4} // container type
            {5} // bytes file path
            {6} // resolver
            */


            // class
            
            public class {1} : ITableDeserialization
            {{
                // data
                [ZeroFormattable]
                public class {2}
                {{
                    {3}
                }}

                // Container
                public {4} Container = new {4}();

                public void Deserialize()
                {{
#if true == false
                    {6}
#endif
                    string path = {5};
                    var load = System.IO.File.ReadAllBytes(path);
                    Container = ZeroFormatterSerializer.Deserialize<{4}>(load);

                }}

                public void DeserializeFromBytes(byte[] bytes)
                {{
#if true == false
                    {6}
#endif
                    Container = ZeroFormatterSerializer.Deserialize<{4}>(bytes);
                }}

#region
#if true
                public void MakeSerializedFile(string txt)
                {{
                    try
                    {{
                        var container = Newtonsoft.Json.JsonConvert.DeserializeObject<{4}>(txt);
                        var bytes = ZeroFormatterSerializer.Serialize(container);
                        System.IO.File.WriteAllBytes({5}, bytes);
                        
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

        public void CreateBaseInterfaceFile(string directory)
        {
            var path = directory + "/" + BASE_CLASS_CS_FILE_NAME;
            Console.WriteLine(path);
            System.IO.File.WriteAllText(path, BASE_CLASS_CS_FILE);
        }

        public void ExportAll(string inputDirectory, string outputDirectory)
        {
            var files = System.IO.Directory.GetFiles(inputDirectory);
            foreach (var f in files)
            {
                var extension = System.IO.Path.GetExtension(f);
                if (extension != ".xlsx")
                    continue;

                var inputFullPath = f;

                Export(inputFullPath, outputDirectory);
            }

            CreateBaseInterfaceFile(outputDirectory);
        }

        public void Export(string inputPath, string outputDirectory)
        {

            string fileName = System.IO.Path.GetFileNameWithoutExtension(inputPath);
            string pathWithoutExtension = inputPath.Replace(".xlsx", "");

            string className = fileName + "Table";
            string dataName = fileName + "Data";

            string jsonPath = outputDirectory + "\\" + fileName + ".json";
            string csPath = outputDirectory + "\\" + fileName + ".cs";
            string bytesPath = outputDirectory + "\\" + fileName + ".bytes";

            Excel.Application excel = new Excel.Application();
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;
            
            try
            {
                workbook = excel.Workbooks.Open(inputPath); //needed full path
                if (workbook == null)
                    return;

                Console.WriteLine($"opening {inputPath}");

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

                // for zero formatter serialization
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
                    NAMESPACE,
                    className,
                    dataName,
                    fields,
                    container,
                    "@" + QUOTE + bytesPath + QUOTE,
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

                var compileSource = file + BASE_CLASS_CS_FILE;

                System.CodeDom.Compiler.CompilerResults results 
                    = codeDom.CompileAssemblyFromSource(cparams, compileSource);

                if (results.Errors.Count > 0)
                {
                    foreach (var err in results.Errors)
                    {
                        Console.WriteLine(err.ToString());
                    }
                    return;
                }
                Type myType = results.CompiledAssembly.GetType($"{NAMESPACE}.{fileName}Table");
                object myObject = Activator.CreateInstance(myType);
                MethodInfo mi = myObject.GetType().GetMethod("MakeSerializedFile");
                MethodInfo deserializeMethod = myObject.GetType().GetMethod("Deserialize");

                
                mi.Invoke(myObject,new object[] { jsonFile, });
                deserializeMethod.Invoke(myObject, new object[] { });

                System.IO.File.WriteAllText(csPath, file);
                
                System.IO.File.Delete(jsonPath);
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

                Console.WriteLine($"closing {inputPath}");
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
