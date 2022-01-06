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
            string outputCsDirectory = defaultOutputDirectory;
            string outputResourceDirectory = defaultOutputDirectory;

            if (args.Length >= 3)
            {
                inputDirectory = args[0];
                outputCsDirectory = args[1];
                outputResourceDirectory = args[2];
            }
            else
            {
                inputDirectory = defaultInputDirectory;
                outputCsDirectory = defaultOutputDirectory;
            }

            Console.WriteLine("-----------------------");
            Console.WriteLine("[ExcelExporter] \n");
            Console.WriteLine($"- Input Directory  : {inputDirectory}");
            Console.WriteLine($"- Output CS Directory : {outputCsDirectory}");
            Console.WriteLine($"- Output Resource Directory : {outputResourceDirectory}");
            Console.WriteLine("-----------------------");
            if (System.IO.Directory.Exists(inputDirectory) == false)
                System.IO.Directory.CreateDirectory(inputDirectory);

            if (System.IO.Directory.Exists(outputCsDirectory) == false)
                System.IO.Directory.CreateDirectory(outputCsDirectory);

            if (System.IO.Directory.Exists(outputResourceDirectory) == false)
                System.IO.Directory.CreateDirectory(outputResourceDirectory);

            e.ExportAll(inputDirectory,outputCsDirectory,outputResourceDirectory);

        }
    }
}
