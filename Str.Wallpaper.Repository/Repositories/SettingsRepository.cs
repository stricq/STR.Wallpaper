using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Str.Wallpaper.Domain.Contracts;

using Str.Wallpaper.Repository.Models.Settings;


namespace Str.Wallpaper.Repository.Repositories {

  [Export(typeof(IWindowSettingsRepository))]
  [Export(typeof(IProgramSettingsRepository))]
  public sealed class SettingsRepository : IWindowSettingsRepository, IProgramSettingsRepository {

    #region Private Fields

    private static readonly string programSettingsFile;
    private static readonly string windowSettingsFile;

    #endregion Private Fields

    #region Constructor

    static SettingsRepository() {
      programSettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\STR.Wallpaper\ProgramSettings.json");
      windowSettingsFile  = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\STR.Wallpaper\WindowSettings.json");
    }

    #endregion Constructor

    #region IWindowSettingsRepository Implementation

    public async Task<WindowSettings> LoadWindowSettingsAsync() {
      WindowSettings settings;

      if (await Task.Run(() => File.Exists(windowSettingsFile))) {
        settings = await Task.Run(() => JsonConvert.DeserializeObject<WindowSettings>(File.ReadAllText(windowSettingsFile)));
      }
      else settings = new WindowSettings {
        WindowW = 1024,
        WindowH = 768,

        WindowX = 100,
        WindowY = 100,

        SplitterDistance = 250
      };

      return settings;
    }

    public async Task SaveWindowSettingsAsync(WindowSettings settings) {
      string json = await Task.Run(() => JsonConvert.SerializeObject(settings, Formatting.Indented));

      if (!await Task.Run(() => File.Exists(windowSettingsFile))) await Task.Run(() => Directory.CreateDirectory(Path.GetDirectoryName(windowSettingsFile)));

      await Task.Run(() => File.WriteAllText(windowSettingsFile, json));
    }

    #endregion IWindowSettingsRepository Implementation

    #region IProgramSettingsRepository Implementation

    public async Task<ProgramSettings> LoadProgramSettingsAsync() {
      ProgramSettings settings;

      if (await Task.Run(() => File.Exists(programSettingsFile))) {
        settings = await Task.Run(() => JsonConvert.DeserializeObject<ProgramSettings>(File.ReadAllText(programSettingsFile)));
      }
      else settings = new ProgramSettings {
        IsRecursiveDirectoryScan = true,
        IsSkipStartupChange      = true,

        UseFilterMinimumWidth  = true,
        UseFilterMinimumHeight = true,

        UseFullPathDrop = true,

        ChangeMinutes = 8,

        FilterMinimumWidth  = 1280,
        FilterMinimumHeight = 720,

        ImageCacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "STR Wallpaper")
      };

      return settings;
    }

    public async Task SaveProgramSettingsAsync(ProgramSettings Settings) {
      string json = await Task.Run(() => JsonConvert.SerializeObject(Settings, Formatting.Indented));

      if (!await Task.Run(() => File.Exists(programSettingsFile))) await Task.Run(() => Directory.CreateDirectory(Path.GetDirectoryName(programSettingsFile)));

      await Task.Run(() => File.WriteAllText(programSettingsFile, json));
    }

    #endregion IProgramSettingsRepository Implementation

  }

}
