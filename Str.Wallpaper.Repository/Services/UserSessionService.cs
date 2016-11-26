using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading.Tasks;

using RestSharp;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;
using Str.Wallpaper.Repository.Models.User;


namespace Str.Wallpaper.Repository.Services {

  [Export(typeof(IUserSessionService))]
  public sealed class UserSessionService : IUserSessionService {

    #region Private Fields

    private const string AcceptJson = "application/json";

    private const string RootUrl        = "https://api.stricq.com/StrWallpaper";

    private const string Version        = "v1";
    private const string UserController = "User";

    private static readonly string ServicePath;

    [ThreadStatic]
    private static IRestClient client;

    #endregion Private Fields

    #region Static Constructor

    static UserSessionService() {
      ServicePath = $"/{Version}/{UserController}";
    }

    #endregion Static Constructor

    #region IUserSessionService Implementation

    public async Task<bool> CreateUserAsync(DomainUserSettings UserSettings) {
      RestRequest request = createRequest($"{ServicePath}/{{username}}/{{password}}", Method.POST);

      request.AddUrlSegment("username", UserSettings.Username);
      request.AddUrlSegment("password", UserSettings.Password);

      IRestResponse<UserSessionResponse> response = await Client.ExecuteTaskAsync<UserSessionResponse>(request);

      if (response.StatusCode == HttpStatusCode.Conflict) {
        UserSettings.SessionId = null;

        return false;
      }

      UserSettings.SessionId = handleError(response).SessionId;

      return true;
    }

    public async Task<bool> LoginAsync(DomainUserSettings UserSettings) {
      RestRequest request = createRequest($"{ServicePath}/{{username}}/{{password}}", Method.PUT);

      request.AddUrlSegment("username", UserSettings.Username);
      request.AddUrlSegment("password", UserSettings.Password);

      IRestResponse<UserSessionResponse> response = await Client.ExecuteTaskAsync<UserSessionResponse>(request);

      if (response.StatusCode == HttpStatusCode.NotFound) {
        UserSettings.SessionId = null;

        return false;
      }

      UserSettings.SessionId = handleError(response).SessionId;

      return true;
    }

    public async Task<bool> DisconnectAsync(DomainUserSettings UserSettings) {
      if (UserSettings.SessionId == null) return true;

      RestRequest request = createRequest($"{ServicePath}/{{sessionId}}", Method.PUT);

      request.AddUrlSegment("sessionId", UserSettings.SessionId.Value.ToString("D"));

      IRestResponse<UserSessionResponse> response = await Client.ExecuteTaskAsync<UserSessionResponse>(request);

      return response.StatusCode == HttpStatusCode.OK;
    }

    #endregion IUserSessionService Implementation

    #region Private Methods

    private static IRestClient Client {
      get {
        if (client != null) return client;

        client = new RestClient(RootUrl);

        return client;
      }
    }

    private static RestRequest createRequest(string path, Method method) {
      RestRequest request = new RestRequest(path, method) { RequestFormat = DataFormat.Json };

      request.AddHeader(HttpRequestHeader.Accept.ToString(), AcceptJson);

      return request;
    }

    private static T handleError<T>(IRestResponse<T> response) where T : new() {
      if (response.StatusCode == HttpStatusCode.OK) return response.Data;

      switch(response.StatusCode) {
        case HttpStatusCode.NotFound: {
          return new T();
        }
        default: {
          throw new Exception($"Session Service Error ({response.StatusCode}).");
        }
      }
    }

    #endregion Private Methods

  }

}
