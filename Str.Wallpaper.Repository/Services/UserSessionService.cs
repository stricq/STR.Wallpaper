using System.ComponentModel.Composition;
using System.Net;
using System.Threading.Tasks;

using RestSharp;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Repository.Models.Dtos;


namespace Str.Wallpaper.Repository.Services {

  [Export(typeof(IUserSessionService))]
  public sealed class UserSessionService : ServiceBase, IUserSessionService {

    #region Private Fields

    private const string Version    = "v1";
    private const string Controller = "User";

    private static readonly string ServicePath;

    #endregion Private Fields

    #region Static Constructor

    static UserSessionService() {
      ServicePath = $"/{Version}/{Controller}";
    }

    #endregion Static Constructor

    #region IUserSessionService Implementation

    public async Task<bool> CreateUserAsync(DomainUser UserSettings) {
      RestRequest request = CreateRequest($"{ServicePath}/{{username}}/{{password}}", Method.POST);

      request.AddUrlSegment("username", UserSettings.Username);
      request.AddUrlSegment("password", UserSettings.Password);

      IRestResponse<UserDto> response = await Client.ExecuteTaskAsync<UserDto>(request);

      if (response.StatusCode == HttpStatusCode.Conflict) {
        UserSettings.SessionId = null;

        return false;
      }

      UserDto user = HandleError(response);

      UserSettings.Id        = user.Id;
      UserSettings.SessionId = user.SessionId;

      return true;
    }

    public async Task<bool> LoginAsync(DomainUser UserSettings) {
      RestRequest request = CreateRequest($"{ServicePath}/{{username}}/{{password}}", Method.GET);

      request.AddUrlSegment("username", UserSettings.Username);
      request.AddUrlSegment("password", UserSettings.Password);

      IRestResponse<UserDto> response = await Client.ExecuteTaskAsync<UserDto>(request);

      if (response.StatusCode == HttpStatusCode.NotFound) {
        UserSettings.SessionId = null;

        return false;
      }

      UserDto user = HandleError(response);

      UserSettings.Id        = user.Id;
      UserSettings.SessionId = user.SessionId;

      return true;
    }

    public async Task<bool> DisconnectAsync(DomainUser UserSettings) {
      if (UserSettings.SessionId == null) return true;

      RestRequest request = CreateRequest($"{ServicePath}/{{sessionId}}", Method.DELETE);

      request.AddUrlSegment("sessionId", UserSettings.SessionId.Value.ToString("D"));

      IRestResponse<ServiceResponseBase> response = await Client.ExecuteTaskAsync<ServiceResponseBase>(request);

      return response.StatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> ChangePassword(DomainUser UserSettings, string newPassword) {
      RestRequest request = CreateRequest($"{ServicePath}/{{username}}/{{oldPassword}}/{{newPassword}}", Method.PATCH);

      request.AddUrlSegment("username",    UserSettings.Username);
      request.AddUrlSegment("oldPassword", UserSettings.Password);
      request.AddUrlSegment("newPassword", newPassword);

      IRestResponse<ServiceResponseBase> response = await Client.ExecuteTaskAsync<ServiceResponseBase>(request);

      return response.StatusCode == HttpStatusCode.OK;
    }

    #endregion IUserSessionService Implementation

  }

}
