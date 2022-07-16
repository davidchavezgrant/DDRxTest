namespace DynamicDataTest.Web.Data.Trades;

sealed class TradeGenerator
{
	readonly Random     _random = new();
	readonly StaticData _staticData;
	int                 _counter;
	readonly object     _locker = new();
	public TradeGenerator(StaticData staticData) => this._staticData = staticData;

	public IEnumerable<Trade> Generate(int numberToGenerate, bool initialLoad = false)
	{
		Trade NewTrade()
		{
			var id     = this._counter++;
			var bank   = this._staticData.Customers[this._random.Next(0,     this._staticData.Customers.Length)];
			var pair   = this._staticData.CurrencyPairs[this._random.Next(0, this._staticData.CurrencyPairs.Length)];
			var amount = (this._random.Next(1,                               2000) / 2) * (10 ^ this._random.Next(1, 5));


			if (initialLoad)
			{
				var status = this._random.NextDouble() > 0.5?
					             TradeStatus.Live :
					             TradeStatus.Closed;

				var seconds = this._random.Next(1, 60 * 60 * 24);
				var time    = DateTime.Now.AddSeconds(-seconds);

				return new Trade(id, bank, pair.Code, status);
			}

			return new Trade(id,
			                 bank,
			                 pair.Code,
			                 TradeStatus.Live);
		}

		
		IEnumerable<Trade> result;
		lock (_locker)
		{
			result = Enumerable.Range(1, numberToGenerate).Select(_ => NewTrade()).ToArray();
		}
		return result;

	}
}