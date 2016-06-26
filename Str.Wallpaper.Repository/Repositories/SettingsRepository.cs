using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Str.Wallpaper.Domain.Contracts;

using Str.Wallpaper.Repository.Models.Settings;


namespace Str.Wallpaper.Repository.Repositories {

  [Export(typeof(IWindowSettingsRepository))]
  public class SettingsRepository : IWindowSettingsRepository {

    #region Private Fields

    private static readonly string windowsSettingsFile;

    #endregion Private Fields

    #region Constructor

    static SettingsRepository() {
      windowsSettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\STR.Wallpaper\WindowSettings.json");
    }

    #endregion Constructor

    #region ISettingsRepository Implementation

    public async Task<WindowSettings> LoadWindowSettings() {
      WindowSettings settings;

      if (await Task.Run(() => File.Exists(windowsSettingsFile))) {
        settings = await Task.Run(() => JsonConvert.DeserializeObject<WindowSettings>(File.ReadAllText(windowsSettingsFile)));
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

    public async Task SaveWindowSettings(WindowSettings settings) {
      string json = await Task.Run(() => JsonConvert.SerializeObject(settings, Formatting.Indented));

      if (!await Task.Run(() => File.Exists(windowsSettingsFile))) await Task.Run(() => Directory.CreateDirectory(Path.GetDirectoryName(windowsSettingsFile)));

      await Task.Run(() => File.WriteAllText(windowsSettingsFile, json));
    }

    #endregion ISettingsRepository Implementation

  }

}
