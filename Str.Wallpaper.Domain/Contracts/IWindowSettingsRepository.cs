using System.Threading.Tasks;

using Str.Wallpaper.Repository.Models.Settings;


namespace Str.Wallpaper.Domain.Contracts {

  public interface IWindowSettingsRepository {

    Task<WindowSettings> LoadWindowSettingsAsync();

    Task SaveWindowSettingsAsync(WindowSettings settings);

  }

}
