using System;
using Binance.Net.Objects.Spot.SpotData;

namespace PumpMonitor.Core
{
    public static class TradingUtils
    {
        public static decimal CalcPercents(this decimal x, decimal y)
        {
            var value = (x - y) / x * 100;
            return -Math.Round(value, 2);
        }

        public static decimal CalcQuantity(this decimal amount, decimal price, decimal percentsOfAmount)
        {
            var percents = amount / 100 * percentsOfAmount;
            return Math.Round(percents / price, 2);
        }

        public static bool IsClosePositionByProfit(this decimal openPrice, decimal currentPrice, decimal percentsOfProfit)
        {
            var percents = CalcPercents(openPrice, currentPrice);

            if (percents >= percentsOfProfit)
                return true;

            return false;
        }

        /// <summary>
        ///  Calc stop loss price from open price. In current moment stopPrice and price is the same
        /// </summary>
        /// <param name="openPrice"></param>
        /// <param name="percentsOfPrice"></param>
        /// <returns></returns>
        public static (decimal stopPrice, decimal price) CalcStopLossPrices(this decimal openPrice, decimal percentsOfPrice)
        {
            var price = openPrice / 100 * percentsOfPrice;

            var result = Math.Round(openPrice - price, 3);
            
            return (result, result);
        }

        public static bool StopLossOrderIsDone(this BinanceOrder order)
        {
            switch (order.Status)
            {
                case Binance.Net.Enums.OrderStatus.New:
                    return false;
                
                case Binance.Net.Enums.OrderStatus.PartiallyFilled:
                    return false;
                
                case Binance.Net.Enums.OrderStatus.Canceled:
                    return true;
                
                case Binance.Net.Enums.OrderStatus.Rejected:
                    return true;
                
                case Binance.Net.Enums.OrderStatus.Filled:
                    return true;
                
                case Binance.Net.Enums.OrderStatus.PendingCancel:
                    return true;
                
                case Binance.Net.Enums.OrderStatus.Expired:
                    return true;

                default:
                    return true;
            }
        }
    }
}