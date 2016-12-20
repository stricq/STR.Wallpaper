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

    private byte rating;

    private int wallpaperCount;

    private string name;
    private string owner;

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

    public byte Rating {
      get { return rating; }
      set { SetField(ref rating, value, () => Rating); }
    }

    public int WallpaperCount {
      get { return wallpaperCount; }
      set { SetField(ref wallpaperCount, value, () => WallpaperCount); }
    }

    public string Name {
      get { return name; }
      set { SetField(ref name, value, () => Name); }
    }

    public string Owner {
      get { return owner; }
      set { SetField(ref owner, value, () => Owner); }
    }

    public string Status => isPublic ? "Public" : "Private";

    public RelayCommand<ContextMenuEventArgs> ContextMenuOpening {
      get { return contextMenuOpening; }
      set { SetField(ref contextMenuOpening, value, () => ContextMenuOpening); }
    }

    #endregion Properties

  }

}
