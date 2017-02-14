using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Str.Wallpaper.Wpf.Constants;
using Str.Wallpaper.Wpf.Messages.Collections;
using Str.Wallpaper.Wpf.ViewModels.Dialogs;

using STR.DialogView.Domain.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;


namespace Str.Wallpaper.Wpf.Controllers.Dialogs {

  [Export(typeof(IController))]
  public sealed class CollectionEditorController : IController {

    #region Private Fields

    private readonly CollectionEditorViewModel viewModel;

    private readonly IMessenger messenger;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public CollectionEditorController(CollectionEditorViewModel ViewModel, IMessenger Messenger) {
      viewModel = ViewModel;

      messenger = Messenger;
    }

    #endregion Constructor

    #region IController Implementation

    public int InitializePriority { get; } = 100;

    public async Task InitializeAsync() {
      registerMessages();
      registerCommands();

      await Task.CompletedTask;
    }

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.RegisterAsync<CollectionEditMessage>(this, onCollectionEditAsync);
    }

    private async Task onCollectionEditAsync(CollectionEditMessage message) {
      viewModel.Message    = message;
      viewModel.Collection = message.Collection;

      messenger.Send(new OpenDialogMessage { Name = DialogNames.CollectionEditor });

      await Task.CompletedTask;
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.Ok     = new RelayCommandAsync(onOkExecuteAsync);
      viewModel.Cancel = new RelayCommandAsync(onCancelExecuteAsync);
    }

    #region OK Command

    private async Task onOkExecuteAsync() {
      messenger.Send(new CloseDialogMessage());

      if (viewModel.Message.CallbackAsync != null) {
        await viewModel.Message.CallbackAsync(viewModel.Message);

        viewModel.Message.CallbackAsync = null;
      }
    }

    #endregion OK Command

    #region Cancel Command

    private async Task onCancelExecuteAsync() {
      messenger.Send(new CloseDialogMessage());

      if (viewModel.Message.CallbackAsync != null) {
        viewModel.Message.IsCancel = true;

        await viewModel.Message.CallbackAsync(viewModel.Message);

        viewModel.Message.CallbackAsync = null;
      }
    }

    #endregion Cancel Command

    #endregion Commands

  }

}
