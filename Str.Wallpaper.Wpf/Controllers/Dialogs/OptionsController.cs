using System.ComponentModel.Composition;

using Str.Wallpaper.Wpf.Constants;
using Str.Wallpaper.Wpf.ViewModels;
using Str.Wallpaper.Wpf.ViewModels.Dialogs;

using STR.DialogView.Domain.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;


namespace Str.Wallpaper.Wpf.Controllers.Dialogs {

  [Export(typeof(IController))]
  public sealed class OptionsController : IController {

    #region Private Fields

    private readonly  OptionsViewModel viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public OptionsController(OptionsViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger) {
          viewModel = ViewModel;
      menuViewModel = MenuViewModel;

      messenger = Messenger;

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {

    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.Cancel = new RelayCommand(onCancelExecute);
      viewModel.Save   = new RelayCommand(onSaveExecute);

      menuViewModel.Options = new RelayCommand(onOptionsExecute);
    }

    #region Options Command

    private void onOptionsExecute() {
      messenger.Send(new OpenDialogMessage { Name = DialogNames.Options });
    }

    #endregion Options Command

    #region Cancel Command

    private void onCancelExecute() {
      messenger.Send(new CloseDialogMessage());
    }

    private void onSaveExecute() {
      messenger.Send(new CloseDialogMessage());
    }

    #endregion Cancel Command

    #endregion Commands

  }

}
