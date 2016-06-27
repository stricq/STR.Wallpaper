using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Threading;

using FontAwesome.WPF;

using Str.Wallpaper.Wpf.Messages.Status;
using Str.Wallpaper.Wpf.ViewModels;

using STR.Common.Extensions;
using STR.MvvmCommon.Contracts;


namespace Str.Wallpaper.Wpf.Controllers {

  [Export(typeof(IController))]
  public class StatusBarController : IController {

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

    }

    #endregion Messages

    #region Private Methods

    private async void onTimerTick(object sender, EventArgs e) {
      await messenger.SendAsync(new StatusTimerTickMessage());

      using(Process process = Process.GetCurrentProcess()) {
        viewModel.Memory = process.WorkingSet64 / 1024.0 / 1024.0;
      }

      if (changeMinutes > 0) {
        viewModel.NextChange = viewModel.NextChange.Subtract(oneSecond);

        if (viewModel.NextChange.TotalSeconds.EqualInPercentRange(0.0)) {
          viewModel.NextChange = TimeSpan.FromMinutes(changeMinutes);

          await messenger.SendAsync(new StatusChangeWallpaperMessage());
        }
      }

    }

    #endregion Private Methods

  }

}
