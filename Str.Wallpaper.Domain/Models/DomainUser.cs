using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Str.Wallpaper.Domain.Contracts;


namespace Str.Wallpaper.Domain.Models {

  public sealed class DomainUser {

    #region Private Fields

    private readonly IUserSettingsRepository userRepository;

    private readonly IUserSessionService sessionService;

    #endregion Private Fields

    #region Constructor

    // ReSharper disable once UnusedMember.Global
    public DomainUser() { } // Needed by AutoMapper but never actually used...

    public DomainUser(IUserSettingsRepository UserRepository, IUserSessionService SessionService) {
      userRepository = UserRepository;
      sessionService = SessionService;

      SelectedCollections = new List<string>();
    }

    #endregion Constructor

    #region Properties

    public string Id { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public List<string> SelectedCollections { get; set; }

    #endregion Properties

    #region Domain Properties

    public bool AreSettingsChanged { get; set; }

    public bool IsLoggingIn { get; set; }

    public bool IsOnline => SessionId != null;

    public Guid? SessionId { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public async Task LoadUserSettingsAsync() {
      await userRepository.LoadUserSettingsAsync(this);
    }

    public async Task SaveUserSettingsAsync() {
      await userRepository.SaveUserSettingsAsync(this);

      AreSettingsChanged = false;
    }

    public async Task<bool> CreateUserAsync() {
      return await sessionService.CreateUserAsync(this);
    }

    public async Task<bool> LoginAsync() {
      return await sessionService.LoginAsync(this);
    }

    public async Task DisconnectAsync() {
      await sessionService.DisconnectAsync(this);

      if (AreSettingsChanged) await SaveUserSettingsAsync();

      SessionId = null;
    }

    #endregion Domain Methods

  }

}
