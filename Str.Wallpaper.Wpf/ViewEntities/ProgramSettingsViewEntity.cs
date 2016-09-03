using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewEntities {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class ProgramSettingsViewEntity : ObservableObject {

    #region Private Fields

    private bool areSettingsChanged;

    private bool isChangeAndExit;
    private bool isRecursiveDirectoryScan;
    private bool isSkipStartupChange;
    private bool isStartMinimized;
    private bool isStartWithWindows;

    private bool useFilterMinimumWidth;
    private bool useFilterMinimumHeight;

    private bool useFullPathDrop;

    private int changeMinutes;

    private int filterMinimumWidth;
    private int filterMinimumHeight;

    private string imageCacheDirectory;

    #endregion Private Fields

    #region Properties

    public bool AreSettingsChanged {
      get { return areSettingsChanged; }
      set { SetField(ref areSettingsChanged, value, () => AreSettingsChanged); }
    }

    public bool IsChangeAndExit {
      get { return isChangeAndExit; }
      set { AreSettingsChanged |= SetField(ref isChangeAndExit, value, () => IsChangeAndExit); }
    }

    public bool IsRecursiveDirectoryScan {
      get { return isRecursiveDirectoryScan; }
      set { AreSettingsChanged |= SetField(ref isRecursiveDirectoryScan, value, () => IsRecursiveDirectoryScan); }
    }

    public bool IsSkipStartupChange {
      get { return isSkipStartupChange; }
      set { AreSettingsChanged |= SetField(ref isSkipStartupChange, value, () => IsSkipStartupChange); }
    }

    public bool IsStartMinimized {
      get { return isStartMinimized; }
      set { AreSettingsChanged |= SetField(ref isStartMinimized, value, () => IsStartMinimized); }
    }

    public bool IsStartWithWindows {
      get { return isStartWithWindows; }
      set { AreSettingsChanged |= SetField(ref isStartWithWindows, value, () => IsStartWithWindows); }
    }

    public bool UseFilterMinimumWidth {
      get { return useFilterMinimumWidth; }
      set { AreSettingsChanged |= SetField(ref useFilterMinimumWidth, value, () => UseFilterMinimumWidth); }
    }

    public bool UseFilterMinimumHeight {
      get { return useFilterMinimumHeight; }
      set { AreSettingsChanged |= SetField(ref useFilterMinimumHeight, value, () => UseFilterMinimumHeight); }
    }

    public bool UseFullPathDrop {
      get { return useFullPathDrop; }
      set { AreSettingsChanged |= SetField(ref useFullPathDrop, value, () => UseFullPathDrop); }
    }

    public int ChangeMinutes {
      get { return changeMinutes; }
      set { AreSettingsChanged |= SetField(ref changeMinutes, value, () => ChangeMinutes); }
    }

    public int FilterMinimumWidth {
      get { return filterMinimumWidth; }
      set { AreSettingsChanged |= SetField(ref filterMinimumWidth, value, () => FilterMinimumWidth); }
    }

    public int FilterMinimumHeight {
      get { return filterMinimumHeight; }
      set { AreSettingsChanged |= SetField(ref filterMinimumHeight, value, () => FilterMinimumHeight); }
    }

    public string ImageCacheDirectory {
      get { return imageCacheDirectory; }
      set { AreSettingsChanged |= SetField(ref imageCacheDirectory, value, () => ImageCacheDirectory); }
    }

    #endregion Properties

  }

}
