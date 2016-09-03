using System.Threading.Tasks;

using Str.Wallpaper.Repository.Models.Settings;


namespace Str.Wallpaper.Domain.Contracts {

  public interface IProgramSettingsRepository {

    Task<ProgramSettings> LoadProgramSettingsAsync();

    Task SaveProgramSettingsAsync(ProgramSettings Settings);

  }

}
