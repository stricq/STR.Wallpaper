using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewEntities {

  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class CollectionViewEntity : ObservableObject {

    #region Private Fields

    private bool isPublic;
    private bool isSelected;
    private bool isActive;
    private bool isContextMenuOpen;

    private int totalWallpapers;

    private string name;
    private string ownerName;

    private RelayCommand<ContextMenuEventArgs> contextMenuOpening;

    #endregion Private Fields

    #region Properties

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public bool IsActive {
      get { return isActive; }
      set { SetField(ref isActive, value, () => IsActive); }
    }

    public bool IsContextMenuOpen {
      get { return isContextMenuOpen; }
      set { SetField(ref isContextMenuOpen, value, () => IsContextMenuOpen); }
    }

    public bool IsPublic {
      get { return isPublic; }
      set { SetField(ref isPublic, value, () => IsPublic, () => Status); }
    }

    public int TotalWallpapers {
      get { return totalWallpapers; }
      set { SetField(ref totalWallpapers, value, () => TotalWallpapers); }
    }

    public string Name {
      get { return name; }
      set { SetField(ref name, value, () => Name); }
    }

    public string OwnerName {
      get { return ownerName; }
      set { SetField(ref ownerName, value, () => OwnerName); }
    }

    public string Status => isPublic ? "Public" : "Private";

    public RelayCommand<ContextMenuEventArgs> ContextMenuOpening {
      get { return contextMenuOpening; }
      set { SetField(ref contextMenuOpening, value, () => ContextMenuOpening); }
    }

    public string Id { get; set; }

    #endregion Properties

  }

}
