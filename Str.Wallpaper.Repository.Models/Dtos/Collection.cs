using System;


namespace Str.Wallpaper.Repository.Models.Dtos {

  public class Collection : ServiceResponseBase {

    public string Id { get; set; }

    public string Name { get; set; }

    public string OwnerId { get; set; } // User.Id

    public string OwnerName { get; set; }

    public bool IsPublic { get; set; }

    public DateTime Created { get; set; }

  }

}
