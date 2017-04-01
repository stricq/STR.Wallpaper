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

    private bool isRemoveFolderVisible;
    private bool isRenameFolderVisible;

    private string editCollectionHeader;
    private string deleteCollectionHeader;

    private string removeFolderHeader;
    private string renameFolderHeader;

    private RelayCommand exit;
    private RelayCommand options;

    private RelayCommandAsync addCollection;
    private RelayCommandAsync editCollection;
    private RelayCommandAsync deleteCollection;

    private RelayCommandAsync addFolder;
    private RelayCommandAsync addImages;
    private RelayCommandAsync addDirectory;

    private RelayCommandAsync removeFolder;
    private RelayCommandAsync renameFolder;

    private RelayCommand collapseAll;
    private RelayCommand expandAll;

    private RelayCommand viewLoadError;
    private RelayCommand clearError;
    private RelayCommand clearAllErrors;

    #endregion Private Fields

    #region Properties

    public bool IsRemoveFolderVisible {
      get { return isRemoveFolderVisible; }
      set { SetField(ref isRemoveFolderVisible, value, () => IsRemoveFolderVisible, () => IsRemoveRenameSeparatorVisible); }
    }

    public bool IsRenameFolderVisible {
      get { return isRenameFolderVisible; }
      set { SetField(ref isRenameFolderVisible, value, () => IsRenameFolderVisible, () => IsRemoveRenameSeparatorVisible); }
    }

    public bool IsRemoveRenameSeparatorVisible => isRemoveFolderVisible || isRenameFolderVisible;

    public string EditCollectionHeader {
      get { return editCollectionHeader; }
      set { SetField(ref editCollectionHeader, value, () => EditCollectionHeader); }
    }

    public string DeleteCollectionHeader {
      get { return deleteCollectionHeader; }
      set { SetField(ref deleteCollectionHeader, value, () => DeleteCollectionHeader); }
    }

    public string RemoveFolderHeader {
      get { return removeFolderHeader; }
      set { SetField(ref removeFolderHeader, value, () => RemoveFolderHeader); }
    }

    public string RenameFolderHeader {
      get { return renameFolderHeader; }
      set { SetField(ref renameFolderHeader, value, () => RenameFolderHeader); }
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

    public RelayCommandAsync AddFolder {
      get { return addFolder; }
      set { SetField(ref addFolder, value, () => AddFolder); }
    }

    public RelayCommandAsync AddImages {
      get { return addImages; }
      set { SetField(ref addImages, value, () => AddImages); }
    }

    public RelayCommandAsync AddDirectory {
      get { return addDirectory; }
      set { SetField(ref addDirectory, value, () => AddDirectory); }
    }

    public RelayCommandAsync RemoveFolder {
      get { return removeFolder; }
      set { SetField(ref removeFolder, value, () => RemoveFolder); }
    }

    public RelayCommandAsync RenameFolder {
      get { return renameFolder; }
      set { SetField(ref renameFolder, value, () => RenameFolder); }
    }

    public RelayCommand CollapseAll {
      get { return collapseAll; }
      set { SetField(ref collapseAll, value, () => CollapseAll); }
    }

    public RelayCommand ExpandAll {
      get { return expandAll; }
      set { SetField(ref expandAll, value, () => ExpandAll); }
    }

    public RelayCommand ViewLoadError {
      get { return viewLoadError; }
      set { SetField(ref viewLoadError, value, () => ViewLoadError); }
    }

    public RelayCommand ClearError {
      get { return clearError; }
      set { SetField(ref clearError, value, () => ClearError); }
    }

    public RelayCommand ClearAllErrors {
      get { return clearAllErrors; }
      set { SetField(ref clearAllErrors, value, () => ClearAllErrors); }
    }

    #endregion Properties

  }

}
