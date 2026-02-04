using System.Windows;
using System.Windows.Input;

namespace StreamRoulette
{
	public partial class CustomMessageBox : Window
	{
		public CustomMessageBox(string message, string title, bool isConfirmation)
		{
			InitializeComponent();
			TxtMessage.Text = message;
			TxtTitle.Text = title;

			if (isConfirmation)
			{
				BtnYes.Visibility = Visibility.Visible;
				BtnNo.Visibility = Visibility.Visible;
				BtnYes.Content = (string)FindResource("Str_Msg_YesConfirm");
				BtnNo.Content = (string)FindResource("Str_Msg_NoConfirm");
			}
			else
			{
				// Info mode: Show only "OK"
				BtnYes.Content = "OK";
				BtnNo.Visibility = Visibility.Collapsed;
			}
		}

		public static bool Show(string message, string title, bool isConfirmation = false)
		{
			var msgBox = new CustomMessageBox(message, title, isConfirmation);
			return msgBox.ShowDialog() == true;
		}

		private void BtnYes_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void BtnClose_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		// Allow dragging the window by the title bar
		private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				DragMove();
		}
	}
}