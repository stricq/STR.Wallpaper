using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Str.Wallpaper.Domain.Models;


namespace Str.Wallpaper.Domain.Contracts {

  public interface ICollectionService {

    Task<List<DomainCollection>> LoadCollectionsAsync(DomainUser User);

    Task<Tuple<int, int>> CountWallpapersAsync(DomainUser User, DomainCollection Collection);

    Task SaveCollectionAsync(DomainUser User, DomainCollection Collection);

    Task DeleteCollectionAsync(DomainUser User, DomainCollection Collection);

  }

}
