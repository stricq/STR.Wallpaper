using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using AutoMapper;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Wpf.Messages.Application;
using Str.Wallpaper.Wpf.Messages.Collections;
using Str.Wallpaper.Wpf.Messages.Folders;
using Str.Wallpaper.Wpf.ViewEntities;
using Str.Wallpaper.Wpf.ViewModels;

using STR.Common.Extensions;
using STR.Common.Messages;

using STR.DialogView.Domain.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;


namespace Str.Wallpaper.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class CollectionsController : IController {

    #region Private Fields

    private bool isLongRunningTask;

    private CollectionViewEntity  contextCollection;
    private CollectionViewEntity selectedCollection;

    private DomainUser user;

    private List<DomainCollection> collections;

    private readonly CollectionsViewModel  viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly ICollectionService collectionService;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public CollectionsController(CollectionsViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IMapper Mapper, ICollectionService CollectionService) {
          viewModel = ViewModel;
      menuViewModel = MenuViewModel;

      viewModel.Collections = new ObservableCollection<CollectionViewEntity>();

      collections = new List<DomainCollection>();

      messenger = Messenger;
      mapper    = Mapper;

      collectionService = CollectionService;
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
      messenger.RegisterAsync<ApplicationLoadedMessage>(this, onApplicationLoaded);

      messenger.Register<UserSettingsChangedMessage>(this, onUserSettingsChanged);
    }

    private async Task onApplicationLoaded(ApplicationLoadedMessage message) {
      await loadCollectionsAsync();
    }

    private void onUserSettingsChanged(UserSettingsChangedMessage message) {
      user = message.UserSettings;

      Task.Run(loadCollectionsAsync).FireAndForget();
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.ContextMenuOpening = new RelayCommand<ContextMenuEventArgs>(onContextMenuOpeningExecute);
      viewModel.ContextMenuClosing = new RelayCommand<ContextMenuEventArgs>(onContextMenuClosingExecute);

      viewModel.AddCollection    = new RelayCommandAsync(onAddCollectionExecuteAsync,    canAddCollectionExecute);
      viewModel.EditCollection   = new RelayCommandAsync(onEditCollectionExecuteAsync,   canEditCollectionExecute);
      viewModel.DeleteCollection = new RelayCommandAsync(onDeleteCollectionExecuteAsync, canDeleteCollectionExecute);

      menuViewModel.AddCollection    = viewModel.AddCollection;
      menuViewModel.EditCollection   = viewModel.EditCollection;
      menuViewModel.DeleteCollection = viewModel.DeleteCollection;
    }

    #region ContextMenu Command

    private void onContextMenuOpeningExecute(ContextMenuEventArgs args) {
      if (contextCollection == null) args.Handled = true;
    }

    private void onContextMenuClosingExecute(ContextMenuEventArgs args) {
      if (contextCollection != null) {
        contextCollection.IsContextMenuOpen = false;

        contextCollection = null;
      }
    }

    #endregion ContextMenu Command

    #region AddCollection Command

    private bool canAddCollectionExecute() {
      return user.IsOnline && !isLongRunningTask;
    }

    private async Task onAddCollectionExecuteAsync() {
      await messenger.SendAsync(new CollectionEditMessage { Collection = new DomainCollection(), Title = "Add New Collection", CallbackAsync = onAddCollectionResponseAsync });
    }

    #endregion AddCollection Command

    #region EditCollection Command

    private bool canEditCollectionExecute() {
      return selectedCollection != null && selectedCollection.OwnerId == user.Id && user.IsOnline && !isLongRunningTask;
    }

    private async Task onEditCollectionExecuteAsync() {
      if (selectedCollection == null) return;

      DomainCollection collection = collections.Single(dc => dc.Id == selectedCollection.Id);

      await messenger.SendAsync(new CollectionEditMessage { Collection = collection.DeepCopy(), OriginalCollection = collection, Title = "Edit Collection", CallbackAsync = onEditCollectionResponseAsync });
    }

    #endregion EditCollection Command

    #region DeleteCollection Command

    private bool canDeleteCollectionExecute() {
      return selectedCollection != null && selectedCollection.OwnerId == user.Id && user.IsOnline && !isLongRunningTask;
    }

    private async Task onDeleteCollectionExecuteAsync() {
      string msg = $"Delete: {selectedCollection.Name}\n\nAll wallpapers will also be removed.\n\nAre you sure?";

      await messenger.SendAsync(new MessageBoxDialogMessage<CollectionViewEntity> { Header = "Delete Collection", Message = msg, CallbackAsync = onDeleteCollectionResponseAsync, Payload = selectedCollection });
    }

    #endregion DeleteCollection Command

    #endregion Commands

    #region Private Methods

    private async Task loadCollectionsAsync() {
      if (!user?.IsOnline ?? true) {
        viewModel.Collections.ForEach(c => c.PropertyChanged -= onCollectionViewEntityChanged);

        viewModel.Collections = new ObservableCollection<CollectionViewEntity>();

        viewModel.IsEnabled = false;

        collections.Clear();

        return;
      }

      if (viewModel.IsEnabled) return;

      collections = await collectionService.LoadCollectionsAsync(user);

      await collections.ForEachAsync(c => collectionService.CountWallpapersAsync(user, c));

      viewModel.Collections = new ObservableCollection<CollectionViewEntity>(mapper.Map<List<CollectionViewEntity>>(collections.OrderBy(c => c.OwnerName).ThenBy(c => c.Name)));

      viewModel.Collections.ForEach(c => {
        if (user.SelectedCollections.Contains(c.Id)) c.IsActive = true;

        c.ContextMenuOpening = new RelayCommand<ContextMenuEventArgs>(onViewEntityContextMenuOpening);

        c.PropertyChanged += onCollectionViewEntityChanged;
      });

      await onCollectionSelectedAsync();

      viewModel.IsEnabled = true;
    }

    private void onCollectionViewEntityChanged(object sender, PropertyChangedEventArgs args) {
      CollectionViewEntity collection = sender as CollectionViewEntity;

      if (collection == null) return;

      switch(args.PropertyName) {
        case "IsSelected": {
          if (Mouse.RightButton == MouseButtonState.Pressed) return;

          collection.IsActive = !collection.IsActive;

          if (collection.IsActive) selectedCollection = collection;

          Task.Run(onCollectionSelectedAsync).FireAndForget();

          setMenuHeaders();

          break;
        }
      }
    }

    private void onViewEntityContextMenuOpening(ContextMenuEventArgs args) {
      contextCollection = ((FrameworkElement)args.Source).DataContext as CollectionViewEntity;

      if (contextCollection != null) {
        contextCollection.IsContextMenuOpen = true;

        selectedCollection = contextCollection;
      }

      setMenuHeaders();
    }

    private async Task onCollectionSelectedAsync() {
      List<DomainCollection> selectedCollections = viewModel.Collections.Where(c => c.IsActive).Select(c => collections.Single(dc => dc.Id == c.Id)).ToList();

      user.SelectedCollections = selectedCollections.Select(dc => dc.Id).ToList();
      user.AreSettingsChanged  = true;

      await messenger.SendAsync(new CollectionsChangedMessage { Collections = selectedCollections });
    }

    private async Task onAddCollectionResponseAsync(CollectionEditMessage message) {
      if (message.IsCancel) return;

      if (String.IsNullOrEmpty(message.Collection.Name)) {
        messenger.Send(new MessageBoxDialogMessage { Header = "Missing Name", Message = "Please enter a Collection name.", HasCancel = false });

        return;
      }

      if (collections.Any(collection => collection.OwnerId == user.Id && collection.Name == message.Collection.Name)) {
        messenger.Send(new MessageBoxDialogMessage { Header = "Duplicate Name", Message = "A collection already exists with that name.", HasCancel = false });

        return;
      }

      message.Collection.OwnerId   = user.Id;
      message.Collection.OwnerName = user.Username;
      message.Collection.Created   = DateTime.Now;

      await collectionService.SaveCollectionAsync(user, message.Collection);

      CollectionViewEntity entity = mapper.Map<CollectionViewEntity>(message.Collection);

      viewModel.Collections.OrderedMerge(entity);

      entity.ContextMenuOpening = new RelayCommand<ContextMenuEventArgs>(onViewEntityContextMenuOpening);

      entity.PropertyChanged += onCollectionViewEntityChanged;

      //if (!settings.History.ContainsKey(message.Collection.Id)) {
      //  settings.History[message.Collection.Id] = new List<string>();
      //}

      selectedCollection = entity;

      setMenuHeaders();
    }

    private async Task onEditCollectionResponseAsync(CollectionEditMessage message) {
      if (message.IsCancel) return;

      if (String.IsNullOrEmpty(message.Collection.Name)) {
        messenger.Send(new MessageBoxDialogMessage { Header = "Missing Name", Message = "Please enter a collection name." });

        return;
      }

      if (viewModel.Collections.Any(c => c.OwnerId == user.Id
                                      && c.Name    == message.Collection.Name
                                      && c.Id      != message.OriginalCollection.Id)) {
        messenger.Send(new MessageBoxDialogMessage { Header = "Duplicate Name", Message = "A collection already exists with that name." });

        return;
      }

      DomainCollection collection = message.OriginalCollection;

      collection.Name     = message.Collection.Name;
      collection.IsPublic = message.Collection.IsPublic;

      await collectionService.SaveCollectionAsync(user, collection);
      //
      // Sort in place does not work here as selection information is lost
      //
      CollectionViewEntity entity = viewModel.Collections.Single(cve => cve.Id == collection.Id);

      entity.Name     = collection.Name;
      entity.IsPublic = collection.IsPublic;

      viewModel.Collections.Remove(entity);

      viewModel.Collections.OrderedMerge(entity);

      await onCollectionSelectedAsync();

      setMenuHeaders();
    }

    private async Task onDeleteCollectionResponseAsync(MessageBoxDialogMessage message) {
      if (message.IsCancel) return;

      CollectionViewEntity entity = ((MessageBoxDialogMessage<CollectionViewEntity>)message).Payload;

      DomainCollection collection = collections.Single(dc => dc.Id == entity.Id);

      await collectionService.DeleteCollectionAsync(user, collection);

      viewModel.Collections.Remove(entity);

      collections.Remove(collection);

      Task.Run(onCollectionSelectedAsync).FireAndForget();

      selectedCollection = null;

      setMenuHeaders();
    }

    private void setMenuHeaders() {
      string name = selectedCollection?.Name ?? "...";

      string   editHeader = $"_Edit {name}";
      string deleteHeader = $"_Delete {name}";

      viewModel.EditHeader   = editHeader;
      viewModel.DeleteHeader = deleteHeader;

      menuViewModel.EditCollectionHeader   = editHeader;
      menuViewModel.DeleteCollectionHeader = deleteHeader;
    }

    #endregion Private Methods

  }

}
