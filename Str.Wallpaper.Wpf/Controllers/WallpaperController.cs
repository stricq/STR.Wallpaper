using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using AutoMapper;

using Str.Wallpaper.Domain.Contracts;

using Str.Wallpaper.Repository.Models.Settings;

using Str.Wallpaper.Wpf.Messages.Application;
using Str.Wallpaper.Wpf.Messages.Status;
using Str.Wallpaper.Wpf.ViewEntities;
using Str.Wallpaper.Wpf.ViewModels;

using STR.Common.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;


namespace Str.Wallpaper.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class WallpaperController : IController {

    #region Private Fields

    private bool isStartupComplete;

    private readonly WallpaperViewModel      viewModel;
    private readonly MainMenuViewModel   menuViewModel;
    private readonly NotifyIconViewModel iconViewModel;

    private readonly IMessenger messenger;

    private readonly IMapper mapper;

    private readonly IWindowSettingsRepository settingsRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public WallpaperController(WallpaperViewModel ViewModel, MainMenuViewModel MenuViewModel, NotifyIconViewModel IconViewModel, IMessenger Messenger, IMapper Mapper, IWindowSettingsRepository SettingsRepository) {
      if (Application.Current != null) Application.Current.DispatcherUnhandledException += onCurrentDispatcherUnhandledException;

      AppDomain.CurrentDomain.UnhandledException += onDomainUnhandledException;

      Dispatcher.CurrentDispatcher.UnhandledException += onCurrentDispatcherUnhandledException;

      TaskScheduler.UnobservedTaskException += onUnobservedTaskException;

      System.Windows.Forms.Application.ThreadException += onThreadException;

          viewModel =     ViewModel;
      menuViewModel = MenuViewModel;
      iconViewModel = IconViewModel;

      messenger = Messenger;
         mapper = Mapper;

      settingsRepository = SettingsRepository;
    }

    #endregion Constructor

    #region IController Implementation

    public int InitializePriority { get; } = 1000;

    public async Task InitializeAsync() {
      iconViewModel.TooltipText = "STR Wallpaper v7";

      viewModel.Settings = mapper.Map<WindowSettingsViewEntity>(await settingsRepository.LoadWindowSettingsAsync());

      if (viewModel.Settings.MainWindowState == WindowState.Minimized || viewModel.Settings.IsStartMinimized) {
        viewModel.Settings.MainWindowState = WindowState.Minimized;

        viewModel.MainWindowVisibility = Visibility.Hidden;
      }
      else {
        viewModel.MainWindowVisibility = Visibility.Visible;
        viewModel.ShowInTaskbar        = true;
      }

      viewModel.Settings.PropertyChanged += onSettingsPropertyChanged;

      registerMessages();
      registerCommands();
    }

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.RegisterAsync<StatusTimerTickMessage>(this, onStatusTimerTick);

      messenger.Register<ApplicationSettingsChangedMessage>(this, onApplicationSettingsChanged);
    }

    private async Task onStatusTimerTick(StatusTimerTickMessage message) {
      if (viewModel.Settings.AreSettingsChanged && isStartupComplete) {
        viewModel.Settings.AreSettingsChanged = false;

        await saveSettings();
      }
    }

    private void onApplicationSettingsChanged(ApplicationSettingsChangedMessage message) {
      viewModel.Settings.IsStartMinimized = message.Settings.IsStartMinimized;
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.SizeChanged = new RelayCommand<SizeChangedEventArgs>(onSizeChanged);

      viewModel.Initialized = new RelayCommand<EventArgs>(onInitialized);
      viewModel.Loaded      = new RelayCommandAsync<RoutedEventArgs>(onLoaded);
      viewModel.Closing     = new RelayCommand<CancelEventArgs>(onClosing);

      menuViewModel.Exit = new RelayCommand(onExit);

      iconViewModel.Exit        = new RelayCommand(onExit);
      iconViewModel.DoubleClick = new RelayCommand(onDoubleClick);
    }

    private void onSizeChanged(SizeChangedEventArgs args) {
      viewModel.Settings.SplitterDistance = args.NewSize.Width + 6;
    }

    private void onInitialized(EventArgs args) {
      isStartupComplete = true;

      messenger.Send(new ApplicationInitializedMessage());
    }

    private async Task onLoaded(RoutedEventArgs args) {
      await messenger.SendAsync(new ApplicationLoadedMessage());
    }

    private void onClosing(CancelEventArgs args) {
      ApplicationClosingMessage message = new ApplicationClosingMessage();

      Task.Run(() => messenger.SendAsync(message)).Wait();

      if (!message.Cancel && viewModel.Settings.AreSettingsChanged) Task.Run(saveSettings).Wait();

      args.Cancel = message.Cancel;
    }

    private void onExit() {
      ApplicationClosingMessage message = new ApplicationClosingMessage();

      messenger.Send(message);

      if (!message.Cancel) {
        if (viewModel.Settings.AreSettingsChanged) Task.Run(saveSettings).Wait();

        Application.Current.Shutdown();
      }
    }

    private void onDoubleClick() {
      if (viewModel.Settings.MainWindowState != WindowState.Minimized) return;

      viewModel.ShowInTaskbar = true;
      viewModel.MainWindowVisibility = Visibility.Visible;
      viewModel.Show();

      viewModel.Settings.MainWindowState = viewModel.Settings.PreMinimizedState;

      viewModel.Activate();
      viewModel.TopMost = true;
      viewModel.TopMost = false;
      viewModel.Focus();
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

    private void onSettingsPropertyChanged(object sender, PropertyChangedEventArgs e) {
      switch(e.PropertyName) {
        case "MainWindowState": {
          if (viewModel.Settings.MainWindowState == WindowState.Minimized) {
            viewModel.MainWindowVisibility = Visibility.Hidden;
            viewModel.ShowInTaskbar        = false;
          }
          else viewModel.Settings.PreMinimizedState = viewModel.Settings.MainWindowState;

          break;
        }
      }
    }

    private async Task saveSettings() {
      await settingsRepository.SaveWindowSettingsAsync(mapper.Map<WindowSettings>(viewModel.Settings));
    }

    #endregion Private Methods

  }

}
