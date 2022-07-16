using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace DynamicDataTest.Web.Data.Trades;

public class Trade : IDisposable, IEquatable<Trade>
{
	readonly ISubject<decimal> _marketPriceChangedSubject = new ReplaySubject<decimal>(1);

	public Trade(Trade trade, TradeStatus status)
	{
		this.Id           = trade.Id;
		this.Customer     = trade.Customer;
		this.CurrencyPair = trade.CurrencyPair;
		this.Status       = status;
		this.MarketPrice  = trade.MarketPrice;
		this.TradePrice   = trade.TradePrice;
		this.Amount       = trade.Amount;
		this.Timestamp    = DateTime.Now;
		this.BuyOrSell    = trade.BuyOrSell;
	}

	public Trade(long        id,
	             string      customer,
	             string      currencyPair,
	             TradeStatus status,
	             BuyOrSell   buyOrSell,
	             decimal     tradePrice,
	             decimal     amount,
	             decimal     marketPrice = 0,
	             DateTime?   timeStamp   = null)
	{
		this.Id           = id;
		this.Customer     = customer;
		this.CurrencyPair = currencyPair;
		this.Status       = status;
		this.MarketPrice  = marketPrice;
		this.TradePrice   = tradePrice;
		this.Amount       = amount;
		this.BuyOrSell    = buyOrSell;
		this.Timestamp    = timeStamp ?? DateTime.Now;
	}

	public decimal   Amount       { get; }
	public BuyOrSell BuyOrSell    { get; }
	public string    CurrencyPair { get; }
	public string    Customer     { get; }

	public long    Id          { get; }
	public decimal MarketPrice { get; private set; }

	public IObservable<decimal> MarketPriceChanged => this._marketPriceChangedSubject.AsObservable();
	public decimal              PercentFromMarket  { get; private set; }
	public TradeStatus          Status             { get; }
	public DateTime             Timestamp          { get; }
	public decimal              TradePrice         { get; }

	public void Dispose() { this._marketPriceChangedSubject.OnCompleted(); }

	public void SetMarketPrice(decimal marketPrice)
	{
		this.MarketPrice       = marketPrice;
		this.PercentFromMarket = Math.Round(((this.TradePrice - this.MarketPrice) / this.MarketPrice) * 100, 4);
		;
		this._marketPriceChangedSubject.OnNext(marketPrice);
	}


#region Equality Members
	public bool Equals(Trade other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;

		return this.Id == other.Id;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;

		return Equals((Trade)obj);
	}

	public override int GetHashCode() => this.Id.GetHashCode();

	public static bool operator ==(Trade left, Trade right) => Equals(left, right);

	public static bool operator !=(Trade left, Trade right) => !Equals(left, right);
#endregion
}


public enum BuyOrSell { Buy, Sell }