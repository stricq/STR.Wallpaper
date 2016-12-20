using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;

using GongSolutions.Wpf.DragDrop;

using Str.Wallpaper.Wpf.ViewEntities;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewModels {

  [Export]
  [ViewModel("CollectionsViewModel")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class CollectionsViewModel : ObservableObject {

    #region Private Fields

    private bool isEnabled;

    private string editHeader;
    private string deleteHeader;

    private ObservableCollection<CollectionViewEntity> collections;

    private RelayCommand    addCollection;
    private RelayCommand   editCollection;
    private RelayCommand deleteCollection;

    private RelayCommand<ContextMenuEventArgs> contextMenuOpening;
    private RelayCommand<ContextMenuEventArgs> contextMenuClosing;

    private IDropTarget dropTarget;

    #endregion Private Fields

    #region Properties

    public RelayCommand AddCollection {
      get { return addCollection; }
      set { SetField(ref addCollection, value, () => AddCollection); }
    }

    public RelayCommand EditCollection {
      get { return editCollection; }
      set { SetField(ref editCollection, value, () => EditCollection); }
    }

    public RelayCommand DeleteCollection {
      get { return deleteCollection; }
      set { SetField(ref deleteCollection, value, () => DeleteCollection); }
    }

    public RelayCommand<ContextMenuEventArgs> ContextMenuOpening {
      get { return contextMenuOpening; }
      set { SetField(ref contextMenuOpening, value, () => ContextMenuOpening); }
    }

    public RelayCommand<ContextMenuEventArgs> ContextMenuClosing {
      get { return contextMenuClosing; }
      set { SetField(ref contextMenuClosing, value, () => ContextMenuClosing); }
    }

    public bool IsEnabled {
      get { return isEnabled; }
      set { SetField(ref isEnabled, value, () => IsEnabled); }
    }

    public string EditHeader {
      get { return editHeader; }
      set { SetField(ref editHeader, value, () => EditHeader); }
    }

    public string DeleteHeader {
      get { return deleteHeader; }
      set { SetField(ref deleteHeader, value, () => DeleteHeader); }
    }

    public ObservableCollection<CollectionViewEntity> Collections {
      get { return collections; }
      set { SetField(ref collections, value, () => Collections); }
    }

    public IDropTarget DropTarget {
      get { return dropTarget; }
      set { SetField(ref dropTarget, value, () => DropTarget); }
    }

    #endregion Properties

  }

}
