using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Binance.Net.Interfaces;

namespace PumpMonitor.Core.Cache
{
    public class BlackListInstruments
    {
        private readonly ConcurrentDictionary<string, string> _instruments = new();
        
        private readonly string[] _blackListPatterns;

        private readonly decimal _tradingVolumeToStartTrade;

        private readonly string _currency;

        public BlackListInstruments(string[] blackListPatterns, double tradingVolumeToStartTrade, string currency)
        {
            _blackListPatterns = blackListPatterns;
            _tradingVolumeToStartTrade = (decimal) tradingVolumeToStartTrade;
            _currency = currency;
        }

        public void Initial(IReadOnlyList<IBinanceTick> items)
        {
            foreach (var item in items)
            {
                if (_instruments.ContainsKey(item.Symbol))
                    return;
                
                if (Array.Exists(_blackListPatterns, blackListElement => item.Symbol.Contains(blackListElement)))
                {
                    _instruments.TryAdd(item.Symbol, item.Symbol);
                    
                    continue;
                }

                if (item.QuoteVolume >= _tradingVolumeToStartTrade && item.Symbol.Contains(_currency))
                    continue;

                _instruments.TryAdd(item.Symbol, item.Symbol);
            }
        }

        public bool IsExist(string instrument)
        {
            return _instruments.ContainsKey(instrument);
        }
    }
}