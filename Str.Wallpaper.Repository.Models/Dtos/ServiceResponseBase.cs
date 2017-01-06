

namespace Str.Wallpaper.Repository.Models.Dtos {

  public class ServiceResponseBase {

    public string Message { get; set; }

    public string ExceptionMessage { get; set; }

    public string ExceptionType { get; set; }

    public string StackTrace { get; set; }

  }

}
