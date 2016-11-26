using System;
using System.Diagnostics.CodeAnalysis;


namespace Str.Wallpaper.Repository.Models.User {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class UserSessionResponse {

    public string Message { get; set; }

    public Guid? SessionId { get; set; }

  }

}
