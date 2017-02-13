using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewModels {

  [Export]
  [ViewModel(nameof(MainMenuViewModel))]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class MainMenuViewModel : ObservableObject {

    #region Private Fields

    private string editCollectionHeader;
    private string deleteCollectionHeader;

    private RelayCommand exit;
    private RelayCommand options;

    private RelayCommandAsync addCollection;
    private RelayCommandAsync editCollection;
    private RelayCommandAsync deleteCollection;

    #endregion Private Fields

    #region Properties

    public string EditCollectionHeader {
      get { return editCollectionHeader; }
      set { SetField(ref editCollectionHeader, value, () => EditCollectionHeader); }
    }

    public string DeleteCollectionHeader {
      get { return deleteCollectionHeader; }
      set { SetField(ref deleteCollectionHeader, value, () => DeleteCollectionHeader); }
    }

    public RelayCommand Exit {
      get { return exit; }
      set { SetField(ref exit, value, () => Exit); }
    }

    public RelayCommand Options {
      get { return options; }
      set { SetField(ref options, value, () => Options); }
    }

    public RelayCommandAsync AddCollection {
      get { return addCollection; }
      set { SetField(ref addCollection, value, () => AddCollection); }
    }

    public RelayCommandAsync EditCollection {
      get { return editCollection; }
      set { SetField(ref editCollection, value, () => EditCollection); }
    }

    public RelayCommandAsync DeleteCollection {
      get { return deleteCollection; }
      set { SetField(ref deleteCollection, value, () => DeleteCollection); }
    }

    #endregion Properties

  }

}
