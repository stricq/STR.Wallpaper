using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;

using FontAwesome.WPF;

using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Repository.Models.Dtos;

using STR.Common.Contracts;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewEntities {

  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public sealed class FolderViewEntity : ObservableObject, ITraversable<FolderViewEntity>, IComparable<FolderViewEntity>, IDeepCloneable {

    #region Private Fields

    private bool isChecked;
    private bool isContextMenuOpen;
    private bool isExpanded;
    private bool isMultiSelected;
    private bool isSelected;

    private bool hasError;

    private string name;

    private ObservableCollection<FolderViewEntity> children;

    private FolderType folderType;

    #endregion Private Fields

    #region Properties

    public string Id { get; set; }

    public bool IsChecked {
      get { return isChecked; }
      set { SetField(ref isChecked, value, () => IsChecked); }
    }

    public bool IsCollectionFolder { get; set; }

    public bool IsContextMenuOpen {
      get { return isContextMenuOpen; }
      set { SetField(ref isContextMenuOpen, value, () => IsContextMenuOpen); }
    }

    public bool IsExpanded {
      get { return isExpanded; }
      set { SetField(ref isExpanded, value, () => IsExpanded, () => Icon); }
    }

    public bool IsMultiSelected {
      get { return isMultiSelected; }
      set { SetField(ref isMultiSelected, value, () => IsMultiSelected); }
    }

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public bool HasError {
      get { return hasError; }
      set { SetField(ref hasError, value, () => HasError); }
    }

    public string Name {
      get { return name; }
      set { SetField(ref name, value, () => Name); }
    }

    public FolderType FolderType {
      get { return folderType; }
      set { SetField(ref folderType, value, () => FolderType, () => Icon, () => IconColor); }
    }

    public FontAwesomeIcon Icon => FolderType == FolderType.Image  ? FontAwesomeIcon.Image :
                                   FolderType == FolderType.Folder ? (IsExpanded ? FontAwesomeIcon.FolderOpen : FontAwesomeIcon.Folder) : FontAwesomeIcon.RssSquare;

    public SolidColorBrush IconColor => FolderType == FolderType.Image  ? new SolidColorBrush(Colors.LawnGreen) :
                                        FolderType == FolderType.Folder ? new SolidColorBrush(Colors.DarkOrange) : new SolidColorBrush(Colors.Orange);

    public DomainFolder Folder { get; set; }

    public FolderViewEntity Parent { get; set; }

    #endregion Properties

    #region ITraversable Implementation

    public ObservableCollection<FolderViewEntity> Children {
      get { return children; }
      set { SetField(ref children, value, () => Children); }
    }

    #endregion ITraversable Implementation

    #region IComparable Implementation

    public int CompareTo(FolderViewEntity other) {
      int compared = FolderType.CompareTo(other.FolderType);

      if (compared == 0) compared = String.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);

      return compared;
    }

    #endregion IComparable Implementation

    #region Overrides

    public override string ToString() {
      return Name;
    }

    #endregion Overrides

  }

}
