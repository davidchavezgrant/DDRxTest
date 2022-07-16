namespace DynamicDataTest.Web.Data.Trades;

public class MarketData : IEquatable<MarketData>
{
	public MarketData(string instrument, decimal bid, decimal offer)
	{
		this.Instrument = instrument;
		this.Bid        = bid;
		this.Offer      = offer;
	}

	public decimal Bid { get; }

	public string  Instrument { get; }
	public decimal Offer      { get; }

	public bool Equals(MarketData other) => string.Equals(this.Instrument, other.Instrument) && (this.Bid == other.Bid) && (this.Offer == other.Offer);

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;

		return obj is MarketData && Equals((MarketData)obj);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			var hashCode = this.Instrument != null?
				               this.Instrument.GetHashCode() :
				               0;

			hashCode = (hashCode * 397) ^ this.Bid.GetHashCode();
			hashCode = (hashCode * 397) ^ this.Offer.GetHashCode();
			return hashCode;
		}
	}

	public static MarketData operator +(MarketData left, decimal pipsValue)
	{
		var bid   = left.Bid   + pipsValue;
		var offer = left.Offer + pipsValue;
		return new MarketData(left.Instrument, bid, offer);
	}

	public static bool operator ==(MarketData left, MarketData right) => left.Equals(right);

	public static bool operator >(MarketData left, MarketData right) => left.Bid > right.Bid;

	public static bool operator >=(MarketData left, MarketData right) => left.Bid >= right.Bid;

	public static bool operator !=(MarketData left, MarketData right) => !left.Equals(right);

	public static bool operator <(MarketData left, MarketData right) => left.Bid < right.Bid;

	public static bool operator <=(MarketData left, MarketData right) => left.Bid <= right.Bid;

	public static MarketData operator -(MarketData left, decimal pipsValue)
	{
		var bid   = left.Bid   - pipsValue;
		var offer = left.Offer - pipsValue;
		return new MarketData(left.Instrument, bid, offer);
	}

	public override string ToString() => $"{this.Instrument}, {this.Bid}/{this.Offer}";
}