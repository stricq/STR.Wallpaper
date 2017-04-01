using System.Collections.Generic;
using System.Windows.Media;


namespace Str.Wallpaper.Repository.Models.Dtos {

  public class UserImageSettings {

    public string Id { get; set; }

    public string OwnerUserId { get; set; }

    public string ParentFolderId { get; set; }

    public string FolderId { get; set; }

    public Color BackgroundColor { get; set; }

    public Dictionary<string, ImageSettings> ScreenImageSettings { get; set; } = new Dictionary<string, ImageSettings>();

    public bool IsChecked { get; set; }

  }

}
