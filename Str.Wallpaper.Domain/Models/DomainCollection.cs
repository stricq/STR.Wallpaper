using System;


namespace Str.Wallpaper.Domain.Models {

  public class DomainCollection {

    #region Properties

    public string Id { get; set; }

    public string Name { get; set; }

    public string OwnerId { get; set; } // User.Id

    public string OwnerName { get; set; }

    public bool IsPublic { get; set; }

    public DateTime Created { get; set; }

    #endregion Properties

    #region Domain Properties

    public int TotalFolders { get; set; }

    public int TotalWallpapers { get; set; }

    #endregion Domain Properties

  }

}
