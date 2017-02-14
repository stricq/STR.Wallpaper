using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using Str.Wallpaper.Domain.Models;

using Str.Wallpaper.Wpf.Messages.Collections;

using STR.DialogView.Domain.Contracts;

using STR.MvvmCommon;


namespace Str.Wallpaper.Wpf.ViewModels.Dialogs {

  [Export]
  [ViewModel("CollectionEditorViewModel")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class CollectionEditorViewModel : ObservableObject, IDialogViewModel {

    #region Private Fields

    private CollectionEditMessage message;

    private DomainCollection collection;

    private RelayCommandAsync ok;
    private RelayCommandAsync cancel;

    #endregion Private Fields

    #region Properties

    public CollectionEditMessage Message {
      get { return message; }
      set { SetField(ref message, value, () => Message); }
    }

    public DomainCollection Collection {
      get { return collection; }
      set { SetField(ref collection, value, () => Collection); }
    }

    public RelayCommandAsync Ok {
      get { return ok; }
      set { SetField(ref ok, value, () => Ok); }
    }

    public RelayCommandAsync Cancel {
      get { return cancel; }
      set { SetField(ref cancel, value, () => Cancel); }
    }

    #endregion Properties

  }

}
