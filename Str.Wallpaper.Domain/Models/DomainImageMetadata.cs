using System;


namespace Str.Wallpaper.Domain.Models {

  public class DomainImageMetadata {

    public Guid Guid { get; set; }

    public string Filename { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int Depth { get; set; }

  }

}
