using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Wpf.Messages.Application;
using Str.Wallpaper.Wpf.Messages.Folders;
using Str.Wallpaper.Wpf.ViewEntities;
using Str.Wallpaper.Wpf.ViewModels;

using STR.Common.Extensions;
using STR.Common.Messages;

using STR.MvvmCommon.Contracts;


namespace Str.Wallpaper.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class CollectionsController : IController {

    #region Private Fields

    private DomainUser user;

    private List<DomainCollection> collections;

    private readonly CollectionsViewModel viewModel;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly ICollectionService collectionService;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public CollectionsController(CollectionsViewModel ViewModel, IMessenger Messenger, IMapper Mapper, ICollectionService CollectionService) {
      viewModel = ViewModel;

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
        if (user.SelectedCollections.Contains(c.Id)) c.IsSelected = true;

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
          Task.Run(onCollectionSelectedAsync).FireAndForget();

          break;
        }
      }
    }

    private async Task onCollectionSelectedAsync() {
      List<DomainCollection> selectedCollections = viewModel.Collections.Where(c => c.IsSelected).Select(c => collections.Single(dc => dc.Id == c.Id)).ToList();

      user.SelectedCollections = selectedCollections.Select(dc => dc.Id).ToList();
      user.AreSettingsChanged  = true;

      await messenger.SendAsync(new CollectionsChangedMessage { Collections = selectedCollections });
    }

    #endregion Private Methods

  }

}
