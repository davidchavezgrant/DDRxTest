namespace DynamicDataTest.Web.Data.Trades;

public class Trade

{
	public Trade(long id, string customer, string currencyPair, TradeStatus status)

	{

		this.Id = id;

		this.Customer = customer;

		this.CurrencyPair = currencyPair;

		this.Status = status;

	}
	
	public Trade(Trade trade, TradeStatus status)
	{
		this.Id           = trade.Id;
		this.Customer     = trade.Customer;
		this.CurrencyPair = trade.CurrencyPair;
		this.Status       = status;
	}



	public string CurrencyPair { get; }

	public string Customer { get; }

	public long Id { get; }

	public decimal Price { get; set; }

	public TradeStatus Status { get; }
}