using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BepInEx;

namespace KN_Loader {
  public static class Updater {
    private static readonly string ScriptPath = Path.GetTempPath() + Path.DirectorySeparatorChar + "KN_Updater.ps1";
    private static readonly string UpdaterPath = Paths.PluginPath + Path.DirectorySeparatorChar + "KN_Updater.dll";
    private static readonly string UpdaterVersionPath = Path.GetTempPath() + Path.DirectorySeparatorChar + "KN_UpdaterVersion.txt";

    public static void StartUpdater(int latestUpdater, bool forceUpdate, bool dev, bool saveLog, bool checkUpdater) {
      if (forceUpdate && !checkUpdater) {
        CheckForNewUpdater(latestUpdater);
      }

      var args = new List<string> {
        forceUpdate ? "0.0.0" : ModLoader.StringVersion,
        dev ? Paths.GameRootPath + Path.DirectorySeparatorChar + "KnUpdate" : Paths.PluginPath,
        $"{saveLog}"
      };

      if (!MakeScript("Update", args.ToArray())) {
        Log.Write("[KN_Loader::Updater]: Failed to make update scrip");
        return;
      }

      if (!RunScript(false)) {
        return;
      }
      Log.Write("[KN_Loader::Updater]: Updater started ...");
    }

    public static void CheckForNewUpdater(int latestUpdater) {
      bool shouldDownload = true;

      int version = GetUpdaterVersion();
      if (version == latestUpdater) {
        shouldDownload = false;
      }

      Log.Write($"[KN_Loader::Updater]: Updater version: C: {version} / L: {latestUpdater}, download: {shouldDownload}");

      if (shouldDownload) {
        DownloadNewUpdater(UpdaterPath);
      }
    }

    private static void DownloadNewUpdater(string path) {
      var bytes = WebDataLoader.DownloadNewUpdater();

      if (bytes == null) {
        return;
      }

      try {
        using (var memory = new MemoryStream(bytes)) {
          using (var fileStream = File.Open(path, FileMode.Create)) {
            memory.CopyTo(fileStream);
          }
        }
      }
      catch (Exception e) {
        Log.Write($"[KN_Loader::Updater]: Failed to save updater to disc, {e.Message}");
      }
    }

    private static int GetUpdaterVersion() {
      int version = -1;

      if (!MakeScript("GetVersion")) {
        return version;
      }

      if (!RunScript(true)) {
        return version;
      }

      try {
        string text = File.ReadAllText(UpdaterVersionPath);
        if (!int.TryParse(text, out version)) {
          Log.Write($"[KN_Loader::Updater]: Failed to parse updater version, text: '{text}'");
          version = -1;
        }
      }
      catch (Exception e) {
        Log.Write($"[KN_Loader::Updater]: Failed to read updater version from '{UpdaterVersionPath}', {e.Message}");
      }

      return version;
    }

    private static bool RunScript(bool version) {
      if (!File.Exists(ScriptPath)) {
        return false;
      }

      var startInfo = new ProcessStartInfo {
        FileName = "powershell.exe",
        Arguments = $"-NoProfile -ExecutionPolicy unrestricted -file \"{ScriptPath}\"",
        UseShellExecute = false,
        WindowStyle = ProcessWindowStyle.Hidden
      };

      string script = version ? "version" : "update";
      try {
        var proc = Process.Start(startInfo);
        if (proc != null) {
          Log.Write($"[KN_Loader::Updater]: Running '{script}' script");
        }
        else {
          Log.Write($"[KN_Loader::Updater]: Unable to run '{script}' script");
          return false;
        }
        return true;
      }
      catch (Exception e) {
        Log.Write($"[KN_Loader::Updater]: Error while starting process '{script}', {e.Message}");
        return false;
      }
    }

    private static bool MakeScript(string entryPoint, string[] args = null) {
      try {
        string file = $"[Reflection.Assembly]::LoadFile(\"{UpdaterPath}\")\n";
        if (args != null) {
          string ar = args.Aggregate("", (current, a) => current + $"\"{a}\", ");
          ar = ar.Remove(ar.Length - 2, 2);

          file += $"$args = {ar}\n";
          file += $"[KN_Updater.Program]::{entryPoint}($args)";
        }
        else {
          file += $"[KN_Updater.Program]::{entryPoint}()";
        }

        File.WriteAllText(ScriptPath, file);
      }
      catch (Exception e) {
        Log.Write($"[KN_Loader::Updater]: Failed to save updater script to disc, {e.Message}");
        return false;
      }

      return true;
    }
  }
}