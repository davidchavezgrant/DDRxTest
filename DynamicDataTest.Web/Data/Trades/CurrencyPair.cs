namespace DynamicDataTest.Web.Data.Trades;

public class CurrencyPair
{
	public CurrencyPair(string  code,
	                    decimal startingPrice,
	                    int     decimalPlaces,
	                    decimal tickFrequency,
	                    int     defaultSpread = 8)
	{
		this.Code          = code;
		this.InitialPrice  = startingPrice;
		this.DecimalPlaces = decimalPlaces;
		this.TickFrequency = tickFrequency;
		this.DefaultSpread = defaultSpread;
		this.PipSize       = (decimal)Math.Pow(10, -decimalPlaces);
	}

	public string  Code          { get; }
	public int     DecimalPlaces { get; }
	public int     DefaultSpread { get; }
	public decimal InitialPrice  { get; }
	public decimal PipSize       { get; }
	public decimal TickFrequency { get; }

	protected bool Equals(CurrencyPair other) => string.Equals(this.Code, other.Code);

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;

		return Equals((CurrencyPair)obj);
	}

	public override int GetHashCode() => this.Code != null?
		                                     this.Code.GetHashCode() :
		                                     0;

	public override string ToString() => $"Code: {this.Code}, DecimalPlaces: {this.DecimalPlaces}";
}