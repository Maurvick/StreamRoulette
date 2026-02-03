using System.IO;
using System.Windows;
using System.Windows.Input;
using StreamRoulette.Models;
using Newtonsoft.Json;

namespace StreamRoulette
{
	public partial class MainWindow : Window
	{
		public AuctionTimer Timer { get; } = new AuctionTimer();
		public Auction Auction { get; } = new Auction();

		// Save state as file in AppData
		private readonly string StateFile = 
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
				"CrazzzyAuction_State.json");

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			LoadState();

			if (Auction.Items.Count == 0) ClearAll();

			// Set default timer to 10 minutes
			Timer.Time = new TimeSpan(0, 10, 0);

			Closing += MainWindow_Closing;
		}

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveState();
		}

		// == Custom Title Bar Buttons ==

		// Allow dragging the window by the title bar
		private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				DragMove();
		}

		private void BtnMinimize_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void BtnMaximize_Click(object sender, RoutedEventArgs e)
		{
			if (WindowState == WindowState.Maximized)
				WindowState = WindowState.Normal;
			else
				WindowState = WindowState.Maximized;
		}

		private void BtnClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		// == State Management ==

		private void SaveState()
		{
			try
			{
				var json = JsonConvert.SerializeObject(Auction, Formatting.Indented);
				File.WriteAllText(StateFile, json);
			}
			catch 
			{
				// Currently ignoring save errors
			}
		}

		private void LoadState()
		{
			try
			{
				if (File.Exists(StateFile))
				{
					var json = File.ReadAllText(StateFile);
					var loaded = JsonConvert.DeserializeObject<Auction>(json);
					if (loaded != null)
					{
						// Copy data since Auction is already created
						Auction.Clear();
						foreach (var item in loaded.Items) Auction.Add(); 
					}
				}
			}
			catch 
			{
				// Currently ignoring load errors
			}
		}

		private void ClearAll()
		{
			Auction.Clear();
			Auction.Add();
			Auction.Add();
			Auction.Add();
		}

		private Lot GetLotFromSender(object sender)
		{
			if (sender is FrameworkElement element && element.DataContext is Lot lot)
				return lot;
			return null;
		}

		// == Event Handlers ==

		private void Button_Add_Click(object sender, RoutedEventArgs e) => Auction.Add();

		// FIXME: Broken confirmation dialog localization due to Auction conflicting naming
		private void Button_Clear_Click(object sender, RoutedEventArgs e)
		{
			if (Auction.CurrentLanguage == "ua")
			{
				if (MessageBox.Show("Видалити всі рядки?", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					ClearAll();
				}
			}
			if (Auction.CurrentLanguage == "ru")
			{
				if (MessageBox.Show("Удалить все строки?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					ClearAll();
				}
			}
			if (Auction.CurrentLanguage == "en")
			{
				if (MessageBox.Show("Delete all rows?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					ClearAll();
				}
			}
		}

		private void Button_DeleteLot_Click(object sender, RoutedEventArgs e)
		{
			var lot = GetLotFromSender(sender);
			if (lot != null) Auction.Delete(lot);
		}

		private void Button_AddMoney_Click(object sender, RoutedEventArgs e)
		{
			Auction.IncreaseRate(GetLotFromSender(sender));
		}

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
			e.Handled = true;
		}

		private void TextBox_Amount_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;
				Auction.IncreaseRate(GetLotFromSender(sender));
			}
		}

		// Addition logic
		private void TextBox_Addition_PreviewKeyDown(object sender, KeyEventArgs e) => TextBox_Amount_PreviewKeyDown(sender, e);

		private void TextBox_Amount_LostFocus(object sender, RoutedEventArgs e)
		{
			Auction.IncreaseRate(GetLotFromSender(sender));
		}

		private void TextBox_Addition_LostFocus(object sender, RoutedEventArgs e) => TextBox_Amount_LostFocus(sender, e);

		private void Rectangle_Color_Tapped(object sender, MouseButtonEventArgs e)
		{
			Auction.MakeRandomColor(GetLotFromSender(sender));
		}

		// Timer
		private void Button_Start_Click(object sender, RoutedEventArgs e)
		{
			if (!Timer.IsRunning) Timer.Start();
			else Timer.Stop();
		}

		private void Button_ResetTime_Click(object sender, RoutedEventArgs e)
		{
			Timer.Stop();
			Timer.Time = TimeSpan.Zero;
		}

		private void BtnLangUA_Click(object sender, RoutedEventArgs e)
		{
			(Application.Current as App).ChangeLanguage("ua");
		}

		private void BtnLangRU_Click(object sender, RoutedEventArgs e)
		{
			(Application.Current as App).ChangeLanguage("ru");
		}

		private void BtnLangEN_Click(object sender, RoutedEventArgs e)
		{
			(Application.Current as App).ChangeLanguage("en");
		}

		// Timer adjustments
		private void Button_Add1Time_Click(object sender, RoutedEventArgs e) => Timer.Time += TimeSpan.FromMinutes(1);
		private void Button_Add2Time_Click(object sender, RoutedEventArgs e) => Timer.Time += TimeSpan.FromMinutes(2);
		private void Button_Set10Time_Click(object sender, RoutedEventArgs e) { Timer.Time = TimeSpan.FromMinutes(10); Timer.Stop(); }
		private void Button_Sub1Time_Click(object sender, RoutedEventArgs e) => Timer.Time -= TimeSpan.FromMinutes(1);

        private void Button_Click(object sender, RoutedEventArgs e)
        {

		}
	}
}