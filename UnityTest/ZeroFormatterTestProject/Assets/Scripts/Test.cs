using System;
using System.Collections.Generic;
using ZeroFormatter;
using ZeroFormatter.Formatters;

namespace Table
{
    /*
            TestTable // main class name
            TestData // data class name
            [Index(0)]public virtual int ID{get;set;}
[Index(1)]public virtual int Power{get;set;}
[Index(2)]public virtual string Desc{get;set;}
[Index(3)]public virtual float Value{get;set;}
 // data fields
            Dictionary<int,TestData>
 // container type
            @"D:\git repository\ExcelExporter\ExcelExporter\Test.byte" // text file path
            Formatter.RegisterDictionary<DefaultResolver, int, TestData>(); // resolver
            */
    // class
    public class TestTable
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
            public virtual int Power
            {
                get;
                set;
            }

            [Index(2)]
            public virtual string Desc
            {
                get;
                set;
            }

            [Index(3)]
            public virtual float Value
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
            string path = @"D:\git repository\ExcelExporter\ExcelExporter\Test.byte";
            var load = System.IO.File.ReadAllBytes(path);
            Container = ZeroFormatterSerializer.Deserialize<Dictionary<int, TestData>>(load);
        }

#region
#if EE_GENERATED
        public void MakeSerializedFile(string txt)
        {
            try
            {
                var container = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, TestData>>(txt);
                var bytes = ZeroFormatterSerializer.Serialize(container);
                System.IO.File.WriteAllBytes(@"D:\git repository\ExcelExporter\ExcelExporter\Test.byte", bytes);
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