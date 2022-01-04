using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

public class ZeroFormatterGenerator : MonoBehaviour
{
    private static readonly string EXE_PATH = Application.dataPath + "/ZeroFormatter/Editor/zfc.exe";
    private static readonly string PROJ_DIRECTORY = Application.dataPath.Replace("/Assets","") + "";
    private static readonly string PROJ_FILE_NAME = "Assembly-CSharp.csproj";

    private static readonly string OUTPUT_PATH = Application.dataPath + "/ZeroFormatterGenerated.cs";
    private const string COMMAND = "-i \"{0}\" -o \"{1}\" ";

    [MenuItem("Window/ZeroFormatter/Execute")]
    private static void Execute()
    {

        var files = System.IO.Directory.GetFiles(PROJ_DIRECTORY);
        string csprojPath = null;
        foreach (var f in files)
        {
            var fileName = System.IO.Path.GetFileName(f);
            if (fileName == PROJ_FILE_NAME)
            {
                csprojPath = f;
                break;
            }
        }

        if (string.IsNullOrEmpty(csprojPath) == true)
            return;

        if (System.IO.File.Exists(EXE_PATH) == true)
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = EXE_PATH,
                Arguments = string.Format(COMMAND, csprojPath, OUTPUT_PATH),
                UseShellExecute = false,
                RedirectStandardOutput = false,
            };

            var process = System.Diagnostics.Process.Start(start);
            process.WaitForExit();
            Debug.Log("end");
        }
    }

}
