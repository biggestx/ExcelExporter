using System;
using System.Collections.Generic;
using ZeroFormatter;
using ZeroFormatter.Formatters;

namespace Table
{
    /*
            Table // namespace
            TestTable // main class name
            TestData // data class name
            [Index(0)]public virtual int ID{get;set;}
[Index(1)]public virtual int Value{get;set;}
[Index(2)]public virtual string Description{get;set;}
 // data fields
            Dictionary<int,TestData>
 // container type
            @"D:\git repository\ExcelExporter\ExcelExporter\bin\Debug\Files\Test.bytes" // text file path
            Formatter.RegisterDictionary<DefaultResolver, int, TestData>(); // resolver
            */
    // class
    public class TestTable : ITableDeserialization
    {
        // data
        [ZeroFormattable]
        public class TestData
        {
            [Index(0)]
            public virtual int ID
            {
                get;
                set;
            }

            [Index(1)]
            public virtual int Value
            {
                get;
                set;
            }

            [Index(2)]
            public virtual string Description
            {
                get;
                set;
            }
        }

        // Container
        public Dictionary<int, TestData> Container = new Dictionary<int, TestData>();
        public void Deserialize()
        {
#if EE_GENERATED == false
                    Formatter.RegisterDictionary<DefaultResolver, int, TestData>();
#endif
            string path = @"D:\git repository\ExcelExporter\ExcelExporter\bin\Debug\Files\Test.bytes";
            var load = System.IO.File.ReadAllBytes(path);
            Container = ZeroFormatterSerializer.Deserialize<Dictionary<int, TestData>>(load);
        }

        public void DeserializeFromBytes(byte[] bytes)
        {
#if EE_GENERATED == false
                    Formatter.RegisterDictionary<DefaultResolver, int, TestData>();
#endif
            Container = ZeroFormatterSerializer.Deserialize<Dictionary<int, TestData>>(bytes);
        }

#region
#if EE_GENERATED
        public void MakeSerializedFile(string txt)
        {
            try
            {
                var container = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, TestData>>(txt);
                var bytes = ZeroFormatterSerializer.Serialize(container);
                System.IO.File.WriteAllBytes(@"D:\git repository\ExcelExporter\ExcelExporter\bin\Debug\Files\Test.bytes", bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Deserialize();
        }
#endif
#endregion
    }
}