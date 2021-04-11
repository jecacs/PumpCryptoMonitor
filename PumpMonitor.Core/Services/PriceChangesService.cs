using System.Linq;
using PumpMonitor.Core.Cache;
using PumpMonitor.Domains;

namespace PumpMonitor.Core.Services
{
    public class PriceChangesService
    {
        private readonly PricesChanges _pricesChanges;

        public PriceChangesService(PricesChanges pricesChanges)
        {
            _pricesChanges = pricesChanges;
        }

        public IOrderedEnumerable<PriceChangedModel> GetSpot(TimeFrame timeFrame)
        {
            var prices = _pricesChanges
                .GetChangesList(timeFrame)
                .OrderByDescending(x => x.Price);

            return prices;
        }
    }
}