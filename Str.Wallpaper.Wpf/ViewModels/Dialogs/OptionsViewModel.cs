using System.ComponentModel.Composition;

using STR.DialogView.Domain.Contracts;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewModels.Dialogs {

  [Export]
  [ViewModel("OptionsViewModel")]
  public sealed class OptionsViewModel : ObservableObject, IDialogViewModel {

    #region Private Fields

    private RelayCommand cancel;
    private RelayCommand save;

    #endregion Private Fields

    #region Properties

    public RelayCommand Cancel {
      get { return cancel; }
      set { SetField(ref cancel, value, () => Cancel); }
    }

    public RelayCommand Save {
      get { return save; }
      set { SetField(ref save, value, () => Save); }
    }

    #endregion Properties

  }

}
