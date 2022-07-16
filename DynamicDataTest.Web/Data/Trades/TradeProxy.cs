using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData.Binding;

namespace DynamicDataTest.Web.Data.Trades;

public class TradeProxy : AbstractNotifyPropertyChanged, IDisposable, IEquatable<TradeProxy>
{
	readonly IDisposable _cleanUp;
	readonly long        _id;
	readonly Trade       _trade;
	decimal              _marketPrice;
	decimal              _pcFromMarketPrice;
	bool                 _recent;

	public TradeProxy(Trade trade)
	{
		this._id    = trade.Id;
		this._trade = trade;

		var isRecent = DateTime.Now.Subtract(trade.Timestamp)
		                       .TotalSeconds
		             < 2;

		var recentIndicator = Disposable.Empty;

		if (isRecent)
		{
			this.Recent = true;

			recentIndicator = Observable.Timer(TimeSpan.FromSeconds(2))
			                            .Subscribe(_ => this.Recent = false);
		}

		//market price changed is an observable on the trade object
		var priceRefresher = trade.MarketPriceChanged
		                          .Subscribe(_ =>
		                          {
			                          this.MarketPrice       = trade.MarketPrice;
			                          this.PercentFromMarket = trade.PercentFromMarket;
		                          });

		this._cleanUp = Disposable.Create(() =>
		{
			recentIndicator.Dispose();
			priceRefresher.Dispose();
		});
	}

	public decimal Amount => this._trade.Amount;

	public string CurrencyPair => this._trade.CurrencyPair;

	public string Customer => this._trade.Customer;

	public long Id => this._trade.Id;

	public decimal MarketPrice
	{
		get => this._marketPrice;
		set => SetAndRaise(ref this._marketPrice, value);
	}

	public decimal PercentFromMarket
	{
		get => this._pcFromMarketPrice;
		set => SetAndRaise(ref this._pcFromMarketPrice, value);
	}

	public bool Recent
	{
		get => this._recent;
		set => SetAndRaise(ref this._recent, value);
	}

	public TradeStatus Status => this._trade.Status;

	public DateTime Timestamp => this._trade.Timestamp;

	public decimal TradePrice => this._trade.TradePrice;

	public void Dispose() { this._cleanUp.Dispose(); }

	public bool Equals(TradeProxy other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;

		return this._id == other._id;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;

		return Equals((TradeProxy)obj);
	}

	public override int GetHashCode() => this._id.GetHashCode();

	public static bool operator ==(TradeProxy left, TradeProxy right) => Equals(left, right);

	public static bool operator !=(TradeProxy left, TradeProxy right) => !Equals(left, right);
}