using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using AutoMapper;

using Ookii.Dialogs.Wpf;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Repository.Models.Settings;

using Str.Wallpaper.Wpf.Constants;
using Str.Wallpaper.Wpf.Messages.Application;
using Str.Wallpaper.Wpf.ViewEntities;
using Str.Wallpaper.Wpf.ViewModels;
using Str.Wallpaper.Wpf.ViewModels.Dialogs;

using STR.Common.Messages;

using STR.DialogView.Domain.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;


namespace Str.Wallpaper.Wpf.Controllers.Dialogs {

  [Export(typeof(IController))]
  public sealed class OptionsController : IController {

    #region Private Fields

    private DomainUserSettings userSettings;

    private ProgramSettingsViewEntity settingsBackup;

    private readonly  OptionsViewModel viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly IProgramSettingsRepository settingsRepository;
    private readonly IUserSettingsRepository        userRepository;

    private readonly IUserSessionService sessionService;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public OptionsController(OptionsViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IMapper Mapper, IProgramSettingsRepository SettingsRepository, IUserSettingsRepository UserRepository, IUserSessionService SessionService) {
          viewModel = ViewModel;
      menuViewModel = MenuViewModel;

      messenger = Messenger;
      mapper    = Mapper;

      settingsRepository = SettingsRepository;
          userRepository =     UserRepository;

      sessionService = SessionService;
    }

    public int InitializePriority { get; } = 900;

    public async Task InitializeAsync() {
      registerMessages();
      registerCommands();

      viewModel.Settings = mapper.Map<ProgramSettingsViewEntity>(await settingsRepository.LoadProgramSettingsAsync());

      messenger.Send(new ApplicationSettingsChangedMessage { Settings = viewModel.Settings });

      userSettings = new DomainUserSettings(userRepository, sessionService);

      await userSettings.LoadUserSettingsAsync();

      if (userSettings.Username != null) {
        await userSettings.LoginAsync();

        messenger.Send(new UserSettingsChangedMessage { UserSettings = userSettings });
      }

      viewModel.User = mapper.Map<UserSettingsViewEntity>(userSettings);
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<ApplicationClosingMessage>(this, onApplicationClosing);
    }

    private void onApplicationClosing(ApplicationClosingMessage message) {
      Task.Run(() => userSettings.DisconnectAsync()).Wait();
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.Cancel = new RelayCommand(onCancelExecute);
      viewModel.Save   = new RelayCommandAsync(onSaveExecute);

      viewModel.ServerLogin      = new RelayCommandAsync(onServerLoginExecute);
      viewModel.ServerDisconnect = new RelayCommandAsync(onServerDisconnectExecute);

      viewModel.SelectCacheDirectory = new RelayCommand(onSelectCacheDirectoryExecute);

      menuViewModel.Options = new RelayCommand(onOptionsExecute);
    }

    #region Options Command

    private void onOptionsExecute() {
      settingsBackup = mapper.Map<ProgramSettingsViewEntity>(viewModel.Settings);

      messenger.Send(new OpenDialogMessage { Name = DialogNames.Options });
    }

    #endregion Options Command

    #region SelectCacheDirectory Command

    private void onSelectCacheDirectoryExecute() {
      VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog {
        Description  = "Select a directory to cache images from the server",
        RootFolder   = Environment.SpecialFolder.MyComputer,
        SelectedPath = viewModel.Settings.ImageCacheDirectory,
        ShowNewFolderButton = true
      };

      bool? result = fbd.ShowDialog();

      if (result.HasValue && result.Value) viewModel.Settings.ImageCacheDirectory = fbd.SelectedPath;
    }

    #endregion SelectCacheDirectory Command

    #region Cancel Command

    private void onCancelExecute() {
      viewModel.Settings = settingsBackup;

      viewModel.User = mapper.Map<UserSettingsViewEntity>(userSettings);

      messenger.Send(new CloseDialogMessage());
    }

    #endregion Cancel Command

    #region Save Command

    private async Task onSaveExecute() {
      settingsBackup = null;

      messenger.Send(new CloseDialogMessage());

      if (viewModel.Settings.AreSettingsChanged) {
        await settingsRepository.SaveProgramSettingsAsync(mapper.Map<ProgramSettings>(viewModel.Settings));

        viewModel.Settings.AreSettingsChanged = false;

        messenger.Send(new ApplicationSettingsChangedMessage { Settings = viewModel.Settings });
      }

      if (viewModel.User.AreSettingsChanged) {
        mapper.Map(viewModel.User, userSettings);

        viewModel.User.AreSettingsChanged = false;

        await userSettings.SaveUserSettingsAsync();
      }
    }

    #endregion Save Command

    #region ServerLogin Command

    private async Task onServerLoginExecute() {
      if (viewModel.User.AreSettingsChanged) {
        mapper.Map(viewModel.User, userSettings);

        await userRepository.SaveUserSettingsAsync(userSettings);

        viewModel.User.AreSettingsChanged = false;
      }

      if (await userSettings.LoginAsync()) {
        mapper.Map(userSettings, viewModel.User);

        messenger.Send(new UserSettingsChangedMessage { UserSettings = userSettings });
      }
      else {
        messenger.Send(new MessageBoxDialogMessage { Header = "Username Not Found", Message = "The Username was not found on the server or the password was incorrect.\n\nClick on 'Create User' to create a new user.",  OkText = "Create User", CancelText = "Cancel", HasCancel = true, Callback = onCreateUserResponse });
      }
    }

    private async void onCreateUserResponse(MessageBoxDialogMessage message) {
      if (message.IsCancel) return;

      try {
        if (await userSettings.CreateUserAsync()) return;
      }
      catch(Exception ex) {
        messenger.SendUi(new ApplicationErrorMessage { HeaderText = "Session Service Error", Exception = ex, OpenErrorWindow = true });

        return;
      }

      messenger.Send(new MessageBoxDialogMessage { Header = "Username Already Exists", Message = "The Username already exists on the server.", OkText = "OK", HasCancel = false });
    }

    #endregion ServerLogin Command

    #region ServerDisconnect Command

    private async Task onServerDisconnectExecute() {
      await userSettings.DisconnectAsync();

      mapper.Map(userSettings, viewModel.User);

      messenger.Send(new UserSettingsChangedMessage { UserSettings = userSettings });
    }

    #endregion ServerDisconnect Command

    #endregion Commands

  }

}
