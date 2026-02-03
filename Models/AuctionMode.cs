using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamRoulette.Models
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum AuctionMode
	{
		Normal,
		Reversed,
		Roulette
	}
}