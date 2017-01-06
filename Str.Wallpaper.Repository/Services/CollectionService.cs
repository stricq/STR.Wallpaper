using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using AutoMapper;

using RestSharp;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Repository.Models.Dtos;


namespace Str.Wallpaper.Repository.Services {

  [Export(typeof(ICollectionService))]
  public sealed class CollectionService : ServiceBase, ICollectionService {

    #region Private Fields

    private const string Version    = "v1";
    private const string Controller = "Collection";

    private static readonly string ServicePath;

    private readonly IMapper mapper;

    #endregion Private Fields

    #region Static Constructor

    static CollectionService() {
      ServicePath = $"/{Version}/{Controller}";
    }

    #endregion Static Constructor

    #region Constructor

    [ImportingConstructor]
    public CollectionService(IMapper Mapper) {
      mapper = Mapper;
    }

    #endregion Constructor

    #region ICollectionService Implementation

    public async Task<List<DomainCollection>> LoadCollectionsAsync(DomainUser User) {
      if (User.SessionId == null) return null;

      RestRequest request = CreateRequest($"{ServicePath}/{{sessionId}}", Method.GET);

      request.AddUrlSegment("sessionId", User.SessionId.Value.ToString("D"));

      IRestResponse<CollectionsDto> response = await Client.ExecuteTaskAsync<CollectionsDto>(request);

      return mapper.Map<List<DomainCollection>>(HandleError(response).Collections) ?? new List<DomainCollection>();
    }

    public async Task<Tuple<int, int>> CountWallpapersAsync(DomainUser User, DomainCollection Collection) {
      if (User.SessionId == null) return Tuple.Create(0, 0);

      RestRequest request = CreateRequest($"{ServicePath}/{{sessionId}}/{{collectionId}}", Method.GET);

      request.AddUrlSegment("sessionId",    User.SessionId.Value.ToString("D"));
      request.AddUrlSegment("collectionId", Collection.Id);

      IRestResponse<CollectionCountsDto> response = await Client.ExecuteTaskAsync<CollectionCountsDto>(request);

      CollectionCountsDto counts = HandleError(response);

      Collection.TotalFolders    = counts.TotalFolders;
      Collection.TotalWallpapers = counts.TotalWallpapers;

      return Tuple.Create(counts.TotalFolders, counts.TotalWallpapers);
    }

    public async Task SaveCollectionAsync(DomainUser User, DomainCollection Collection) {
      if (User.SessionId == null) return;

      RestRequest request = CreateRequest($"{ServicePath}/{{sessionId}}", Method.POST);

      request.AddUrlSegment("sessionId", User.SessionId.Value.ToString("D"));

      request.AddBody(mapper.Map<Collection>(Collection));

      IRestResponse<Collection> response = await Client.ExecuteTaskAsync<Collection>(request);

      Collection collection = HandleError(response);

      Collection.Id = collection.Id;
    }

    public async Task DeleteCollectionAsync(DomainUser User, DomainCollection Collection) {
      if (User.SessionId == null) return;

      RestRequest request = CreateRequest($"{ServicePath}/{{sessionId}}/{{collectionId}}", Method.DELETE);

      request.AddUrlSegment("sessionId",    User.SessionId.Value.ToString("D"));
      request.AddUrlSegment("collectionId", Collection.Id);

      IRestResponse<ServiceResponseBase> response = await Client.ExecuteTaskAsync<ServiceResponseBase>(request);

      HandleError(response);
    }

    #endregion ICollectionService Implementation

  }

}
