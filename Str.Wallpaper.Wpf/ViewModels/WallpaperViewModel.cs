using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;

using Str.Wallpaper.Wpf.ViewEntities;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewModels {

  [Export]
  [ViewModel("WallpaperViewModel")]
  public class WallpaperViewModel : ObservableObject {

    #region PrivateFields

    private RelayCommand<RoutedEventArgs> loaded;

    private RelayCommand<CancelEventArgs> closing;

    private RelayCommand<SizeChangedEventArgs> sizeChanged;

    private WindowSettingsViewEntity settings;

    #endregion PrivateFields

    #region Properties

    public RelayCommand<RoutedEventArgs> Loaded {
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
