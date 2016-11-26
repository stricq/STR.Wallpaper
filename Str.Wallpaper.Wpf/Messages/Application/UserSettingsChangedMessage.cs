using Str.Wallpaper.Domain.Models;

using STR.Common.Messages;


namespace Str.Wallpaper.Wpf.Messages.Application {

  internal sealed class UserSettingsChangedMessage : MessageBase {

    public DomainUserSettings UserSettings { get; set; }

  }
}
