using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

using GongSolutions.Wpf.DragDrop;

using Str.Wallpaper.Wpf.ViewEntities;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewModels {

  [Export]
  [ViewModel("FolderTreeViewModel")]
  public sealed class FolderTreeViewModel : ObservableObject {

    #region Private Fields

    private bool isTreeViewEnabled;
    private bool isRemoveVisible;
    private bool isRenameVisible;

    private string collectionName;
    private string removeHeader;
    private string renameHeader;

    private ObservableCollection<FolderViewEntity> folders;

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

    private RelayCommand<ContextMenuEventArgs> contextMenuOpening;
    private RelayCommand<ContextMenuEventArgs> contextMenuClosing;

    private RelayCommandAsync selectRandom;
    private RelayCommandAsync setWallpaper;

    private RelayCommand<KeyEventArgs> previewKeyDown;

    private IDragSource dragSource;
    private IDropTarget dropTarget;

    #endregion Private Fields

    #region Properties

    public bool IsTreeViewEnabled {
      get { return isTreeViewEnabled; }
      set { SetField(ref isTreeViewEnabled, value, () => IsTreeViewEnabled); }
    }

    public bool IsRemoveVisible {
      get { return isRemoveVisible; }
      set { SetField(ref isRemoveVisible, value, () => IsRemoveVisible, () => IsRemoveRenameSeparatorVisible); }
    }

    public bool IsRenameVisible {
      get { return isRenameVisible; }
      set { SetField(ref isRenameVisible, value, () => IsRenameVisible, () => IsRemoveRenameSeparatorVisible); }
    }

    public bool IsRemoveRenameSeparatorVisible => isRemoveVisible || isRenameVisible;

    public string CollectionName {
      get { return collectionName; }
      set { SetField(ref collectionName, value, () => CollectionName); }
    }

    public string RemoveHeader {
      get { return removeHeader; }
      set { SetField(ref removeHeader, value, () => RemoveHeader); }
    }

    public string RenameHeader {
      get { return renameHeader; }
      set { SetField(ref renameHeader, value, () => RenameHeader); }
    }

    public ObservableCollection<FolderViewEntity> Folders {
      get { return folders; }
      set { SetField(ref folders, value, () => Folders); }
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

    public RelayCommand<ContextMenuEventArgs> ContextMenuOpening {
      get { return contextMenuOpening; }
      set { SetField(ref contextMenuOpening, value, () => ContextMenuOpening); }
    }

    public RelayCommand<ContextMenuEventArgs> ContextMenuClosing {
      get { return contextMenuClosing; }
      set { SetField(ref contextMenuClosing, value, () => ContextMenuClosing); }
    }

    public RelayCommandAsync SelectRandom {
      get { return selectRandom; }
      set { SetField(ref selectRandom, value, () => SelectRandom); }
    }

    public RelayCommandAsync SetWallpaper {
      get { return setWallpaper; }
      set { SetField(ref setWallpaper, value, () => SetWallpaper); }
    }

    public RelayCommand<KeyEventArgs> PreviewKeyDown {
      get { return previewKeyDown; }
      set { SetField(ref previewKeyDown, value, () => PreviewKeyDown); }
    }

    public IDragSource DragSource {
      get { return dragSource; }
      set { SetField(ref dragSource, value, () => DragSource); }
    }

    public IDropTarget DropTarget {
      get { return dropTarget; }
      set { SetField(ref dropTarget, value, () => DropTarget); }
    }

    #endregion Properties

  }

}
