using System;
using System.Net;

using Microsoft.AspNet.SignalR.Client;

using RestSharp;

using Str.Wallpaper.Repository.Models.Dtos;


namespace Str.Wallpaper.Repository.Services {

  public class ServiceBase {

    #region Private Fields

    private const string AcceptJson = "application/json";

    private const string RootUrl = "http://api.stricq.com";
//  private const string RootUrl = "http://localhost:20923";

    private const string Path = "/StrWallpaper";

    [ThreadStatic]
    private static IRestClient client;

    #endregion Private Fields

    protected static HubConnection CreateConnection() {
      return new HubConnection($"{RootUrl}/signalr/hubs");
    }

    protected static IRestClient Client {
      get {
        if (client != null) return client;

        client = new RestClient($"{RootUrl}{Path}");

        return client;
      }
    }

    protected static RestRequest CreateRequest(string path, Method method) {
      RestRequest request = new RestRequest(path, method) { RequestFormat = DataFormat.Json };

      request.AddHeader(HttpRequestHeader.Accept.ToString(), AcceptJson);

      return request;
    }

    protected static T HandleError<T>(IRestResponse<T> response) where T : ServiceResponseBase {
      if (response.StatusCode == HttpStatusCode.OK) return response.Data;

      switch(response.StatusCode) {
        case HttpStatusCode.NoContent:
        case HttpStatusCode.NotFound: {
          return null;
        }
        default: {
          throw new Exception($"Session Service Error ({response.Data.Message ?? response.StatusDescription}).");
        }
      }
    }

  }

}
