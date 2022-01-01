using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            ExcelExporter e = new ExcelExporter();

            //string path = @"D:\git repository\ExcelExporter\ExcelExporter\Test.xlsx";
            //e.Export(path);

            string path = "Files";
            e.ExportAll(path);
        }
    }
}
