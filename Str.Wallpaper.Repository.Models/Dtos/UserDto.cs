using System;
using System.Diagnostics.CodeAnalysis;


namespace Str.Wallpaper.Repository.Models.Dtos {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public sealed class UserDto : ServiceResponseBase {

    public string Id { get; set; }

    public string Username { get; set; }

    public Guid? SessionId { get; set; }

  }

}
