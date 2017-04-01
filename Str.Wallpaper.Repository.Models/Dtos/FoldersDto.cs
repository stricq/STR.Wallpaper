using System.Collections.Generic;


namespace Str.Wallpaper.Repository.Models.Dtos {

  public class FoldersDto : ServiceResponseBase {

    public string CollectionId { get; set; }

    public string ParentId { get; set; }

    public List<Folder> Folders { get; set; }

  }

}
