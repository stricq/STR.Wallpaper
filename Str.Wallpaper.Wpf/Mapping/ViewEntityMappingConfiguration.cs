using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;

using AutoMapper;

using Str.Wallpaper.Domain.Contracts;
using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Repository.Models.Dtos;
using Str.Wallpaper.Repository.Models.Settings;

using Str.Wallpaper.Wpf.ViewEntities;


namespace Str.Wallpaper.Wpf.Mapping {

  [Export(typeof(IAutoMapperConfiguration))]
  public sealed class ViewEntityMappingConfiguration : IAutoMapperConfiguration {

    #region IAutoMapperConfiguration Implementation

    public void RegisterMappings(IMapperConfigurationExpression config) {
      settings(config);

      domain(config);
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

      config.CreateMap<DomainUser, UserSettingsViewEntity>().ForMember(dest => dest.IsOnline,           opt => opt.ResolveUsing(src => src.SessionId != null))
                                                            .ForMember(dest => dest.IsNotOnline,        opt => opt.Ignore());

      config.CreateMap<UserSettingsViewEntity, DomainUser>().ForMember(dest => dest.SessionId,           opt => opt.Ignore())
                                                            .ForMember(dest => dest.IsLoggingIn,         opt => opt.Ignore())
                                                            .ForMember(dest => dest.Id,                  opt => opt.Ignore())
                                                            .ForMember(dest => dest.SelectedCollections, opt => opt.Ignore());
    }

    private static void domain(IMapperConfigurationExpression config) {
      config.CreateMap<DomainCollection, CollectionViewEntity>().ForMember(dest => dest.IsSelected,         opt => opt.Ignore())
                                                                .ForMember(dest => dest.IsActive,           opt => opt.Ignore())
                                                                .ForMember(dest => dest.IsContextMenuOpen,  opt => opt.Ignore())
                                                                .ForMember(dest => dest.Status,             opt => opt.Ignore())
                                                                .ForMember(dest => dest.ContextMenuOpening, opt => opt.Ignore());

      config.CreateMap<DomainCollection, FolderViewEntity>().ForMember(dest => dest.Folder,             opt => opt.ResolveUsing(src => new DomainFolder { Collection = src, CollectionId = src.Id }))
                                                            .ForMember(dest => dest.Children,           opt => opt.UseValue(new ObservableCollection<FolderViewEntity>()))
                                                            .ForMember(dest => dest.FolderType,         opt => opt.UseValue(FolderType.Folder))
                                                            .ForMember(dest => dest.IsChecked,          opt => opt.UseValue(true))
                                                            .ForMember(dest => dest.IsCollectionFolder, opt => opt.UseValue(true))
                                                            .ForMember(dest => dest.IsContextMenuOpen,  opt => opt.Ignore())
                                                            .ForMember(dest => dest.IsExpanded,         opt => opt.Ignore())
                                                            .ForMember(dest => dest.IsMultiSelected,    opt => opt.Ignore())
                                                            .ForMember(dest => dest.IsSelected,         opt => opt.Ignore())
                                                            .ForMember(dest => dest.HasError,           opt => opt.Ignore())
                                                            .ForMember(dest => dest.Parent,             opt => opt.Ignore());

      config.CreateMap<DomainFolder, FolderViewEntity>().ForMember(dest => dest.Folder,             opt => opt.MapFrom(src => src))
                                                        .ForMember(dest => dest.IsChecked,          opt => opt.MapFrom(src => src.Settings.IsChecked))
                                                        .ForMember(dest => dest.IsCollectionFolder, opt => opt.UseValue(false))
                                                        .ForMember(dest => dest.Children,           opt => opt.UseValue(new ObservableCollection<FolderViewEntity>()))
                                                        .ForMember(dest => dest.IsContextMenuOpen,  opt => opt.Ignore())
                                                        .ForMember(dest => dest.IsExpanded,         opt => opt.Ignore())
                                                        .ForMember(dest => dest.IsMultiSelected,    opt => opt.Ignore())
                                                        .ForMember(dest => dest.IsSelected,         opt => opt.Ignore())
                                                        .ForMember(dest => dest.HasError,           opt => opt.Ignore())
                                                        .ForMember(dest => dest.Parent,             opt => opt.Ignore());
    }

    #endregion Private Methods

  }

}
