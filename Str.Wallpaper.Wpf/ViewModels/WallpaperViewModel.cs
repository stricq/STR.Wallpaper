﻿using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

using Str.Wallpaper.Wpf.ViewEntities;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewModels {

  [Export]
  [ViewModel(nameof(WallpaperViewModel))]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class WallpaperViewModel : ObservableObject {

    #region PrivateFields

    private Visibility mainWindowVisibility;

    private bool showInTaskbar;
    private bool topMost;

    private Action show = () => { };

    private Func<bool> focus    = () => true;
    private Func<bool> activate = () => true;

    private RelayCommand<EventArgs> initialized;

    private RelayCommandAsync<RoutedEventArgs> loaded;

    private RelayCommand<CancelEventArgs> closing;

    private RelayCommand<SizeChangedEventArgs> sizeChanged;

    private WindowSettingsViewEntity settings;

    #endregion PrivateFields

    #region Properties

    public Visibility MainWindowVisibility {
      get { return mainWindowVisibility; }
      set { SetField(ref mainWindowVisibility, value, () => MainWindowVisibility); }
    }

    public bool ShowInTaskbar {
      get { return showInTaskbar; }
      set { SetField(ref showInTaskbar, value, () => ShowInTaskbar); }
    }

    public bool TopMost {
      get { return topMost; }
      set { SetField(ref topMost, value, () => TopMost); }
    }

    public Action Show {
      get { return show; }
      set { SetField(ref show, value, () => Show); }
    }

    public Func<bool> Focus {
      get { return focus; }
      set { SetField(ref focus, value, () => Focus); }
    }

    public Func<bool> Activate {
      get { return activate; }
      set { SetField(ref activate, value, () => Activate); }
    }

    public RelayCommand<EventArgs> Initialized {
      get { return initialized; }
      set { SetField(ref initialized, value, () => Initialized); }
    }

    public RelayCommandAsync<RoutedEventArgs> Loaded {
      get { return loaded; }
      set { SetField(ref loaded, value, () => Loaded); }
    }

    public RelayCommand<CancelEventArgs> Closing {
      get { return closing; }
      set { SetField(ref closing, value, () => Closing); }
    }

    public RelayCommand<SizeChangedEventArgs> SizeChanged {
      get { return sizeChanged; }
      set { SetField(ref sizeChanged, value, () => SizeChanged); }
    }

    public WindowSettingsViewEntity Settings {
      get { return settings; }
      set { SetField(ref settings, value, () => Settings); }
    }

    #endregion Properties

  }

}
