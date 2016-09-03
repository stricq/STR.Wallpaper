using System.Diagnostics.CodeAnalysis;


namespace Str.Wallpaper.Repository.Models.Settings {

  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  public sealed class WindowSettings {

    public bool IsStartMinimized { get; set; }

    public double WindowW { get; set; }

    public double WindowH { get; set; }

    public double WindowX { get; set; }

    public double WindowY { get; set; }

    public double SplitterDistance { get; set; }

    public int MainWindowState { get; set; }

    public int PreMinimizedState { get; set; }

  }

}
