﻿using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
      if (Application.Current != null) Application.Current.DispatcherUnhandledException += onCurrentDispatcherUnhandledException;

      AppDomain.CurrentDomain.UnhandledException += onDomainUnhandledException;

      Dispatcher.CurrentDispatcher.UnhandledException += onCurrentDispatcherUnhandledException;

      TaskScheduler.UnobservedTaskException += onUnobservedTaskException;

      System.Windows.Forms.Application.ThreadException += onThreadException;

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

      viewModel.Loaded  = new RelayCommand<RoutedEventArgs>(onLoaded);
      viewModel.Closing = new RelayCommand<CancelEventArgs>(onClosing);
    }

    private void onSizeChanged(SizeChangedEventArgs args) {
      viewModel.Settings.SplitterDistance = args.NewSize.Width + 6;
    }

    private void onLoaded(RoutedEventArgs args) {
      isStartupComplete = true;

      messenger.Send(new ApplicationLoadedMessage());
    }

    private void onClosing(CancelEventArgs args) {
      ApplicationClosingMessage message = new ApplicationClosingMessage();

      messenger.Send(message);

      args.Cancel = message.Cancel;
    }

    #endregion Commands

    #region Private Methods

    private void onDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
      Exception ex = e.ExceptionObject as Exception;

      if (ex != null) {
        if (e.IsTerminating) MessageBox.Show(ex.Message, "Fatal Domain Unhandled Exception");
        else messenger.SendUi(new ApplicationErrorMessage { HeaderText = "Domain Unhandled Exception", Exception = ex });
      }
    }

    private void onCurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
      if (e.Exception != null) {
        if (isStartupComplete) {
          messenger.SendUi(new ApplicationErrorMessage { HeaderText = "Dispatcher Unhandled Exception", Exception = e.Exception });

          e.Handled = true;
        }
        else {
          e.Handled = true;

          MessageBox.Show(e.Exception.Message, "Fatal Dispatcher Exception");

          Application.Current.Shutdown();
        }
      }
    }

    private void onUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
      if (e.Exception == null || e.Exception.InnerExceptions.Count == 0) return;

      foreach(Exception ex in e.Exception.InnerExceptions) {
        if (isStartupComplete) {
          messenger.SendUi(new ApplicationErrorMessage { HeaderText = "Unobserved Task Exception", Exception = ex });
        }
        else {
          MessageBox.Show(ex.Message, "Fatal Unobserved Task Exception");
        }
      }

      if (!isStartupComplete) Application.Current.Shutdown();

      e.SetObserved();
    }

    private void onThreadException(object sender, ThreadExceptionEventArgs e) {
      if (e.Exception == null) return;

      messenger.SendUi(new ApplicationErrorMessage { HeaderText = "Thread Exception", Exception = e.Exception });
    }

    #endregion Private Methods

  }

}
