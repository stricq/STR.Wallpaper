using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using Str.Wallpaper.Wpf.ViewEntities;

using STR.DialogView.Domain.Contracts;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewModels.Dialogs {

  [Export]
  [ViewModel("OptionsViewModel")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class OptionsViewModel : ObservableObject, IDialogViewModel {

    #region Private Fields

    private RelayCommand selectCacheDirectory;
    private RelayCommand cancel;

    private RelayCommandAsync save;
    private RelayCommandAsync serverLogin;
    private RelayCommandAsync serverDisconnect;

    private ProgramSettingsViewEntity settings;

    private UserSettingsViewEntity user;

    #endregion Private Fields

    #region Properties

    public RelayCommand SelectCacheDirectory {
      get { return selectCacheDirectory; }
      set { SetField(ref selectCacheDirectory, value, () => SelectCacheDirectory); }
    }

    public RelayCommand Cancel {
      get { return cancel; }
      set { SetField(ref cancel, value, () => Cancel); }
    }

    public RelayCommandAsync Save {
      get { return save; }
      set { SetField(ref save, value, () => Save); }
    }

    public RelayCommandAsync ServerLogin {
      get { return serverLogin; }
      set { SetField(ref serverLogin, value, () => ServerLogin); }
    }

    public RelayCommandAsync ServerDisconnect {
      get { return serverDisconnect; }
      set { SetField(ref serverDisconnect, value, () => ServerDisconnect); }
    }

    public ProgramSettingsViewEntity Settings {
      get { return settings; }
      set { SetField(ref settings, value, () => Settings); }
    }

    public UserSettingsViewEntity User {
      get { return user; }
      set { SetField(ref user, value, () => User); }
    }

    #endregion Properties

  }

}
