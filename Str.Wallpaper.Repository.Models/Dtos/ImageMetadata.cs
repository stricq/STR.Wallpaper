using System;


namespace Str.Wallpaper.Repository.Models.Dtos {

  public class ImageMetadata {

    public Guid? Guid { get; set; }

    public string Filename { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int Depth { get; set; }

  }

}
