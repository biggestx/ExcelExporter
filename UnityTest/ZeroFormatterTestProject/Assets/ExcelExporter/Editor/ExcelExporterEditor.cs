using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExcelExporterEditor : EditorWindow
{

    public string RootPath = null;
    public string ExePath = null;
    public string InputPath = null;
    public string OutputPath = null;

    public static string GetRootPath()
    {
        return Application.dataPath + "/ExcelExporter";
    }
    public static string GetEditorPath()
    {
        return GetRootPath() + "/Editor";
    }

    public static string GetDefaultInputPath()
    {
        return GetRootPath() + "/Editor/Files";
    }
    public static string GetDefaultOutputPath()
    {
        return GetRootPath() + "/Editor/Files/Exported";
    }

    [MenuItem("Window/ExcelExporter/Open")]
    public static void Open()
    {
        string DEFAULT_ROOT_PATH = GetRootPath();
        string DEFAULT_EXE_PATH = DEFAULT_ROOT_PATH + "/Editor/ExcelExporter.exe";
        string DEFAULT_INPUT_PATH = GetDefaultInputPath();
        string DEFAULT_OUTPUT_PATH = GetDefaultOutputPath();

        var config = LoadConfig();

        var window = EditorWindow.GetWindow(typeof(ExcelExporterEditor)) as ExcelExporterEditor;
        window.minSize = new Vector2(800, 300);
        window.maxSize = new Vector2(1000, 301);

        window.RootPath = DEFAULT_ROOT_PATH;
        window.ExePath = DEFAULT_EXE_PATH;
        window.InputPath = config != null ? config.InputPath : DEFAULT_INPUT_PATH;
        window.OutputPath = config != null  ? config.OutputPath : DEFAULT_OUTPUT_PATH;

        window.Show();
    }



    public void OnGUI()
    {
        InputPath = EditorGUILayout.TextField("InputPath", InputPath);
        if (GUILayout.Button("Select InputPath"))
        {
            var path = EditorUtility.OpenFolderPanel("title", Application.dataPath, "");
            InputPath = string.IsNullOrEmpty(path) ? InputPath : path;
        }

        GUILayout.Space(30);

        OutputPath = EditorGUILayout.TextField("OutputPath", OutputPath);
        if (GUILayout.Button("Select OutputPath"))
        {
            var path = EditorUtility.OpenFolderPanel("title", Application.dataPath, "");
            OutputPath = string.IsNullOrEmpty(path) ? OutputPath : path;
        }

        GUILayout.Space(60);

        if (GUILayout.Button("Execute"))
        {
            Execute();
        }


        if (GUILayout.Button("Save Config"))
        {
            SaveConfig(new Config()
            {
                InputPath = InputPath,
                OutputPath = OutputPath,
            });
        }

        if (GUILayout.Button("Set to Default Paths"))
        {
            InputPath = GetDefaultInputPath();
            OutputPath = GetDefaultOutputPath();
        }

    }

    public class Config
    {
        public string InputPath;
        public string OutputPath;
    }

    private const string CONFIG_FILE_NAME = "/ExcelExporterConfig.json";

    private static Config LoadConfig()
    {
        var fullPath = GetEditorPath() + CONFIG_FILE_NAME;
        if (System.IO.File.Exists(fullPath) == false)
            return null;

        var json = System.IO.File.ReadAllText(fullPath);
        var config = JsonUtility.FromJson<Config>(json);

        Debug.Log("Load config from " + fullPath);

        return config;
    }

    private static void SaveConfig(Config config)
    {
        var fullPath = GetEditorPath() + CONFIG_FILE_NAME;

        if (System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);
        
        var json = JsonUtility.ToJson(config);
        System.IO.File.WriteAllText(fullPath, json);
        AssetDatabase.Refresh();
        Debug.Log("Save config to " + fullPath);
    }

    public void Execute()
    {
        Debug.Log($"Execute \n- ExePath : {ExePath} \n-InputPath : {InputPath} \n-OutputPath : {OutputPath}");

        var queto = "\"";

        if (System.IO.File.Exists(ExePath) == true)
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = ExePath,
                Arguments = $"{queto}{InputPath}{queto} {queto}{OutputPath}{queto}",
                UseShellExecute = false,
                RedirectStandardOutput = false,
            };

            var process = System.Diagnostics.Process.Start(start);

            process.WaitForExit();
            process.Close();
            Debug.Log("end");
        }
        else
        {
            Debug.LogError("exe file not exist at " + ExePath);
        }

        ZeroFormatterGenerator.Execute(RootPath, OutputPath);

        AssetDatabase.Refresh();
    }

}

public class ZeroFormatterGenerator
{
    private const string COMMAND = "-i \"{0}\" -o \"{1}\" ";

    public static void Execute(string basePath,string outputPath)
    {
        string exePath = basePath + "/ZeroFormatter/Editor/zfc.exe";
        string projDirectory = Application.dataPath.Replace("/Assets", "") + "";
        string projFileName = "Assembly-CSharp.csproj";

        string output = outputPath + "/ZeroFormatterGenerated.cs";

        var files = System.IO.Directory.GetFiles(projDirectory);
        string csprojPath = null;
        foreach (var f in files)
        {
            var fileName = System.IO.Path.GetFileName(f);
            if (fileName == projFileName)
            {
                csprojPath = f;
                break;
            }
        }

        if (string.IsNullOrEmpty(csprojPath) == true)
            return;

        if (System.IO.File.Exists(exePath) == true)
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = exePath,
                Arguments = string.Format(COMMAND, csprojPath, output),
                UseShellExecute = false,
                RedirectStandardOutput = false,
            };

            var process = System.Diagnostics.Process.Start(start);
            process.WaitForExit();
            Debug.Log("end");
        }
    }

}