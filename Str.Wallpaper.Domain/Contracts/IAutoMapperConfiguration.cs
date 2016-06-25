using AutoMapper;


namespace Str.Wallpaper.Domain.Contracts {

  public interface IAutoMapperConfiguration {

    void RegisterMappings(IMapperConfiguration config);

  }

}
