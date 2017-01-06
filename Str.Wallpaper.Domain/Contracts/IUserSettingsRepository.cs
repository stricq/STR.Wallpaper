using System.Threading.Tasks;

using Str.Wallpaper.Domain.Models;


namespace Str.Wallpaper.Domain.Contracts {

  public interface IUserSettingsRepository {

    Task LoadUserSettingsAsync(DomainUser Settings);

    Task SaveUserSettingsAsync(DomainUser Settings);

  }

}
