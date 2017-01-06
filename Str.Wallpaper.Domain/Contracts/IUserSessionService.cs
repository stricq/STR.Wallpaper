using System;
using System.Threading.Tasks;

using Str.Wallpaper.Domain.Models;


namespace Str.Wallpaper.Domain.Contracts {

  public interface IUserSessionService {

    Task<bool> CreateUserAsync(DomainUser UserSettings);

    Task<bool> LoginAsync(DomainUser UserSettings);

    Task<bool> DisconnectAsync(DomainUser UserSettings);

    Task<bool> ChangePassword(DomainUser UserSettings, string newPassword);

  }

}
