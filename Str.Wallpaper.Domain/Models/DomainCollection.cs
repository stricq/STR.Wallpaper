using System;
using System.Collections.Generic;

using STR.Common.Contracts;


namespace Str.Wallpaper.Domain.Models {

  public sealed class DomainCollection : IDeepCloneable {

    #region Properties

    public string Id { get; set; }

    public string Name { get; set; }

    public string OwnerUserId { get; set; } // User.Id

    public string OwnerName { get; set; }

    public bool IsPublic { get; set; }

    public DateTime Created { get; set; }

    #endregion Properties

    #region Domain Properties

    public int TotalFolders { get; set; }

    public int TotalWallpapers { get; set; }

    public List<DomainFolder> Folders { get; set; } = new List<DomainFolder>();

    #endregion Domain Properties

  }

}
