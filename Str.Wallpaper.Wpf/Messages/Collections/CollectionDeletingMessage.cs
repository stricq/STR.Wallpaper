using Str.Wallpaper.Domain.Models;

using STR.Common.Messages;


namespace Str.Wallpaper.Wpf.Messages.Collections {

  internal sealed class CollectionDeletingMessage : MessageBase {

    public CollectionDeletingMessage() : base(true) { }

    public DomainCollection Collection { get; set; }

  }

}
