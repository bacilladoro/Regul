using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Regul.OlibUI;
using System.Threading.Tasks;

namespace Regul.Views
{
    public class MessageBox : OlibModalWindow
    {
        public enum MessageBoxButtons
        {
            Ok,
            OkCancel,
            YesNo,
            YesNoCancel,
            NoNoToAllYesYesToAllAbandon
        }
        public enum MessageBoxIcon
        {
            Information,
            Error,
            Warning,
            Question
        }
        public enum MessageBoxResult
        {
            Ok,
            Cancel,
            Yes,
            No,
            YesToAll,
            NoToAll,
            Abandon
        }

        public MessageBox() => AvaloniaXamlLoader.Load(this);

        public static Task<MessageBoxResult> Show(Window parent, string textException, string text, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox msgbox = new MessageBox
            {
                Title = title
            };
            msgbox.FindControl<TextBlock>("Text").Text = text;
            StackPanel buttonPanel = msgbox.FindControl<StackPanel>("Buttons");
            Path iconControl = msgbox.FindControl<Path>("Icon");
            TextBox errorText = msgbox.FindControl<TextBox>("ErrorText");

            MessageBoxResult res = MessageBoxResult.Ok;

            void AddButton(string caption, MessageBoxResult r, bool def = false)
            {
                Button btn = new Button { Content = caption };
                btn.Click += (_, __) =>
                {
                    res = r;
                    msgbox.Close();
                };
                buttonPanel.Children.Add(btn);
                if (def)
                    res = r;
            }

            void ChangeIcon(string icon) => iconControl.Data = (Geometry)Application.Current.FindResource($"{icon}Icon");

            switch (buttons)
            {
                case MessageBoxButtons.Ok:
                case MessageBoxButtons.OkCancel:
                    msgbox.KeyDown += (s, e) =>
                    {
                        if (e.Key == Key.Enter)
                        {
                            res = MessageBoxResult.Ok;
                            msgbox.Close();
                        }
                    };
                    AddButton((string)Application.Current.FindResource("OK"), MessageBoxResult.Ok, true);
                    break;
                case MessageBoxButtons.YesNo:
                case MessageBoxButtons.YesNoCancel:
                    AddButton((string)Application.Current.FindResource("Yes"), MessageBoxResult.Yes);
                    AddButton((string)Application.Current.FindResource("No"), MessageBoxResult.No, true);
                    break;
                case MessageBoxButtons.NoNoToAllYesYesToAllAbandon:
                    AddButton("No", MessageBoxResult.No);
                    AddButton("No to all", MessageBoxResult.NoToAll);
                    AddButton("Yes", MessageBoxResult.Yes);
                    AddButton("Yes to all", MessageBoxResult.YesToAll);
                    AddButton("Abandon", MessageBoxResult.Abandon);
                    break;
            }

            if (buttons == MessageBoxButtons.OkCancel || buttons == MessageBoxButtons.YesNoCancel)
                AddButton((string)Application.Current.FindResource("Cancel"), MessageBoxResult.Cancel, true);

            switch (icon)
            {
                case MessageBoxIcon.Error:
                    ChangeIcon("Error");
                    break;
                case MessageBoxIcon.Information:
                    ChangeIcon("Information");
                    break;
                case MessageBoxIcon.Question:
                    ChangeIcon("Question");
                    break;
                case MessageBoxIcon.Warning:
                    ChangeIcon("Warning");
                    break;
            }

            if (textException != null) errorText.Text = textException;
            else errorText.IsVisible = false;

            TaskCompletionSource<MessageBoxResult> tcs = new TaskCompletionSource<MessageBoxResult>();
            msgbox.Closed += delegate { _ = tcs.TrySetResult(res); };
            if (parent != null) _ = msgbox.ShowDialog(parent);
            else msgbox.Show();
            return tcs.Task;

        }
    }
}