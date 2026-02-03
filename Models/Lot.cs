using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Newtonsoft.Json;

namespace StreamRoulette.Models
{
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
			if (double.TryParse(Str, NumberStyles.Float,
				CultureInfo.CurrentCulture, out double value)) return (int)value;
			if (double.TryParse(Str, NumberStyles.Float,
				CultureInfo.InvariantCulture, out value)) return (int)value;
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
}