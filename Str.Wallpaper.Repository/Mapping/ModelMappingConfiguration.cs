using System.ComponentModel.Composition;

using AutoMapper;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Repository.Models.Dtos;
using Str.Wallpaper.Repository.Models.Settings;


namespace Str.Wallpaper.Repository.Mapping {

  [Export(typeof(IAutoMapperConfiguration))]
  public sealed class ModelMappingConfiguration : IAutoMapperConfiguration {

    #region IAutoMapperConfiguration Implementation

    public void RegisterMappings(IMapperConfigurationExpression config) {
      config.CreateMap<UserSettings, DomainUser>().ForMember(dest => dest.SessionId,          opt => opt.Ignore())
                                                  .ForMember(dest => dest.IsLoggingIn,        opt => opt.Ignore())
                                                  .ForMember(dest => dest.Id,                 opt => opt.Ignore())
                                                  .ForMember(dest => dest.AreSettingsChanged, opt => opt.Ignore())
                                                  .ReverseMap();

      config.CreateMap<Collection, DomainCollection>().ForMember(dest => dest.TotalWallpapers, opt => opt.Ignore())
                                                      .ForMember(dest => dest.TotalFolders,    opt => opt.Ignore());
    }

    #endregion IAutoMapperConfiguration Implementation

  }

}
