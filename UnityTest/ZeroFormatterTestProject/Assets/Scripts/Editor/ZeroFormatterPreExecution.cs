using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class ZeroFormatterPreExecution : MonoBehaviour
{
    [MenuItem("Window/Test")]
    public static void Test()
    {
        const string QUOTE = "\"";
        string parameters = $@"-i {QUOTE}D:\git repository\ExcelExporter\UnityTest\ZeroFormatterTestProject\Assembly-CSharp.csproj{QUOTE} - o {QUOTE}D:\git repository\ExcelExporter\UnityTest\ZeroFormatterTestProject\ZeroFormatterGenerated.cs{QUOTE}";

        var process = new Process();
        var si = new ProcessStartInfo();

        si.FileName = "cmd.exe";
        si.WorkingDirectory = Application.dataPath;

        si.UseShellExecute = false;

        si.RedirectStandardOutput = true;
        si.RedirectStandardInput = true;
        si.RedirectStandardError = true;

        process.StartInfo = si;
        process.Start();

        process.StandardInput.WriteLine($@"Plugins\ZeroFormatter\zfc.exe -i {QUOTE}../Assembly-CSharp.csproj{QUOTE} -o {QUOTE}ZeroFormatterGenerated.cs {QUOTE}");

        process.StandardInput.Close();

        UnityEngine.Debug.LogError("output : " + process.StandardOutput.ReadToEnd());
        UnityEngine.Debug.LogError("error : " + process.StandardError.ReadToEnd());

        process.WaitForExit();
        process.Close();
    }

}

#endif