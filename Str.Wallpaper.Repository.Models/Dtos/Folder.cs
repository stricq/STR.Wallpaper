using System;


namespace Str.Wallpaper.Repository.Models.Dtos {

  public class Folder : ServiceResponseBase {

    public string Id { get; set; }

    public string ParentId { get; set; }

    public string CollectionId { get; set; }

    public string Name { get; set; }

    public FolderType Type { get; set; }

    public ImageMetadata ImageInfo { get; set; }

    public bool IsUploaded { get; set; }

    public DateTime Created { get; set; }

  }

}
