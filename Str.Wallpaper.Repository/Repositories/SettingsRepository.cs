using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using AutoMapper;

using Newtonsoft.Json;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Repository.Models.Settings;


namespace Str.Wallpaper.Repository.Repositories {

  [Export(typeof(IWindowSettingsRepository))]
  [Export(typeof(IProgramSettingsRepository))]
  [Export(typeof(IUserSettingsRepository))]
  public sealed class SettingsRepository : IWindowSettingsRepository, IProgramSettingsRepository, IUserSettingsRepository {

    #region Private Fields

    private const string Secret = ";aoiuf6qe875PO&*HNWVYT.wf[l-0fl"; // AES

    private static readonly string programSettingsFile;
    private static readonly string    userSettingsFile;
    private static readonly string windowSettingsFile;

    private readonly ICryptoService cryptoService;

    private readonly IMapper mapper;

    #endregion Private Fields

    #region Constructors

    static SettingsRepository() {
      programSettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\STR.Wallpaper\ProgramSettings.json");
         userSettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\STR.Wallpaper\UserSettings.json");
      windowSettingsFile  = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\STR.Wallpaper\WindowSettings.json");
    }

    [ImportingConstructor]
    public SettingsRepository(ICryptoService CryptoService, IMapper Mapper) {
      cryptoService = CryptoService;

      mapper = Mapper;
    }

    #endregion Constructors

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

    #region IUserSettingsRepository Implementation

    public async Task LoadUserSettingsAsync(DomainUser Settings) {
      if (await Task.Run(() => File.Exists(userSettingsFile))) {
        UserSettings settings = await Task.Run(() => JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(userSettingsFile)));

        settings.Password = cryptoService.DecryptStringAes(settings.Password, Secret, settings.NaCl);

        mapper.Map(settings, Settings);
      }
    }

    public async Task SaveUserSettingsAsync(DomainUser Settings) {
      UserSettings settings = mapper.Map<UserSettings>(Settings);

      Tuple<string, string> saltHash = cryptoService.EncryptStringAes(settings.Password, Secret, settings.NaCl);

      settings.NaCl     = saltHash.Item1;
      settings.Password = saltHash.Item2;

      string json = await Task.Run(() => JsonConvert.SerializeObject(settings, Formatting.Indented));

      if (!await Task.Run(() => File.Exists(userSettingsFile))) await Task.Run(() => Directory.CreateDirectory(Path.GetDirectoryName(userSettingsFile)));

      await Task.Run(() => File.WriteAllText(userSettingsFile, json));
    }

    #endregion IUserSettingsRepository Implementation

  }

}
