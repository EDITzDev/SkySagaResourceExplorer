using System;
using System.IO;
using System.Threading;
using System.Text.Json;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ResourceExplorer;

public static class Program
{
    public static List<ResourcePack> Packs = new();
    public static Dictionary<ulong, string> Names = new();

    [STAThread]
    private static void Main()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

        LoadNameList("NameList32.json", true);
        LoadNameList("NameList64.json", false);

        ApplicationConfiguration.Initialize();
        Application.Run(new Main());
    }

    private static void LoadNameList(string path, bool crc32)
    {
        if (!File.Exists(path))
            return;

        var nameList = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(path));

        if (nameList is null)
            return;

        foreach (var name in nameList)
            Names.TryAdd(crc32 ? Util.ComputeCrc32(name) : Util.ComputeCrc64(name), name);
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        try
        {
            var exception = string.Empty;

            if (e.Exception is not null)
                exception = $"{e.Exception.Message}{e.Exception.StackTrace}";

            MessageBox.Show($"""
                            A critical error has occurred, please raise an issue in GitHub with the below information:
                            {exception}
                            """, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
        finally
        {
            Application.Exit();
        }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            var exception = string.Empty;

            if (e.ExceptionObject is Exception ex)
                exception = $"{ex.Message}{ex.StackTrace}";

            MessageBox.Show($"""
                            A critical error has occurred, please raise an issue in GitHub with the below information:
                            {exception}
                            """, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
        finally
        {
            Application.Exit();
        }
    }
}