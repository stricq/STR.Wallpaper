using System.Threading.Tasks;

using Str.Wallpaper.Domain.Models;


namespace Str.Wallpaper.Domain.Contracts {

  public interface IUserSettingsRepository {

    Task LoadUserSettingsAsync(DomainUserSettings Settings);

    Task SaveUserSettingsAsync(DomainUserSettings Settings);

  }

}
