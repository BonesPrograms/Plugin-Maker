using System.Diagnostics;

namespace BonesPluginMaker;

public class PluginMaker
{
    const string ModsRoot = @"C:\Users\user\Desktop\VietnamWarModLab";
    const string InteropAssembly = @"C:\Program Files (x86)\Steam\steamapps\common\VietnamWar\BepInEx\interop\Assembly-CSharp.dll";
    public void Make()
    {
        string pluginName = EnterName();
        Console.WriteLine($"Are you sure you want the name of your plugin to be {pluginName}?");
        Console.WriteLine("Press ENTER for yes, press any other key to change the name.");
        if (Console.ReadKey().Key != ConsoleKey.Enter)
        {
            Console.WriteLine("Enter a new name:");
            Make();
        }
        else
            CreateDirectory(pluginName);
    }

    void CreateDirectory(string name)
    {
        string pluginPath = Path.Combine(ModsRoot, name!);
        Directory.CreateDirectory(pluginPath);
        CreateProject(name, pluginPath);
        AddInteropReference(name, pluginPath);
        Console.WriteLine($"{name} created in {pluginPath}");
    }

    void AddInteropReference(string name, string path)
    {
        string csproj = $@"{path}\{name}.csproj";
        List<string> text = GetAndModifyText(csproj);
        using (StreamWriter writer = new(csproj))
        {
            foreach (var txt in text)
                writer.WriteLine(txt);
        }
    }

    static List<string> GetAndModifyText(string csproj)
    {
        List<string> text = [.. File.ReadAllLines(csproj)];
        int insertion = text.IndexOf("  </ItemGroup>");
        text.Insert(insertion, $"    <Reference Include=\"{InteropAssembly}\"/>");
        return text;
    }



    void CreateProject(string name, string path)
    {
        ProcessStartInfo create = new()
        {
            FileName = "dotnet",
            Arguments = $"new bep6plugin_unity_il2cpp -n \"{name}\" -o \"{path}\"",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        using (Process? process = Process.Start(create))
        {
            if (process != null)
            {
                string output = process.StandardOutput.ReadToEnd();
                string err = process.StandardError.ReadToEnd();
                process.WaitForExit();
                int code = process.ExitCode;
                if (code != 0)
                {
                    Console.WriteLine(output);
                    Console.WriteLine(err);
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Process is null");
                Console.ReadLine();
            }
        }
    }

    public static string EnterName()
    {
        bool invalid = true;
        string? plugin = string.Empty;
        while (invalid)
        {
            plugin = Console.ReadLine();
            invalid = string.IsNullOrWhiteSpace(plugin);
        }
        return plugin!;
    }
}