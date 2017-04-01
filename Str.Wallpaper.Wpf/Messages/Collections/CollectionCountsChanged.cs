using System.Collections.Generic;

using Str.Wallpaper.Domain.Models;

using STR.Common.Messages;


namespace Str.Wallpaper.Wpf.Messages.Collections {

  public class CollectionCountsChanged : MessageBase {

    public CollectionCountsChanged() : base(true) { }

    public List<DomainCollection> Collections { get; set; }

  }

}
