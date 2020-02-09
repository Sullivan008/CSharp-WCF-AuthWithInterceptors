using System;
using System.Windows;

namespace ClientForAuthenticationAndAuthorization.Dialogs
{
    public class NotificationDialog
    {
        private string Content { get; }
        private string Title { get; }

        public NotificationDialog(string content, string title)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Title = title ?? throw new ArgumentNullException(nameof(content));
        }

        public MessageBoxResult ShowErrorMessageBox() =>
            MessageBox.Show(Content, Title, MessageBoxButton.OK, MessageBoxImage.Error);

        public MessageBoxResult ShowInfoMessageBox() =>
            MessageBox.Show(Content, Title, MessageBoxButton.OK, MessageBoxImage.Information);
    }
}