﻿using System.Windows;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewEntities {

  public class WindowSettingsViewEntity : ObservableObject {

    #region Private Fields

    private bool areSettingsChanged;

    private double windowW;
    private double windowH;

    private double windowX;
    private double windowY;

    private double splitterDistance;

    private WindowState mainWindowState;
    private WindowState preMinimizedState;

    #endregion Private Fields

    #region Properties

    public bool AreSettingsChanged {
      get { return areSettingsChanged; }
      set { SetField(ref areSettingsChanged, value, () => AreSettingsChanged); }
    }

    public double WindowW {
      get { return windowW; }
      set { AreSettingsChanged |= SetField(ref windowW, value, () => WindowW); }
    }

    public double WindowH {
      get { return windowH; }
      set { AreSettingsChanged |= SetField(ref windowH, value, () => WindowH); }
    }

    public double WindowX {
      get { return windowX; }
      set { AreSettingsChanged |= SetField(ref windowX, value, () => WindowX); }
    }

    public double WindowY {
      get { return windowY; }
      set { AreSettingsChanged |= SetField(ref windowY, value, () => WindowY); }
    }

    public double SplitterDistance {
      get { return splitterDistance; }
      set { AreSettingsChanged |= SetField(ref splitterDistance, value, () => SplitterDistance); }
    }

    public WindowState MainWindowState {
      get { return mainWindowState; }
      set { AreSettingsChanged |= SetField(ref mainWindowState, value, () => MainWindowState); }
    }

    public WindowState PreMinimizedState {
      get { return preMinimizedState; }
      set { AreSettingsChanged |= SetField(ref preMinimizedState, value, () => PreMinimizedState); }
    }

    #endregion Properties

  }

}
