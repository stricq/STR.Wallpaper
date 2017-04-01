using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using AutoMapper;

using GongSolutions.Wpf.DragDrop;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Repository.Models.Dtos;
using Str.Wallpaper.Repository.Models.Settings;

using Str.Wallpaper.Wpf.Messages.Application;
using Str.Wallpaper.Wpf.Messages.Collections;
using Str.Wallpaper.Wpf.ViewEntities;
using Str.Wallpaper.Wpf.ViewModels;

using STR.Common.Contracts;
using STR.Common.Extensions;
using STR.Common.Messages;

using STR.DialogView.Domain.Messages;

using STR.MvvmCommon.Contracts;


namespace Str.Wallpaper.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class FolderTreeController : IController, IDragSource, IDropTarget {

    #region Private Fields

    private DomainUser user;

    private FolderViewEntity  contextFolder;
    private FolderViewEntity selectedFolder;

    private ProgramSettings settings;

    private List<DomainCollection> activeCollections;

    private List<FolderViewEntity> multiSelect;
    private List<FolderViewEntity> parents;

    private readonly FolderTreeViewModel     viewModel;
    private readonly MainMenuViewModel   menuViewModel;
    private readonly NotifyIconViewModel iconViewModel;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly IFolderService folderService;

    private readonly IAsyncService asyncService;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public FolderTreeController(FolderTreeViewModel ViewModel, MainMenuViewModel MenuViewModel, NotifyIconViewModel IconViewModel, IAsyncService AsyncService, IMessenger Messenger, IMapper Mapper, IFolderService FolderService) {
          viewModel =     ViewModel;
      menuViewModel = MenuViewModel;
      iconViewModel = IconViewModel;

      viewModel.Folders           = new ObservableCollection<FolderViewEntity>();
      viewModel.IsTreeViewEnabled = true;

      asyncService = AsyncService;

      messenger = Messenger;
      mapper    = Mapper;

      folderService = FolderService;

      activeCollections = new List<DomainCollection>();

      multiSelect = new List<FolderViewEntity>();

      parents = new List<FolderViewEntity>();
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
      messenger.Register<UserSettingsChangedMessage>(this, onUserSettingsChanged);

      messenger.RegisterAsync<CollectionsChangedMessage>(this, onCollectionsChangedAsync);
    }

    private void onUserSettingsChanged(UserSettingsChangedMessage message) {
      user = message.UserSettings;
    }

    private async Task onCollectionsChangedAsync(CollectionsChangedMessage message) {
      List<DomainCollection> mods = (from row1 in message.Collections
                                     join row2 in activeCollections on row1.Id equals row2.Id
                                   select row1).ToList();

      mods.ForEach(c => {
        FolderViewEntity rootFolder = viewModel.Folders.SingleOrDefault(folder => folder.Id == c.Id);

        if (rootFolder != null && rootFolder.Name != c.Name) {
          rootFolder.Name = c.Name;

          viewModel.Folders.Sort();
        }
      });

      List<DomainCollection> adds = (from row1 in message.Collections
                                     join row2 in activeCollections on row1.Id equals row2.Id into collGroup
                                     from sub  in collGroup.DefaultIfEmpty()
                                    where sub == null
                                   select row1).ToList();

      activeCollections.AddRange(adds);

      parents.Clear();

      await adds.ForEachAsync(loadFoldersAsync);

      List<DomainCollection> dels = (from row1 in activeCollections
                                     join row2 in message.Collections on row1.Id equals row2.Id into collGroup
                                     from sub  in collGroup.DefaultIfEmpty()
                                    where sub == null
                                   select row1).ToList();

      dels.ForEach(c => {
        FolderViewEntity rootFolder = viewModel.Folders.SingleOrDefault(folder => folder.Id == c.Id);

        activeCollections.Remove(c);

        if (activeCollections.Count == 0) selectedFolder = null;

        if (rootFolder != null) {
          multiSelect.Clear();

          rootFolder.Folder.Collection.Folders.Clear();

          asyncService.RunUiContext(() => viewModel.Folders.Remove(rootFolder));
        }
      });

      setMenuHeaders();
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {

    }

    #endregion Commands

    #region Private Methods

    private async Task loadFoldersAsync(DomainCollection collection) {
      FolderViewEntity root = mapper.Map<FolderViewEntity>(collection);

      parents.Add(root);

      await asyncService.RunUiContext(() => viewModel.Folders.OrderedMerge(root));

      await folderService.LoadFoldersAsync(user, collection, null, onLoadFoldersCallback);
    }

    private void onLoadFoldersCallback(string collectionId, string parentId, List<DomainFolder> folders, Exception ex) {
      if (ex != null) {
        while(ex.InnerException != null) ex = ex.InnerException;

        messenger.SendUi(new ApplicationErrorMessage { HeaderText = "SignalR Exception", Exception = ex });

        return;
      }

      if (folders == null) return; // Close out progress here

      DomainCollection collection = activeCollections.SingleOrDefault(dc => dc.Id == collectionId);

      if (collection == null) return; // Collection likely removed by user while still loading

      FolderViewEntity parentEntity = parentId == null ? parents.SingleOrDefault(fve => fve.Id == collectionId) : parents.SingleOrDefault(fve => fve.Id == parentId);

      if (parentEntity == null) return;

      collection.Folders.AddRange(folders);

      viewModel.CollectionName = collection.Folders.Count.ToString(); // Debug

      List<FolderViewEntity> fveParents = mapper.Map<List<FolderViewEntity>>(folders.Where(df => df.FolderType == FolderType.Folder));

      parents.AddRange(fveParents);

      asyncService.RunUiContext(() => parentEntity.Children.OrderedMerge(fveParents.OrderBy(fve => fve))).Wait();

      asyncService.RunUiContext(() => parentEntity.Children.OrderedMerge(mapper.Map<List<FolderViewEntity>>(folders.Where(df => df.FolderType == FolderType.Image)).OrderBy(fve => fve))).FireAndForget();

      foreach(DomainFolder folder in folders.Where(df => df.FolderType == FolderType.Folder)) {
        Task.Run(() => folderService.LoadFoldersAsync(user, collection, folder, onLoadFoldersCallback)).Wait();
      }
    }

    private void setMenuHeaders() {
      viewModel.CollectionName = selectedFolder != null ? selectedFolder.Folder.Collection.Name : activeCollections.Count > 0 ? String.Join(", ", activeCollections.OrderBy(c => c.Name).Select(c => c.Name)) : String.Empty;

      string removeHeader;
      string renameHeader = selectedFolder != null ? $"Rename {selectedFolder.Name}" : "Rename";

      if (multiSelect.Count > 1) removeHeader = "Remove Selected Items";
      else removeHeader = selectedFolder != null ? $"Remove {selectedFolder.Name}" : "Remove";

      viewModel.RemoveHeader = removeHeader;
      viewModel.RenameHeader = renameHeader;

      menuViewModel.RemoveFolderHeader = removeHeader;
      menuViewModel.RenameFolderHeader = renameHeader;

      bool isRemoveVisible = selectedFolder != null && !selectedFolder.IsCollectionFolder;
      bool isRenameVisible = selectedFolder != null && !selectedFolder.IsCollectionFolder && multiSelect.Count <= 1;

      viewModel.IsRemoveVisible = isRemoveVisible;
      viewModel.IsRenameVisible = isRenameVisible;

      menuViewModel.IsRemoveFolderVisible = isRemoveVisible;
      menuViewModel.IsRenameFolderVisible = isRenameVisible;

      iconViewModel.TooltipText = selectedFolder != null ? selectedFolder.Name : "STR Wallpaper v6";
    }

    #endregion Private Methods

    #region IDragSource Implementation

    public bool CanStartDrag(IDragInfo dragInfo) {
      FolderViewEntity folder = dragInfo.SourceItem as FolderViewEntity;

      if (folder == null) return false;

      return !folder.IsCollectionFolder;
    }

    public void StartDrag(IDragInfo dragInfo) {
      dragInfo.Effects = DragDropEffects.Move;
      dragInfo.Data    = multiSelect.ToList();
    }

    public void DragCancelled() { }

    public void Dropped(IDropInfo dropInfo) { }

    public bool TryCatchOccurredException(Exception exception) {
      return false;
    }

    #endregion IDragSource Implementation

    #region IDropTarget Implementation

    public void DragOver(IDropInfo dropInfo) {
      List<FolderViewEntity> sourceItems = dropInfo.Data as List<FolderViewEntity>;

      FolderViewEntity targetItem = dropInfo.TargetItem as FolderViewEntity;

      if (sourceItems == null || sourceItems.Count == 0 || targetItem == null) return;

      if (targetItem.Folder.Collection.OwnerUserId != user.Id) return; // Cannot drop into someone else's collection

      if (sourceItems.Any(f => f == targetItem || f.Parent == targetItem)) return; // Cannot drop onto self or self's parent

      if (targetItem.FolderType == FolderType.Folder) {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
//      dropInfo.Effects = sourceItems[0].Collection.OwnerId == user.Id ? DragDropEffects.Move : DragDropEffects.Move;
        dropInfo.Effects = DragDropEffects.Move;
      }
    }

    public async void Drop(IDropInfo dropInfo) {
      List<FolderViewEntity> sourceItems = dropInfo.Data as List<FolderViewEntity>;

      FolderViewEntity targetItem = dropInfo.TargetItem as FolderViewEntity;

      if (sourceItems == null || sourceItems.Count == 0 || targetItem == null) return;

      await onDropAsync(sourceItems, targetItem);
    }

    private async Task onDropAsync(List<FolderViewEntity> sourceItems, FolderViewEntity targetItem) {
      multiSelect.ForEach(f => {
        f.IsMultiSelected = false;
        f.IsSelected      = false;
      });

      multiSelect.Clear();

      DomainCollection sourceCollection = sourceItems[0].Folder.Collection;

      if (sourceItems[0].Folder.Collection.OwnerUserId == user.Id) await onDropMoveAsync(sourceItems, targetItem);
      else await onDropCopyAsync(sourceItems, targetItem);

      await messenger.SendAsync(new CollectionCountsChanged { Collections = new List<DomainCollection> { sourceCollection, targetItem.Folder.Collection } });
    }

    private async Task onDropMoveAsync(List<FolderViewEntity> sourceItems, FolderViewEntity targetItem) {
      int removed = 0;

      foreach(FolderViewEntity folder in sourceItems.ToList().Where(f => f.Folder.Collection != targetItem.Folder.Collection)) {
        if (await folderService.CheckFilenameExistsAsync(targetItem.Folder.Collection, Path.GetFileName(folder.Folder.Metadata.Filename))) {
          sourceItems.Remove(folder);

          ++removed;
        }
      }

      foreach(FolderViewEntity f in sourceItems.ToList()) {
        FolderViewEntity parent = await verifyAndCreatePathAsync(f, targetItem);

        f.Parent.Children.Remove(f);

        f.Parent = parent;

        f.Folder.ParentId                = parent.Folder.Id;
        f.Folder.Settings.ParentFolderId = parent.Folder.Id;

        f.Folder.Collection   = parent.Folder.Collection;
        f.Folder.CollectionId = parent.Folder.CollectionId;

        parent.Children.OrderedMerge(f);

        parent.IsExpanded = true;

        if (f.FolderType == FolderType.Folder && f.Children.Any()) await onDropMoveAsync(f.Children.ToList(), f);
      }

      await folderService.SaveFoldersAsync(user, sourceItems.Select(fve => fve.Folder).ToList());

//    await imageSettingsStore.SaveUserImageSettingsAsync(sourceItems.Where(f => !String.IsNullOrEmpty(f.Folder.Settings.Id)).Select(f => f.Folder.Settings).ToList());

      if (removed > 0) {
        messenger.Send(new MessageBoxDialogMessage { Header = "Some Items Not Moved", Message = $"{removed} item{(removed == 1 ? " was" : "s were")} not moved as {(removed == 1 ? "it" : "they")} already exist{(removed == 1 ? "s" : "")} in the destination Collection." });
      }
    }

    private async Task<FolderViewEntity> verifyAndCreatePathAsync(FolderViewEntity source, FolderViewEntity target) {
      if (!settings.UseFullPathDrop || !target.IsCollectionFolder || source.Parent.IsCollectionFolder) return target;

      FolderViewEntity p = source.Parent;

      FolderViewEntity parentPath = null;

      while(!p.IsCollectionFolder) {
        FolderViewEntity folder = p.DeepCopy();

        folder.Folder.Id = null;

        folder.Children.Clear();

        if (parentPath != null) {
          parentPath.Parent = folder;

          folder.Children.Add(parentPath);
        }

        parentPath = folder;

        p = p.Parent;
      }

      FolderViewEntity parent = viewModel.Folders.Single(f => f.Folder.Collection.Id == target.Folder.Collection.Id);

      p = parentPath;

      FolderViewEntity result = null;

      if (p != null) {
        do {
          FolderViewEntity temp = parent.Children.SingleOrDefault(f => f.Name == p.Name);

          if (temp == null || String.IsNullOrEmpty(temp.Folder.Id)) {
            p.Folder.Collection   = parent.Folder.Collection;
            p.Folder.CollectionId = parent.Folder.CollectionId;

            p.Parent = parent;

            p.Folder.ParentId = parent.Folder.Id;

            await folderService.SaveFolderAsync(user, p.Folder);

            p.Folder.Settings.FolderId       = p.Folder.Id;
            p.Folder.Settings.ParentFolderId = parent.Folder.Id;

            if (!parent.Children.Contains(p)) parent.Children.OrderedMerge(p);

            parent = p;
          }
          else parent = temp;

          result = temp ?? p;

        } while((p = p.Children.SingleOrDefault()) != null);
      }

      return result ?? target;
    }

    private async Task onDropCopyAsync(List<FolderViewEntity> sourceItems, FolderViewEntity targetItem) {
      List<FolderViewEntity> copied = await Task.Run(() => sourceItems.Select(copyFolder).ToList());

      await saveCopyFoldersAsync(copied, targetItem);

      targetItem.Children.OrderedMerge(copied.OrderBy(f => f));
    }

    private static FolderViewEntity copyFolder(FolderViewEntity folder) {
      FolderViewEntity copy = folder.DeepCopy();

      copy.Folder.Id = null;

      copy.Children.Clear();

      copy.Children.AddRange(folder.Children.Select(copyFolder));

      return copy;
    }

    private async Task saveCopyFoldersAsync(List<FolderViewEntity> folders, FolderViewEntity parent) {
      folders.ForEach(f => {
        f.Folder.Collection   = parent.Folder.Collection;
        f.Folder.CollectionId = parent.Folder.CollectionId;

        f.Parent = parent;

        f.Folder.ParentId = parent.Folder.Id;

        f.Folder.Settings.ParentFolderId = parent.Folder.Id;
        f.Folder.Settings.OwnerUserId    = parent.Folder.Collection.OwnerUserId;
      });

      await folderService.SaveFoldersAsync(user, folders.Select(fve => fve.Folder).ToList());

      List<FolderViewEntity> settingsFolders = folders.Where(f => !String.IsNullOrEmpty(f.Folder.Settings.Id)).ToList();

      settingsFolders.ForEach(f => {
        f.Folder.Settings.Id       = null;
        f.Folder.Settings.FolderId = f.Parent.Folder.Id;
      });

//    await imageSettingsStore.SaveUserImageSettingsAsync(settingsFolders.Select(f => f.Folder.Settings).ToList());

      await folders.Where(f => f.FolderType == FolderType.Folder).ForEachAsync(f => saveCopyFoldersAsync(f.Children.ToList(), f));
    }

    #endregion IDropTarget Implementation

  }

}
