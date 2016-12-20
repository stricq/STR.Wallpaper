using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

using FontAwesome.WPF;

using Str.Wallpaper.Wpf.Messages.Application;
using Str.Wallpaper.Wpf.Messages.Status;
using Str.Wallpaper.Wpf.ViewModels;

using STR.Common.Extensions;

using STR.MvvmCommon.Contracts;


namespace Str.Wallpaper.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class StatusBarController : IController {

    #region Private Fields

    private int changeMinutes;

    private DispatcherTimer timer;

    private readonly StatusBarViewModel viewModel;

    private readonly TimeSpan oneSecond = TimeSpan.FromSeconds(1);

    private readonly IMessenger messenger;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public StatusBarController(StatusBarViewModel ViewModel, IMessenger Messenger) {
      viewModel = ViewModel;

      messenger = Messenger;

      viewModel.Icon            = FontAwesomeIcon.ExclamationTriangle;
      viewModel.IconColor       = new SolidColorBrush(Colors.Maroon);
      viewModel.JobProgressText = "Offline";
      viewModel.NextChange      = TimeSpan.FromMinutes(15);

      timer = new DispatcherTimer();
    }

    #endregion Constructor

    #region IController Implementation

    public int InitializePriority { get; } = 100;

    public Task InitializeAsync() {
      timer.Tick    += onTimerTick;
      timer.Interval = oneSecond;

      timer.Start();

      changeMinutes = 15;

      registerMessages();

      return Task.CompletedTask;
    }

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.Register<ApplicationSettingsChangedMessage>(this, onApplicationSettingsChanged);

      messenger.Register<UserSettingsChangedMessage>(this, onUserSettingsChanged);
    }

    private void onApplicationSettingsChanged(ApplicationSettingsChangedMessage message) {
      if (changeMinutes != message.Settings.ChangeMinutes) {
        changeMinutes = message.Settings.ChangeMinutes;

        viewModel.NextChange = TimeSpan.FromMinutes(changeMinutes);
      }
    }

    private void onUserSettingsChanged(UserSettingsChangedMessage message) {
      if (message.UserSettings.SessionId == null) {
        if (message.UserSettings.IsLoggingIn) {
          viewModel.Icon            = FontAwesomeIcon.Spinner;
          viewModel.IconColor       = new SolidColorBrush(Colors.Yellow);
          viewModel.Spin            = true;
          viewModel.JobProgressText = "Logging In";
        }
        else {
          viewModel.Icon            = FontAwesomeIcon.ExclamationTriangle;
          viewModel.IconColor       = new SolidColorBrush(Colors.Maroon);
          viewModel.Spin            = false;
          viewModel.JobProgressText = "Offline";
        }
      }
      else {
        viewModel.Icon            = FontAwesomeIcon.Pause;
        viewModel.IconColor       = new SolidColorBrush(Colors.White);
        viewModel.Spin            = false;
        viewModel.JobProgressText = "Idle";
      }
    }

    #endregion Messages

    #region Private Methods

    private void onTimerTick(object sender, EventArgs e) {
      messenger.SendAsync(new StatusTimerTickMessage()).FireAndForget();

      using(Process process = Process.GetCurrentProcess()) {
        viewModel.Memory = process.WorkingSet64 / 1024.0 / 1024.0;
      }

      if (changeMinutes > 0) {
        viewModel.NextChange = viewModel.NextChange.Subtract(oneSecond);

        if (viewModel.NextChange.TotalSeconds.EqualInPercentRange(0.0)) {
          viewModel.NextChange = TimeSpan.FromMinutes(changeMinutes);

          messenger.SendAsync(new StatusChangeWallpaperMessage()).FireAndForget();
        }
      }

    }

    #endregion Private Methods

  }

}
