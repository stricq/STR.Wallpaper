using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using AutoMapper;

using Ookii.Dialogs.Wpf;

using Str.Wallpaper.Domain.Contracts;

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

    private ProgramSettingsViewEntity settingsBackup;

    private readonly  OptionsViewModel viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly IProgramSettingsRepository settingsRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public OptionsController(OptionsViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IMapper Mapper, IProgramSettingsRepository SettingsRepository) {
          viewModel = ViewModel;
      menuViewModel = MenuViewModel;

      messenger = Messenger;
      mapper    = Mapper;

      settingsRepository = SettingsRepository;

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<ApplicationLoadedMessage>(this, onApplicationLoaded);
    }

    private void onApplicationLoaded(ApplicationLoadedMessage message) {
      viewModel.Settings = mapper.Map<ProgramSettingsViewEntity>(Task.Run(() => settingsRepository.LoadProgramSettingsAsync()).Result);

      messenger.Send(new ApplicationSettingsChangedMessage { Settings = viewModel.Settings });
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.Cancel = new RelayCommand(onCancelExecute);
      viewModel.Save   = new RelayCommandAsync(onSaveExecute);

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

      messenger.Send(new CloseDialogMessage());
    }

    #endregion Cancel Command

    #region Save Command

    private async Task onSaveExecute() {
      messenger.Send(new CloseDialogMessage());

      await settingsRepository.SaveProgramSettingsAsync(mapper.Map<ProgramSettings>(viewModel.Settings));
    }

    #endregion Save Command

    #endregion Commands

  }

}
