using System;
using System.IO;
using System.Reflection;

namespace KN_Updater {
  public static class Program {
    private const int Version = 03;

    private static string version_ = "0.0.0";
    private static string modPath_ = "";
    private static bool saveLog_;

    private static readonly string UpdaterVersionPath = Path.GetTempPath() + Path.DirectorySeparatorChar + "KN_UpdaterVersion.txt";

    public static void Update(string[] args) {
      const string octokit = "KN_Updater.Data.Octokit.dll";
      Embedded.Load(octokit, "Octokit.dll");

      AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

      Log.Init();

      try {
        version_ = args[0];
        Log.Write($"Current version: {version_}");

        modPath_ = args[1];
        Log.Write($"Mod path: {modPath_}");

        saveLog_ = Convert.ToBoolean(args[2]);
      }
      catch (Exception e) {
        Log.Write($"Failed to parse args, {e.Message}");
        SaveLog();
      }

      var updater = new Updater();
      if (!updater.Initialize()) {
        Log.Write("Kino update failed. Exiting ...");
        SaveLog();
        return;
      }

      if (!updater.IsUpdateNeeded(version_)) {
        Log.Write("No update needed. Exiting ...");
        SaveLog();
        return;
      }

      updater.Run(modPath_);

      SaveLog();
    }

    public static void GetVersion() {
      Console.WriteLine($"Updater version: {Version}");
      try {
        File.WriteAllText(UpdaterVersionPath, $"{Version}");
      }
      catch (Exception e) {
        Console.WriteLine($"Failed to write version, {e}");
      }
    }

    private static void SaveLog() {
      if (saveLog_) {
        Log.Save(modPath_);
      }
    }

    private static Assembly AssemblyResolve(object sender, ResolveEventArgs args) {
      return Embedded.Get(args.Name);
    }
  }
}