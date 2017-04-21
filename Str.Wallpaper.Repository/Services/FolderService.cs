using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNet.SignalR.Client;

using RestSharp;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Repository.Models.Dtos;


namespace Str.Wallpaper.Repository.Services {

  [Export(typeof(IFolderService))]
  public sealed class FolderService : ServiceBase, IFolderService {

    #region Private Fields

    private const string Version    = "v1";
    private const string Controller = "Folder";

    private const string Hub = "FoldersHub";

    private static readonly string ServicePath;

    private readonly IMapper mapper;

    #endregion Private Fields

    #region Static Constructor

    static FolderService() {
      ServicePath = $"/{Version}/{Controller}";
    }

    #endregion Static Constructor

    #region Constructor

    [ImportingConstructor]
    public FolderService(IMapper Mapper) {
      mapper = Mapper;
    }

    #endregion Constructor

    #region IFolderService Implementation

    public async Task LoadFoldersAsync(DomainUser User, DomainCollection Collection, DomainFolder Parent, Action<string, string, List<DomainFolder>, Exception> Callback) {
      if (User.SessionId == null) return;

      string group = Guid.NewGuid().ToString("D");

      HubConnection connection = CreateConnection();

      connection.DeadlockErrorTimeout = TimeSpan.FromSeconds(60);

      IHubProxy foldersProxy = connection.CreateHubProxy(Hub);

      connection.Error += ex => { Callback(null, null, null, ex); };

      foldersProxy.On<FoldersDto>("OnFoldersLoaded", dto => {
        if (dto.Folders == null) {
          Callback(dto.CollectionId, dto.ParentId, null, null);

          return;
        }

        Callback(dto.CollectionId, dto.ParentId, mapper.Map<List<DomainFolder>>(dto.Folders), null);
      });

      await connection.Start();

      await foldersProxy.Invoke("JoinGroupAsync", group);

      await foldersProxy.Invoke("LoadFoldersAsync", group, User.SessionId, Collection.Id, Parent?.Id);

      connection.Stop();
      connection.Dispose();
    }

    public async Task<bool> CheckFilenameExistsAsync(DomainCollection Collection, string Filename) {
      RestRequest request = CreateRequest($"{ServicePath}/{{collectionId}}", Method.PUT);

      request.AddUrlSegment("collectionId", Collection.Id);

      request.AddBody(Filename);

      IRestResponse<CheckImageDto> response = await Client.ExecuteTaskAsync<CheckImageDto>(request);

      return HandleError(response).HasFilename;
    }

    public async Task SaveFolderAsync(DomainUser User, DomainFolder Folder) {
      await SaveFoldersAsync(User, new List<DomainFolder> { Folder });
    }

    public async Task SaveFoldersAsync(DomainUser User, List<DomainFolder> Folders) {
      if (User.SessionId == null) return;

      RestRequest request = CreateRequest($"{ServicePath}/{{sessionId}}", Method.POST);

      request.AddUrlSegment("sessionId", User.SessionId.Value.ToString("D"));

      request.AddBody(Folders);

      IRestResponse<FoldersDto> response = await Client.ExecuteTaskAsync<FoldersDto>(request);

      List<Folder> folders = HandleError(response).Folders;

      for(int i = 0; i < Folders.Count; ++i) {
        Folders[i].Id = folders[i].Id;

        Folders[i].Settings.FolderId       = folders[i].Id;
        Folders[i].Settings.ParentFolderId = folders[i].ParentId;
      }
    }

    public async Task DeleteFolderAsync(DomainUser User, DomainFolder Folder) {
      if (User.SessionId == null) return;

      RestRequest request = CreateRequest($"{ServicePath}/{{sessionId}}/Folder/{{folderId}}", Method.DELETE);

      request.AddUrlSegment("sessionId", User.SessionId.Value.ToString("D"));
      request.AddUrlSegment("folderId",  Folder.Id);

      IRestResponse<ServiceResponseBase> response = await Client.ExecuteTaskAsync<ServiceResponseBase>(request);

      HandleError(response);
    }

    public async Task DeleteFolderChildrenAsync(DomainUser User, DomainFolder ParentFolder) {
      if (User.SessionId == null) return;

      RestRequest request = CreateRequest($"{ServicePath}/{{sessionId}}/Parent/{{parentId}}", Method.DELETE);

      request.AddUrlSegment("sessionId", User.SessionId.Value.ToString("D"));
      request.AddUrlSegment("parentId",  ParentFolder.Id);

      IRestResponse<ServiceResponseBase> response = await Client.ExecuteTaskAsync<ServiceResponseBase>(request);

      HandleError(response);
    }

    public async Task<int> CountImageReferencesAsync(DomainFolder Folder) {
      if (Folder.Metadata?.Guid == null) return 0;

      RestRequest request = CreateRequest($"{ServicePath}/{{guid}}", Method.GET);

      request.AddUrlSegment("guid", Folder.Metadata.Guid.Value.ToString("D"));

      IRestResponse<CheckImageDto> response = await Client.ExecuteTaskAsync<CheckImageDto>(request);

      return HandleError(response).GuidReferences;
    }

    #endregion IFolderService Implementation

  }

}
