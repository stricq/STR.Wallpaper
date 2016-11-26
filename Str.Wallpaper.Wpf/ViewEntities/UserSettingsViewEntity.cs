using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewEntities {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class UserSettingsViewEntity : ObservableObject {

    #region Private Fields

    private bool areSettingsChanged;

    private bool isOnline;

    private string username;
    private string password;

    #endregion Private Fields

    #region Properties

    public bool AreSettingsChanged {
      get { return areSettingsChanged; }
      set { SetField(ref areSettingsChanged, value, () => AreSettingsChanged); }
    }

    public bool IsOnline {
      get { return isOnline; }
      set { SetField(ref isOnline, value, () => IsOnline, () => IsNotOnline); }
    }

    public bool IsNotOnline => !isOnline;

    public string Username {
      get { return username; }
      set { AreSettingsChanged |= SetField(ref username, value, () => Username); }
    }

    public string Password {
      get { return password; }
      set { AreSettingsChanged |= SetField(ref password, value, () => Password); }
    }

    #endregion Properties

  }

}
