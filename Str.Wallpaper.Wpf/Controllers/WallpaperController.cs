using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;

using AutoMapper;

using Str.Wallpaper.Domain.Contracts;

using Str.Wallpaper.Repository.Models.Settings;

using Str.Wallpaper.Wpf.Messages.Status;
using Str.Wallpaper.Wpf.ViewEntities;
using Str.Wallpaper.Wpf.ViewModels;

using STR.Common.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;


namespace Str.Wallpaper.Wpf.Controllers {

  [Export(typeof(IController))]
  public class WallpaperController : IController {

    #region Private Fields

    private bool isStartupComplete;

    private readonly WallpaperViewModel viewModel;

    private readonly IMessenger messenger;

    private readonly IMapper mapper;

    private readonly IWindowSettingsRepository settingsRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public WallpaperController(WallpaperViewModel ViewModel, IMessenger Messenger, IMapper Mapper, IWindowSettingsRepository SettingsRepository) {
      viewModel = ViewModel;

      messenger = Messenger;
         mapper = Mapper;

      settingsRepository = SettingsRepository;

      viewModel.Settings = mapper.Map<WindowSettingsViewEntity>(Task.Run(() => settingsRepository.LoadWindowSettings()).Result);

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.RegisterAsync<StatusTimerTickMessage>(this, onStatusTimerTick);
    }

    private async Task onStatusTimerTick(StatusTimerTickMessage message) {
      if (viewModel.Settings.AreSettingsChanged && isStartupComplete) {
        await settingsRepository.SaveWindowSettings(mapper.Map<WindowSettings>(viewModel.Settings));

        viewModel.Settings.AreSettingsChanged = false;
      }
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.SizeChanged = new RelayCommand<SizeChangedEventArgs>(onSizeChanged);

      viewModel.Loaded = new RelayCommand<RoutedEventArgs>(onLoaded);
    }

    private void onSizeChanged(SizeChangedEventArgs args) {
      viewModel.Settings.SplitterDistance = args.NewSize.Width + 6;
    }

    private void onLoaded(RoutedEventArgs args) {
      isStartupComplete = true;

      messenger.Send(new ApplicationLoadedMessage());
    }

    #endregion Commands

    #region Private Methods

    #endregion Private Methods

  }

}
