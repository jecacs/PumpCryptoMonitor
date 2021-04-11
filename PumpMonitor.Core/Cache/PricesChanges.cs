using System.Collections.Concurrent;
using System.Collections.Generic;
using PumpMonitor.Domains;

namespace PumpMonitor.Core.Cache
{
    public class PricesChanges
    {
        private readonly ConcurrentDictionary<string, IDictionary<TimeFrame, decimal>> _changed = new();
        
        public decimal CalculateMinChanges(string instrument, TimeFrame timeFrame, decimal price, decimal lastPrice)
        {
            if (!_changed.ContainsKey(instrument))
                _changed.TryAdd(instrument, new Dictionary<TimeFrame, decimal>
                {
                    {timeFrame, 0}
                });

            var percents = price.CalcPercents(lastPrice);
            _changed[instrument][timeFrame] = percents;

            return percents;
        }

        public IDictionary<string, decimal> GetChangesDictionary(TimeFrame timeFrame)
        {
            var res = new Dictionary<string, decimal>();

            foreach (var instrument in _changed.Keys)
            {
                res.Add(instrument, _changed[instrument][timeFrame]);
            }

            return res;
        }
        
        public IEnumerable<PriceChangedModel> GetChangesList(TimeFrame timeFrame)
        {
            foreach (var instrument in _changed.Keys)
            {
                yield return new PriceChangedModel
                {
                    Instrument = instrument,
                    Price = _changed[instrument][timeFrame]
                };
            }
        }
    }
}