using System.ComponentModel.Composition;
using System.Windows.Controls;

using Str.Wallpaper.Wpf.Constants;

using STR.DialogView.Domain.Contracts;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.Views.Dialogs {

  [Export(typeof(IDialogViewLocator))]
  [ViewTag(Name = DialogNames.Options)]
  public sealed partial class OptionsView : UserControl, IDialogViewLocator {

    public OptionsView() {
      InitializeComponent();
    }

  }

}
