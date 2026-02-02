using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Newtonsoft.Json;

namespace StreamRoulette.Models
{
	public interface ILot
	{
		string Name { get; set; }
		string Amount { get; set; }
		string Addition { get; set; }
		double Chance { get; }
		Color Color { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class Lot : ILot, INotifyPropertyChanged
	{
		private string _Name = "";
		private string _Amount = "";
		private int? _AmountValue = 0;
		private string _Addition = "";
		private int? _AdditionValue = 0;
		private double _Chance = 0;
		private Color _Color;

		public event PropertyChangedEventHandler PropertyChanged;

		public Lot()
		{
			MakeRandomColor();
		}

		public double Chance
		{
			get => _Chance;
			internal set { _Chance = value; NotifyPropertyChanged(); }
		}

		[JsonProperty]
		public string Name
		{
			get => _Name;
			set { _Name = value; NotifyPropertyChanged(); }
		}

		[JsonProperty]
		public string Amount
		{
			get => _Amount;
			set
			{
				_Amount = value;
				_AmountValue = ParseNumber(value);
				NotifyPropertyChanged();
			}
		}

		public int? AmountValue => _AmountValue;

		[JsonProperty]
		public string Addition
		{
			get => _Addition;
			set
			{
				_Addition = value;
				_AdditionValue = ParseNumber(value);
				NotifyPropertyChanged();
			}
		}

		public int? AdditionValue => _AdditionValue;

		[JsonProperty]
		public Color Color
		{
			get => _Color;
			set { _Color = value; NotifyPropertyChanged(); }
		}

		internal void MakeRandomColor()
		{
			var random = new Random();
			_Color = ColorFromHSV(random.NextDouble() * 360, random.NextDouble() * 0.5 + 0.5, 1.0);
			NotifyPropertyChanged(nameof(Color));
		}

		private static Color ColorFromHSV(double hue, double saturation, double value)
		{
			int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
			double f = hue / 60 - Math.Floor(hue / 60);

			value = value * 255;
			byte v = Convert.ToByte(value);
			byte p = Convert.ToByte(value * (1 - saturation));
			byte q = Convert.ToByte(value * (1 - f * saturation));
			byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

			if (hi == 0) return Color.FromRgb(v, t, p);
			else if (hi == 1) return Color.FromRgb(q, v, p);
			else if (hi == 2) return Color.FromRgb(p, v, t);
			else if (hi == 3) return Color.FromRgb(p, q, v);
			else if (hi == 4) return Color.FromRgb(t, p, v);
			else return Color.FromRgb(v, p, q);
		}

		private int? ParseNumber(string Str)
		{
			if (string.IsNullOrEmpty(Str)) return 0;
			if (double.TryParse(Str, NumberStyles.Float, CultureInfo.CurrentCulture, out double value)) return (int)value;
			if (double.TryParse(Str, NumberStyles.Float, CultureInfo.InvariantCulture, out value)) return (int)value;
			return null;
		}

		internal void IncreaseRate()
		{
			if (AmountValue != null && AdditionValue != null)
			{
				var NewAmount = AmountValue + AdditionValue;
				Addition = "";
				Amount = NewAmount != 0 ? NewAmount.ToString() : "";
			}
		}

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	internal class LotComparer : IComparer<Lot>
	{
		public int Compare(Lot x, Lot y)
		{
			if (x.AmountValue == null)
				return 1;

			if (y.AmountValue == null)
				return -1;

			return (int)y.AmountValue - (int)x.AmountValue;
		}
	}

	internal class ReverseLotComparer : IComparer<Lot>
	{
		public int Compare(Lot x, Lot y)
		{
			// 1. Захист від відсутніх об'єктів
			if (x == null && y == null) return 0;
			if (x == null) return 1;
			if (y == null) return -1;

			// 2. Якщо значення однакові - повертаємо 0 (важливо для стабільності сортування)
			if (x.AmountValue == y.AmountValue) return 0;

			// 3. Обробка пустих значень (null)
			if (x.AmountValue == null) return 1; // Пусті вниз
			if (y.AmountValue == null) return -1;

			// 4. Специфічна логіка для 0 (як в оригіналі)
			if ((int)x.AmountValue == 0) return 1;
			if ((int)y.AmountValue == 0) return -1;

			// 5. Зворотнє сортування (Інверсія)
			// Використовуємо порівняння значень, щоб уникнути переповнення при відніманні
			return ((int)x.AmountValue).CompareTo((int)y.AmountValue);
		}
	}


	public enum AuctionMode { Normal, Reversed, Roulette }

	[JsonObject(MemberSerialization.OptIn)]
	public class AuctionModel : INotifyPropertyChanged
	{
		[JsonProperty("Items")]
		private readonly ObservableCollection<ILot> _Items;
		private readonly ReadOnlyObservableCollection<ILot> _ReadOnlyItems;
		private readonly ObservableCollection<ILot> _SortedItems;
		private readonly ReadOnlyObservableCollection<ILot> _ReadOnlySortedItems;
		private int _Bank;
		private AuctionMode _Mode;

		public event PropertyChangedEventHandler PropertyChanged;

		public AuctionModel()
		{
			_Items = new ObservableCollection<ILot>();
			_ReadOnlyItems = new ReadOnlyObservableCollection<ILot>(_Items);
			_SortedItems = new ObservableCollection<ILot>();
			_ReadOnlySortedItems = new ReadOnlyObservableCollection<ILot>(_SortedItems);
			_Items.CollectionChanged += (s, e) => { RebuildSortedItems(); UpdateItems(); };
		}

		public int Bank => _Bank;

		[JsonProperty]
		public AuctionMode Mode
		{
			get => _Mode;
			set { _Mode = value; RebuildSortedItems(); NotifyPropertyChanged(); }
		}

		public ReadOnlyObservableCollection<ILot> SortedItems => _ReadOnlySortedItems;
		public ReadOnlyObservableCollection<ILot> Items => _ReadOnlyItems;

		public void Clear() => _Items.Clear();

		public ILot Add()
		{
			var lot = new Lot();
			_Items.Add(lot);
			UpdateItems();
			return lot;
		}

		public void Delete(ILot item) => _Items.Remove(item);

		public void IncreaseRate(ILot item)
		{
			if (item is Lot l)
			{
				l.IncreaseRate();
				RebuildSortedItems();
				UpdateItems();
			}
		}

		public void MakeRandomColor(ILot item) => (item as Lot)?.MakeRandomColor();

		private void RebuildSortedItems()
		{
			IComparer<Lot> comparer;

			if (Mode != AuctionMode.Reversed)
				comparer = new LotComparer();
			else
				comparer = new ReverseLotComparer();

			// Сортуємо список (безпечно, з перевіркою на null)
			var sortedList = _Items.Where(x => x != null).OrderBy(x => x as Lot, comparer).ToList();

			// Синхронізуємо колекцію _SortedItems
			// Найбезпечніший спосіб для WPF, щоб не ламався UI:
			_SortedItems.Clear();
			foreach (var item in sortedList)
			{
				_SortedItems.Add(item);
			}
		}

		private void UpdateItems()
		{
			_Bank = 0;
			int positiveBank = 0;
			foreach (var item in _Items)
			{
				if (item is Lot l && l.AmountValue != null)
				{
					_Bank += Math.Abs((int)l.AmountValue);
					positiveBank += Math.Max(0, (int)l.AmountValue);
				}
			}
			foreach (var item in _Items)
			{
				if (item is Lot l)
				{
					l.Chance = (l.AmountValue != null && positiveBank > 0)
						? Math.Max(0, (double)l.AmountValue / positiveBank) : 0;
				}
			}
			NotifyPropertyChanged(nameof(Bank));
		}

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}