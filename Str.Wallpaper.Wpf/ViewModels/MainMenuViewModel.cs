﻿using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewModels {

  [Export]
  [ViewModel("MainMenuViewModel")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class MainMenuViewModel : ObservableObject {

    #region Private Fields

    private RelayCommand exit;

    #endregion Private Fields

    #region Properties

    public RelayCommand Exit {
      get { return exit; }
      set { SetField(ref exit, value, () => Exit); }
    }

    #endregion Properties

  }

}