using System.Diagnostics.CodeAnalysis;


namespace Str.Wallpaper.Repository.Models.Settings {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public sealed class UserSettings {

    public string Username { get; set; }

    public string Password { get; set; }

    public string NaCl { get; set; }

  }

}
