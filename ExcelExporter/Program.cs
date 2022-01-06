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

            string defaultInputDirectory = System.IO.Directory.GetCurrentDirectory() + "\\Files_new";
            string defaultOutputDirectory = System.IO.Directory.GetCurrentDirectory() + "\\Files_new\\Exported";

            string inputDirectory = defaultInputDirectory;
            string outputDirectory = defaultOutputDirectory;

            if (args.Length >= 2)
            {
                inputDirectory = args[0];
                outputDirectory = args[1];
            }
            else
            {
                inputDirectory = defaultInputDirectory;
                outputDirectory = defaultOutputDirectory;
            }

            Console.WriteLine("-----------------------");
            Console.WriteLine("[ExcelExporter] \n");
            Console.WriteLine($"- Input Directory  : {inputDirectory}");
            Console.WriteLine($"- Output Directory : {outputDirectory}");
            Console.WriteLine("-----------------------");
            if (System.IO.Directory.Exists(inputDirectory) == false)
                System.IO.Directory.CreateDirectory(inputDirectory);

            if (System.IO.Directory.Exists(outputDirectory) == false)
                System.IO.Directory.CreateDirectory(outputDirectory);

            e.ExportAll(inputDirectory,outputDirectory);

            System.Threading.Thread.Sleep(2000);
        }
    }
}
