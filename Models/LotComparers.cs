namespace StreamRoulette.Models
{
	internal class LotComparer : IComparer<Lot>
	{
		public int Compare(Lot x, Lot y)
		{
			if (x.AmountValue == null) return 1;
			if (y.AmountValue == null) return -1;

			return (int)y.AmountValue - (int)x.AmountValue;
		}
	}

	internal class ReverseLotComparer : IComparer<Lot>
	{
		public int Compare(Lot x, Lot y)
		{
			// check for empty objects
			if (x == null && y == null) return 0;
			if (x == null) return 1;
			if (y == null) return -1;

			// return 0 if both AmountValues are equal
			if (x.AmountValue == y.AmountValue) return 0;

			// move empty slots lower
			if (x.AmountValue == null) return 1;
			if (y.AmountValue == null) return -1;

			if ((int)x.AmountValue == 0) return 1;
			if ((int)y.AmountValue == 0) return -1;

			// Inverted sorting
			return ((int)x.AmountValue).CompareTo((int)y.AmountValue);
		}
	}
}