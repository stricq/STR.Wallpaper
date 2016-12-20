using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;

using FontAwesome.WPF;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewModels {

  [Export]
  [ViewModel("StatusBarViewModel")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class StatusBarViewModel : ObservableObject {

    #region Private Fields

    private bool spin;

    private double memory;

    private string jobProgressText;
    private string      statusText;

    private SolidColorBrush iconColor;

    private FontAwesomeIcon icon;

    private TimeSpan nextChange;

    #endregion Private Fields

    #region Properties

    public bool Spin {
      get { return spin; }
      set { SetField(ref spin, value, () => Spin); }
    }

    public double Memory {
      get { return memory; }
      set { SetField(ref memory, value, () => Memory); }
    }

    public string JobProgressText {
      get { return jobProgressText; }
      set { SetField(ref jobProgressText, value, () => JobProgressText); }
    }

    public string StatusText {
      get { return statusText; }
      set { SetField(ref statusText, value, () => StatusText); }
    }

    public SolidColorBrush IconColor {
      get { return iconColor; }
      set { SetField(ref iconColor, value, () => IconColor); }
    }

    public FontAwesomeIcon Icon {
      get { return icon; }
      set { SetField(ref icon, value, () => Icon); }
    }

    public TimeSpan NextChange {
      get { return nextChange; }
      set { SetField(ref nextChange, value, () => NextChange); }
    }

    #endregion Properties

  }

}
