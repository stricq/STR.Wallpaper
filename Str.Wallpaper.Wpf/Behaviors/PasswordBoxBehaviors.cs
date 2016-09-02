using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;


namespace Str.Wallpaper.Wpf.Behaviors {

  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public static class PasswordBoxBehaviors {

    #region Dependency Properties

    #region Attach Property

    public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxBehaviors), new PropertyMetadata(false, onAttachPropertyChanged));

    public static bool GetAttach(DependencyObject dp) {
      return (bool)dp.GetValue(AttachProperty);
    }

    public static void SetAttach(DependencyObject dp, bool value) {
      dp.SetValue(AttachProperty, value);
    }

    private static void onAttachPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
      PasswordBox passwordBox = sender as PasswordBox;

      if (passwordBox == null) return;

      if ((bool)e.OldValue) passwordBox.PasswordChanged -= onPasswordChanged;
      if ((bool)e.NewValue) passwordBox.PasswordChanged += onPasswordChanged;
    }

    #endregion Attach Property

    #region Password Property

    public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxBehaviors), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, onPasswordPropertyChanged));

    public static string GetPassword(DependencyObject dp) {
      return dp.GetValue(PasswordProperty) as string;
    }

    public static void SetPassword(DependencyObject dp, string value) {
      dp.SetValue(PasswordProperty, value);
    }

    private static void onPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
      PasswordBox passwordBox = sender as PasswordBox;

      if (passwordBox == null) return;

      passwordBox.PasswordChanged -= onPasswordChanged;

      if (!GetIsUpdating(passwordBox)) passwordBox.Password = e.NewValue as string;

      passwordBox.PasswordChanged += onPasswordChanged;
    }

    private static void onPasswordChanged(object sender, RoutedEventArgs e) {
      PasswordBox passwordBox = sender as PasswordBox;

      if (passwordBox == null) return;

      SetIsUpdating(passwordBox, true);

      SetPassword(passwordBox, passwordBox.Password);

      SetIsUpdating(passwordBox, false);
    }

    #endregion Password Property

    #region IsUpdating Property

    private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),   typeof(PasswordBoxBehaviors));

    private static bool GetIsUpdating(DependencyObject dp) {
      return (bool)dp.GetValue(IsUpdatingProperty);
    }

    private static void SetIsUpdating(DependencyObject dp, bool value) {
      dp.SetValue(IsUpdatingProperty, value);
    }

    #endregion IsUpdating Property

    #endregion Dependency Properties

  }

}
