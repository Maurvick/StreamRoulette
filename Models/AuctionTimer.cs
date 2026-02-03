using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace StreamRoulette.Models
{
	public class AuctionTimer : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private DispatcherTimer Timer;
		private long TicksEnd;
		private long SavedValue;
		private bool _IsRunning;

		public bool IsRunning => _IsRunning;

		public AuctionTimer()
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
			TicksEnd = Environment.TickCount64 + SavedValue;
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
					// using TimeSpan as standard representation of time
					if (diff < 0) { Stop(); SavedValue = 0; return TimeSpan.Zero; }
					return TimeSpan.FromMilliseconds(diff * 10);
				}
				return TimeSpan.FromMilliseconds(SavedValue * 10);
			}
			set
			{
				SavedValue = (long)value.TotalMilliseconds / 10;
				if (IsRunning) TicksEnd = Environment.TickCount64 + SavedValue;
				NotifyPropertyChanged();
			}
		}

		private void NotifyPropertyChanged([CallerMemberName] string prop = "") =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
	}
}