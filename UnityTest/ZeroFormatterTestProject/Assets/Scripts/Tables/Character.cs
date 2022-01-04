using System;
using System.Collections.Generic;
using ZeroFormatter;
using ZeroFormatter.Formatters;

namespace Table
{
    /*
            Table // namespace
            CharacterTable // main class name
            CharacterData // data class name
            [Index(0)]public virtual int ID{get;set;}
[Index(1)]public virtual int Value{get;set;}
[Index(2)]public virtual string Description{get;set;}
 // data fields
            Dictionary<int,CharacterData>
 // container type
            @"D:\git repository\ExcelExporter\ExcelExporter\bin\Debug\Files\Character.bytes" // text file path
            Formatter.RegisterDictionary<DefaultResolver, int, CharacterData>(); // resolver
            */
    // class
    public class CharacterTable : ITableDeserialization
    {
        // data
        [ZeroFormattable]
        public class CharacterData
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
        public Dictionary<int, CharacterData> Container = new Dictionary<int, CharacterData>();
        public void Deserialize()
        {
#if EE_GENERATED == false
                    Formatter.RegisterDictionary<DefaultResolver, int, CharacterData>();
#endif
            string path = @"D:\git repository\ExcelExporter\ExcelExporter\bin\Debug\Files\Character.bytes";
            var load = System.IO.File.ReadAllBytes(path);
            Container = ZeroFormatterSerializer.Deserialize<Dictionary<int, CharacterData>>(load);
        }

        public void DeserializeFromBytes(byte[] bytes)
        {
#if EE_GENERATED == false
                    Formatter.RegisterDictionary<DefaultResolver, int, CharacterData>();
#endif
            Container = ZeroFormatterSerializer.Deserialize<Dictionary<int, CharacterData>>(bytes);
        }

#region
#if EE_GENERATED
        public void MakeSerializedFile(string txt)
        {
            try
            {
                var container = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, CharacterData>>(txt);
                var bytes = ZeroFormatterSerializer.Serialize(container);
                System.IO.File.WriteAllBytes(@"D:\git repository\ExcelExporter\ExcelExporter\bin\Debug\Files\Character.bytes", bytes);
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