using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Str.Wallpaper.Domain.Models;


namespace Str.Wallpaper.Domain.Contracts {

  public interface IFolderService {

    Task<bool> CheckFilenameExistsAsync(DomainCollection Collection, string Filename);

    Task LoadFoldersAsync(DomainUser User, DomainCollection Collection, DomainFolder Parent, Action<string, string, List<DomainFolder>, Exception> Callback);

    Task SaveFolderAsync(DomainUser User, DomainFolder Folder);

    Task SaveFoldersAsync(DomainUser User, List<DomainFolder> Folders);

    Task DeleteFolderAsync(DomainUser User, DomainFolder Folder);

    Task DeleteFolderChildrenAsync(DomainUser User, DomainFolder ParentFolder);

    Task<int> CountImageReferencesAsync(DomainFolder Folder);

  }

}
