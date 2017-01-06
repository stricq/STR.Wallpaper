using System.Collections.Generic;

using Str.Wallpaper.Domain.Models;

using STR.Common.Messages;


namespace Str.Wallpaper.Wpf.Messages.Folders {

  public class CollectionsChangedMessage : MessageBase {

    public List<DomainCollection> Collections { get; set; }

  }

}
