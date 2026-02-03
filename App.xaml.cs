using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StreamRoulette.Models;

namespace StreamRoulette
{
	public partial class App : Application
	{
		private static Auction _AuctionModel = new Auction();

		// Use singleton pattern for Auction model
		public static Auction AuctionModel => _AuctionModel;

		// Save app state locally (AppData/Local/CrazzzyAuction/state.txt)
		private static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrazzzyAuction");
		private static readonly string StateFileName = Path.Combine(AppDataPath, "state.txt");

		protected override void OnStartup(StartupEventArgs e)
		{
			// Create global exception handler
			DispatcherUnhandledException += App_DispatcherUnhandledException;

			base.OnStartup(e);

			if (!Directory.Exists(AppDataPath))
			{
				Directory.CreateDirectory(AppDataPath);
			}

			LoadState();

			MainWindow mainWindow = new MainWindow();
			mainWindow.Show();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			SaveState();
			base.OnExit(e);
		}

		private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show($"An error occured: {e.Exception.Message}\n\n{e.Exception.StackTrace}",
							"Critical error",
							MessageBoxButton.OK,
							MessageBoxImage.Error);

			e.Handled = true;
		}

		public void ChangeLanguage(string langCode)
		{
			// Save the choice in the model (to write to file later)
			AuctionModel.CurrentLanguage = langCode;

			ResourceDictionary dict = new ResourceDictionary();
			try
			{
				dict.Source = new Uri($"Resources/Lang/Lang.{langCode}.xaml", UriKind.Relative);
			}
			catch
			{
				return;
			}

			ResourceDictionary oldDict = null;
			foreach (ResourceDictionary d in Resources.MergedDictionaries)
			{
				if (d.Source != null && d.Source.OriginalString.Contains("Resources/Lang/"))
				{
					oldDict = d;
					break;
				}
			}

			if (oldDict != null)
			{
				Resources.MergedDictionaries.Remove(oldDict);
			}

			Resources.MergedDictionaries.Add(dict);
		}

		private void LoadState()
		{
			try
			{
				if (File.Exists(StateFileName))
				{
					var text = File.ReadAllText(StateFileName);
					var auctionModel = JsonConvert.DeserializeObject<Auction>(text);

					if (auctionModel != null)
					{
						// We cannot just replace the _AuctionModel object, 
						// so we copy the data into the existing singleton.
						_AuctionModel.Clear();
						foreach (var item in auctionModel.Items)
						{
							_AuctionModel.Add();
						}
						_AuctionModel.CurrentLanguage = auctionModel.CurrentLanguage;
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error loading state: {ex.Message}");
			}

			// Use the loaded language immediately after loading data
			ChangeLanguage(_AuctionModel.CurrentLanguage);
		}

		private void SaveState()
		{
			try
			{
				var json = JsonConvert.SerializeObject(AuctionModel, Formatting.Indented, new StringEnumConverter());
				File.WriteAllText(StateFileName, json);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error saving state: {ex.Message}");
			}
		}
	}
}