using System.Collections.Generic;

using Str.Wallpaper.Domain.Models;

using STR.Common.Messages;


namespace Str.Wallpaper.Wpf.Messages.Collections {

  internal sealed class CollectionsChangedMessage : MessageBase {

    public List<DomainCollection> Collections { get; set; }

  }

}
