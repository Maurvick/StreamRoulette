using System.Windows.Media;

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
}