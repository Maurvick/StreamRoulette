using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StreamRoulette.Models;

namespace StreamRoulette
{
	public partial class App : Application
	{
		private static AuctionModel _AuctionModel = new AuctionModel();

		// Сінглтон моделі
		public static AuctionModel AuctionModel => _AuctionModel;

		// Шлях до файлу (AppData/Local/CrazzzyAuction/state.txt)
		private static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrazzzyAuction");
		private static readonly string StateFileName = Path.Combine(AppDataPath, "state.txt");

		protected override void OnStartup(StartupEventArgs e)
		{
			// Глобальний перехоплювач помилок
			DispatcherUnhandledException += App_DispatcherUnhandledException;

			base.OnStartup(e);

			// Створюємо папку, якщо її немає
			if (!Directory.Exists(AppDataPath))
			{
				Directory.CreateDirectory(AppDataPath);
			}

			LoadState();

			// Запускаємо головне вікно
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
			// Показати повідомлення про помилку
			MessageBox.Show($"An error occured: {e.Exception.Message}\n\n{e.Exception.StackTrace}",
							"Critical error",
							MessageBoxButton.OK,
							MessageBoxImage.Error);

			// Запобігти закриттю програми (якщо помилка не фатальна)
			e.Handled = true;
		}

		private void LoadState()
		{
			try
			{
				if (File.Exists(StateFileName))
				{
					var text = File.ReadAllText(StateFileName);

					// Використовуємо налаштування для збереження типів, якщо потрібно, або просто десеріалізацію
					var auctionModel = JsonConvert.DeserializeObject<AuctionModel>(text);
					// Примітка: ConverterILotToLot може знадобитися, якщо AuctionModel використовує інтерфейси

					if (auctionModel != null)
					{
						// Тут ми не можемо просто замінити об'єкт _AuctionModel, 
						// тому ми копіюємо дані в існуючий сінглтон.
						// Або, як варіант, зробіть сеттер для _AuctionModel (менш безпечно для прив'язок).

						_AuctionModel.Clear();
						// Тут потрібна логіка копіювання items з auctionModel в _AuctionModel
						// Для простоти, якщо AuctionModel дозволяє:
						foreach (var item in auctionModel.Items)
						{
							// Це спрацює, якщо ви реалізували Add(item) або доступ до колекції
							_AuctionModel.Add(); // Спрощено, тут треба переносити властивості
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Логування помилки
				System.Diagnostics.Debug.WriteLine($"Error loading state: {ex.Message}");
			}
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