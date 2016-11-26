using System.ComponentModel.Composition;
using System.Windows;

using AutoMapper;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;
using Str.Wallpaper.Repository.Models.Settings;

using Str.Wallpaper.Wpf.ViewEntities;


namespace Str.Wallpaper.Wpf.Mapping {

  [Export(typeof(IAutoMapperConfiguration))]
  public sealed class ViewEntityMappingConfiguration : IAutoMapperConfiguration {

    #region IAutoMapperConfiguration Implementation

    public void RegisterMappings(IMapperConfigurationExpression config) {
      settings(config);
    }

    #endregion IAutoMapperConfiguration  Implementation

    #region Private Methods

    private static void settings(IMapperConfigurationExpression config) {
      config.CreateMap<ProgramSettings, ProgramSettingsViewEntity>().ForMember(dest => dest.AreSettingsChanged, opt => opt.Ignore());

      config.CreateMap<ProgramSettingsViewEntity, ProgramSettings>();

      config.CreateMap<ProgramSettingsViewEntity, ProgramSettingsViewEntity>();

      config.CreateMap<WindowSettings, WindowSettingsViewEntity>().ForMember(dest => dest.MainWindowState,    opt => opt.ResolveUsing(src => (WindowState)src.MainWindowState))
                                                                  .ForMember(dest => dest.PreMinimizedState,  opt => opt.ResolveUsing(src => (WindowState)src.PreMinimizedState))
                                                                  .ForMember(dest => dest.AreSettingsChanged, opt => opt.Ignore());

      config.CreateMap<WindowSettingsViewEntity, WindowSettings>().ForMember(dest => dest.MainWindowState,   opt => opt.ResolveUsing(src => (int)src.MainWindowState))
                                                                  .ForMember(dest => dest.PreMinimizedState, opt => opt.ResolveUsing(src => (int)src.PreMinimizedState));

      config.CreateMap<DomainUserSettings, UserSettingsViewEntity>().ForMember(dest => dest.IsOnline,           opt => opt.ResolveUsing(src => src.SessionId != null))
                                                                    .ForMember(dest => dest.IsNotOnline,        opt => opt.Ignore())
                                                                    .ForMember(dest => dest.AreSettingsChanged, opt => opt.Ignore());

      config.CreateMap<UserSettingsViewEntity, DomainUserSettings>().ForMember(dest => dest.SessionId, opt => opt.Ignore());
    }

    #endregion Private Methods

  }

}
