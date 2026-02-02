using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace StreamRoulette.Models
{
	public class AuctionTimerModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private DispatcherTimer Timer;
		private long TicksEnd;
		private long SavedValue;
		private bool _IsRunning;
		private const int MaxValue = 60 * 60 * 1000;

		public bool IsRunning => _IsRunning;

		public AuctionTimerModel()
		{
			Timer = new DispatcherTimer();
			Timer.Tick += Timer_Tick;
			Timer.Interval = TimeSpan.FromMilliseconds(100);
			_IsRunning = false;
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			NotifyPropertyChanged(nameof(Time));
		}

		public void Start()
		{
			if (_IsRunning) return;
			Timer.Start();
			_IsRunning = true;
			TicksEnd = Environment.TickCount64 + SavedValue; // Використовуйте TickCount64
			NotifyPropertyChanged(nameof(Time));
		}

		public void Stop()
		{
			if (!_IsRunning) return;
			Timer.Stop();
			_IsRunning = false;
			SavedValue = TicksEnd - Environment.TickCount64;
			NotifyPropertyChanged(nameof(Time));
		}

		public TimeSpan Time
		{
			get
			{
				if (IsRunning)
				{
					long diff = TicksEnd - Environment.TickCount64;
					if (diff < 0) { Stop(); SavedValue = 0; return TimeSpan.Zero; }
					return TimeSpan.FromMilliseconds(diff * 10); // Ваша логіка з UWP (x10000 ticks)
																 // УВАГА: У вашому оригіналі дивна логіка часу (Ticks / 10000). 
																 // Я адаптував під стандартний TimeSpan, перевірте коефіцієнти.
				}
				return TimeSpan.FromMilliseconds(SavedValue * 10);
			}
			set
			{
				// Логіка встановлення часу
				SavedValue = (long)value.TotalMilliseconds / 10;
				if (IsRunning) TicksEnd = Environment.TickCount64 + SavedValue;
				NotifyPropertyChanged();
			}
		}

		private void NotifyPropertyChanged([CallerMemberName] string prop = "") =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
	}
}