using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
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

    private readonly StatusBarViewModel viewModel;

    private readonly TimeSpan oneSecond = TimeSpan.FromSeconds(1);

    private readonly IMessenger messenger;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public StatusBarController(StatusBarViewModel ViewModel, IMessenger Messenger) {
      viewModel = ViewModel;

      messenger = Messenger;

      DispatcherTimer timer = new DispatcherTimer();

      timer.Tick    += onTimerTick;
      timer.Interval = oneSecond;

      timer.Start();

      viewModel.Icon            = FontAwesomeIcon.Pause;
      viewModel.JobProgressText = "Idle";
      viewModel.NextChange      = TimeSpan.FromMinutes(15);

      changeMinutes = 15;

      registerMessages();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<ApplicationSettingsChangedMessage>(this, onApplicationSettingsChanged);
    }

    private void onApplicationSettingsChanged(ApplicationSettingsChangedMessage message) {
      if (changeMinutes != message.Settings.ChangeMinutes) {
        changeMinutes = message.Settings.ChangeMinutes;

        viewModel.NextChange = TimeSpan.FromMinutes(changeMinutes);
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
