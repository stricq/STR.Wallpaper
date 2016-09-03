using Str.Wallpaper.Wpf.ViewEntities;

using STR.Common.Messages;


namespace Str.Wallpaper.Wpf.Messages.Application {

  internal sealed class ApplicationSettingsChangedMessage : MessageBase {

    public ProgramSettingsViewEntity Settings { get; set; }

  }

}
