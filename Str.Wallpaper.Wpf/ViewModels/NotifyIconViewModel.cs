using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewModels {

  [Export]
  [ViewModel("NotifyIconViewModel")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class NotifyIconViewModel : ObservableObject {

    #region Private Fields

    private string tooltipText;

    private RelayCommand doubleClick;
    private RelayCommand exit;

    #endregion Private Fields

    #region Properties

    public string TooltipText {
      get { return tooltipText; }
      set { SetField(ref tooltipText, value, () => TooltipText); }
    }

    public RelayCommand DoubleClick {
      get { return doubleClick; }
      set { SetField(ref doubleClick, value, () => DoubleClick); }
    }

    public RelayCommand Exit {
      get { return exit; }
      set { SetField(ref exit, value, () => Exit); }
    }

    #endregion Properties

  }

}
