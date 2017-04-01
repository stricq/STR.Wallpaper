using System;

using Str.Wallpaper.Repository.Models.Dtos;


namespace Str.Wallpaper.Domain.Models {

  public class DomainFolder {

    #region Model

    public string Id { get; set; }

    public string ParentId { get; set; }

    public string CollectionId { get; set; }

    public string Name { get; set; }

    public FolderType FolderType { get; set; }

    public ImageMetadata Metadata { get; set; }

    public bool IsUploaded { get; set; }

    public DateTime Created { get; set; }

    #endregion Model

    #region Domain Properties

    public DomainCollection Collection { get; set; }

    public UserImageSettings Settings { get; set; }

    #endregion Domain Properties

  }

}
