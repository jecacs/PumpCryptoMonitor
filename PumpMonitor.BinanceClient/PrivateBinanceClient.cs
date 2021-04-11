using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net.Enums;
using Binance.Net.Objects.Spot;
using Binance.Net.Objects.Spot.SpotData;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.ExchangeInterfaces;
using CryptoExchange.Net.Objects;
using Serilog;

namespace PumpMonitor.BinanceClient
{
    public class PrivateBinanceClient
    {
        private readonly Binance.Net.BinanceClient _binanceClient;
        private readonly ILogger _logger;
        
        private readonly bool _isTest;

        public PrivateBinanceClient(string apiKey, string secretKey, ILogger logger, bool isTest = false)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey));
            
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException(nameof(secretKey));
            
            _binanceClient = new Binance.Net.BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials(apiKey, secretKey)
            });

            _logger = logger;
            
            _isTest = isTest;
        }

        private delegate Task<WebCallResult<BinancePlacedOrder>> PlaceOrder(
            string symbol,
            OrderSide side,
            OrderType type,
            decimal? quantity = null,
            decimal? quoteOrderQuantity = null,
            string? newClientOrderId = null,
            decimal? price = null,
            TimeInForce? timeInForce = null,
            decimal? stopPrice = null,
            decimal? icebergQty = null,
            OrderResponseType? orderResponseType = null,
            int? receiveWindow = null,
            CancellationToken ct = default
            );

        private PlaceOrder ApiRequest => _isTest 
            ? _binanceClient.Spot.Order.PlaceTestOrderAsync
            : _binanceClient.Spot.Order.PlaceOrderAsync;

        public async Task<BinancePlacedOrder?> OpenMarketPositionAsync(string instrument, decimal quantity, CancellationToken? cancellationToken)
        {
            try
            {
                var apiResult = await ApiRequest(
                    symbol: instrument,
                    side: OrderSide.Buy,
                    type: OrderType.Market,
                    quantity: quantity,
                    orderResponseType: OrderResponseType.Full,
                    ct: cancellationToken ?? new CancellationToken()
                );

                if (apiResult.ResponseStatusCode == HttpStatusCode.OK)
                {
                    _logger.Information(
                        "Market position by {instrument} is done, by price: {price}, quantity: {quantity}", 
                        instrument, apiResult.Data.Price, apiResult.Data.Quantity
                        );

                    return apiResult.Data;
                }
                
                _logger.Error(
                    "Position not open by {instrument}, reason: {reason}", 
                    instrument, 
                    apiResult.Error?.Message
                    );

                return null;
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);

                return null;
            }
        }

        public async Task<bool> CancelStopLossAsync(string instrument, long orderId, CancellationToken cancellationToken)
        {
            try
            {
                var apiResult = await _binanceClient.Spot.Order
                    .CancelOrderAsync(instrument, orderId, ct: cancellationToken);

                if (apiResult.ResponseStatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                
                _logger.Error(
                    "StopLoss cancel created for a instrument: {instrument}, reason: {reason}", 
                    instrument, 
                    apiResult.Error?.Message
                );
                
                return false;
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);

                return false;
            }
        }
        
        public async Task<BinancePlacedOrder?> OpenStopLossAsync(string instrument, decimal quantity, decimal stopPrice, decimal price, CancellationToken? cancellationToken)
        {
            try
            {
                var apiResult = await ApiRequest(
                    symbol: instrument,
                    side: OrderSide.Sell, 
                    type: OrderType.StopLossLimit,
                    timeInForce: TimeInForce.GoodTillCancel,
                    quantity: quantity,
                    stopPrice: stopPrice,
                    price: price,
                    ct: cancellationToken ?? new CancellationToken()
                );

                if (apiResult.ResponseStatusCode == HttpStatusCode.OK)
                {
                    _logger.Information(
                        "StopLoss {instrument} is done, by price: {price}, quantity: {quantity}", 
                        instrument, price, quantity
                    );

                    return apiResult.Data;
                }

                _logger.Error(
                    "StopLoss not open {instrument}, reason: {reason}, code: {code}, price: {price}, stopPrice: {price}", 
                    instrument, 
                    apiResult.Error?.Message,
                    apiResult.Error?.Code,
                    price,
                    price
                );

                return null;
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);

                return null;
            }
        }

        public async Task<BinancePlacedOrder?> CloseMarketPositionAsync(string instrument, decimal quantity, CancellationToken? cancellationToken)
        {
            try
            {
                var apiResult = await ApiRequest(
                    symbol: instrument,
                    side: OrderSide.Sell, 
                    type: OrderType.Market,
                    quantity: quantity,
                    ct: cancellationToken ?? new CancellationToken()
                    );
                
                if (apiResult.ResponseStatusCode == HttpStatusCode.OK)
                {
                    _logger.Information(
                        "Position close {instrument} is done, by price: {price}, quantity: {quantity}", 
                        apiResult.Data.Symbol, apiResult.Data.Price, apiResult.Data.Quantity
                    );

                    return apiResult.Data;
                }

                _logger.Error(
                    "Position close in not closed {instrument}, reason: {reason}", 
                    instrument, 
                    apiResult.Error?.Message
                );

                return null;
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);

                return null;
            }
        }

        public async Task<decimal> GetBalanceByInstrumentAsync(string instrument)
        {
            try
            {
                var apiResult = await ((IExchangeClient) _binanceClient).GetBalancesAsync();

                if (apiResult.ResponseStatusCode == HttpStatusCode.OK)
                {
                    var response = apiResult.Data.FirstOrDefault(x => x.CommonAsset == instrument);

                    if (response != null)
                        return response.CommonAvailable;

                    return 0;
                }

                _logger.Error(
                    "Cant get balance by instrument: {instrument}, reason: {reason}", 
                    instrument, 
                    apiResult.Error?.Message
                );

                return 0;
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);

                return 0;
            }
        }

        public async Task<BinanceOrder?> GetActiveOrderAsync(string instrument, long orderId, CancellationToken? cancellationToken)
        {
            try
            {
                var apiResult = await _binanceClient.Spot.Order
                    .GetOrderAsync(instrument, orderId, ct: cancellationToken ?? new CancellationToken());
                
                if (apiResult.ResponseStatusCode == HttpStatusCode.OK)
                    return apiResult.Data;
                
                return null;
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);

                return null;
            }
        }
    }
}