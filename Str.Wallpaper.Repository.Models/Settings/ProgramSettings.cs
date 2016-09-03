using System.Diagnostics.CodeAnalysis;


namespace Str.Wallpaper.Repository.Models.Settings {

  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  public sealed class ProgramSettings {

    public bool IsChangeAndExit { get; set; }

    public bool IsRecursiveDirectoryScan { get; set; }

    public bool IsSkipStartupChange { get; set; }

    public bool IsStartMinimized { get; set; }

    public bool IsStartWithWindows { get; set; }

    public bool UseFilterMinimumWidth { get; set; }

    public bool UseFilterMinimumHeight { get; set; }

    public bool UseFullPathDrop { get; set; }

    public int ChangeMinutes { get; set; }

    public int FilterMinimumWidth { get; set; }

    public int FilterMinimumHeight { get; set; }

    public string ImageCacheDirectory { get; set; }

  }

}
