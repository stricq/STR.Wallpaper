using System;
using System.Threading.Tasks;

using Str.Wallpaper.Domain.Models;


namespace Str.Wallpaper.Domain.Contracts {

  public interface IUserSessionService {

    Task<bool> CreateUserAsync(DomainUserSettings UserSettings);

    Task<bool> LoginAsync(DomainUserSettings UserSettings);

    Task<bool> DisconnectAsync(DomainUserSettings UserSettings);

    Task<bool> ChangePassword(DomainUserSettings UserSettings, string newPassword);

  }

}
