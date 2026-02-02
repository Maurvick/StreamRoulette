using System.IO;
using System.Windows;
using System.Windows.Input;
using StreamRoulette.Models;
using Newtonsoft.Json;

namespace StreamRoulette
{
	public partial class MainWindow : Window
	{
		// Таймер у WPF знаходиться в System.Windows.Threading
		public AuctionTimerModel Timer { get; } = new AuctionTimerModel();
		public AuctionModel Auction { get; } = new AuctionModel();

		// Файл для збереження в AppData
		private readonly string StateFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrazzzyAuction_State.json");

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			LoadState();

			if (Auction.Items.Count == 0) ClearAll();

			// Встановлюємо таймер за замовчуванням
			Timer.Time = new TimeSpan(0, 10, 0);

			Closing += MainWindow_Closing;
		}

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveState();
		}

		private void SaveState()
		{
			try
			{
				var json = JsonConvert.SerializeObject(Auction, Formatting.Indented);
				File.WriteAllText(StateFile, json);
			}
			catch { /* ігноруємо помилки запису */ }
		}

		private void LoadState()
		{
			try
			{
				if (File.Exists(StateFile))
				{
					var json = File.ReadAllText(StateFile);
					var loaded = JsonConvert.DeserializeObject<AuctionModel>(json);
					if (loaded != null)
					{
						// Копіюємо дані, оскільки Auction вже створено
						Auction.Clear();
						foreach (var item in loaded.Items) Auction.Add(); // Додаємо логіку перенесення даних якщо потрібно
																		  // Простіше було б присвоїти Auction = loaded, але тоді треба оновити DataContext або реалізувати INPC для властивості Auction
					}
				}
			}
			catch { }
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

		// Обробники подій

		private void Button_Add_Click(object sender, RoutedEventArgs e) => Auction.Add();

		private void Button_Clear_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Очистити все? Це видалить усі лоти.", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				ClearAll();
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

		// Для Addition
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

		// Таймер
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

		private void Button_Add1Time_Click(object sender, RoutedEventArgs e) => Timer.Time += TimeSpan.FromMinutes(1);
		private void Button_Add2Time_Click(object sender, RoutedEventArgs e) => Timer.Time += TimeSpan.FromMinutes(2);
		private void Button_Set10Time_Click(object sender, RoutedEventArgs e) { Timer.Time = TimeSpan.FromMinutes(10); Timer.Stop(); }
		private void Button_Sub1Time_Click(object sender, RoutedEventArgs e) => Timer.Time -= TimeSpan.FromMinutes(1);
	}
}