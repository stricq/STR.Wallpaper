

namespace Str.Wallpaper.Repository.Models.Dtos {

  public class ImageSettings {

    public PlacementType Placement { get; set; }

    public LayoutType Layout { get; set; }

    public double OffsetX { get; set; }

    public double OffsetY { get; set; }

    public double Zoom { get; set; }

    public bool Mirror { get; set; }

    public int Rotation { get; set; }

  }

}
