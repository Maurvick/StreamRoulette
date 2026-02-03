using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace StreamRoulette.Models
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Auction : INotifyPropertyChanged
	{
		[JsonProperty("Items")]
		private readonly ObservableCollection<ILot> _Items;
		private readonly ReadOnlyObservableCollection<ILot> _ReadOnlyItems;
		private readonly ObservableCollection<ILot> _SortedItems;
		private readonly ReadOnlyObservableCollection<ILot> _ReadOnlySortedItems;
		private int _Bank;
		private AuctionMode _Mode;

		[JsonProperty]
		public string CurrentLanguage { get; set; } = "en"; // Default app language

		public event PropertyChangedEventHandler PropertyChanged;

		public Auction()
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

			// Sort list
			var sortedList = _Items.Where(x => x != null)
				.OrderBy(x => x as Lot, comparer).ToList();

			// Synchronize _SortedItems collection
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