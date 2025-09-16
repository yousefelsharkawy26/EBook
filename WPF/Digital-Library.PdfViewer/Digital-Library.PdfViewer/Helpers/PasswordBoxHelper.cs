using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace Digital_Library.PdfViewer.Helpers;

public static class PasswordBoxHelper
{
    public static readonly DependencyProperty BoundPasswordProperty =
        DependencyProperty.RegisterAttached(
            "BoundPassword",
            typeof(SecureString),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(null, OnBoundPasswordChanged));

    public static SecureString GetBoundPassword(DependencyObject d)
    {
        return (SecureString)d.GetValue(BoundPasswordProperty);
    }

    public static void SetBoundPassword(DependencyObject d, SecureString value)
    {
        d.SetValue(BoundPasswordProperty, value);
    }

    private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox box)
        {
            box.PasswordChanged -= HandlePasswordChanged;
            box.PasswordChanged += HandlePasswordChanged;
        }
    }

    private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox box)
        {
            SetBoundPassword(box, box.SecurePassword);
        }
    }
}