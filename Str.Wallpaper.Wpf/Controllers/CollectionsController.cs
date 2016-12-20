using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Str.Wallpaper.Wpf.ViewEntities;
using Str.Wallpaper.Wpf.ViewModels;

using STR.MvvmCommon.Contracts;


namespace Str.Wallpaper.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class CollectionsController : IController {

    #region Private Fields

    private readonly CollectionsViewModel viewModel;

    private readonly IMessenger messenger;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public CollectionsController(CollectionsViewModel ViewModel, IMessenger Messenger) {
      viewModel = ViewModel;

      viewModel.Collections = new ObservableCollection<CollectionViewEntity>();

      messenger = Messenger;
    }

    #endregion Constructor

    #region IController Implementation

    public int InitializePriority { get; } = 100;

    public Task InitializeAsync() {
      return Task.CompletedTask;
    }

    #endregion IController Implementation

  }

}
