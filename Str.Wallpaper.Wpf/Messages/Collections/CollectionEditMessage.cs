using System;
using System.Threading.Tasks;

using Str.Wallpaper.Domain.Models;

using STR.Common.Messages;


namespace Str.Wallpaper.Wpf.Messages.Collections {

  public class CollectionEditMessage : MessageBase {

    public CollectionEditMessage() : base(true) { }

    public bool IsCancel { get; set; }

    public DomainCollection Collection { get; set; }

    public DomainCollection OriginalCollection { get; set; }

    public string Title { get; set; }

    public Func<CollectionEditMessage, Task> CallbackAsync { get; set; }

  }

}
