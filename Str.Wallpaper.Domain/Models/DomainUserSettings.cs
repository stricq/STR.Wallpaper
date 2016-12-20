using System;
using System.Threading.Tasks;

using Str.Wallpaper.Domain.Contracts;


namespace Str.Wallpaper.Domain.Models {

  public sealed class DomainUserSettings {

    #region Private Fields

    private readonly IUserSettingsRepository userRepository;

    private readonly IUserSessionService sessionService;

    #endregion Private Fields

    #region Constructor

    // ReSharper disable once UnusedMember.Global
    public DomainUserSettings() { } // Needed by AutoMapper but never actually used...

    public DomainUserSettings(IUserSettingsRepository UserRepository, IUserSessionService SessionService) {
      userRepository = UserRepository;
      sessionService = SessionService;
    }

    #endregion Constructor

    #region Properties

    public string Username { get; set; }

    public string Password { get; set; }

    #endregion Properties

    #region Domain Properties

    public bool IsLoggingIn { get; set; }

    public Guid? SessionId { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public async Task LoadUserSettingsAsync() {
      await userRepository.LoadUserSettingsAsync(this);
    }

    public async Task SaveUserSettingsAsync() {
      await userRepository.SaveUserSettingsAsync(this);
    }

    public async Task<bool> CreateUserAsync() {
      return await sessionService.CreateUserAsync(this);
    }

    public async Task<bool> LoginAsync() {
      return await sessionService.LoginAsync(this);
    }

    public async Task DisconnectAsync() {
      await sessionService.DisconnectAsync(this);

      SessionId = null;
    }

    #endregion Domain Methods

  }

}
