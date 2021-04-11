namespace PumpMonitor.Blazor
{
    public class AppSettings
    {
        public const string KeyName = "Settings";
        
        public string PricesCacheCleanTs { get; set; }
        
        public string PricesCacheWorkerTs { get; set; }
        
        public string BasicCurrency { get; set; }

        public Bot Bot { get; set; }
        
        public string[] CurrencyBlackList { get; set; }
        
        public Binance Binance { get; set; }
    }

    public class Binance
    {
        public string ApiKey { get; set; }
        
        public string ApiSecret { get; set; }
    }

    public class Bot
    {
        public bool IsEnabled { get; set; }
        
        public decimal MinBalance { get; set; }

        public double PercentsOfAmount { get; set; }
        
        public double PercentsToStartTrade { get; set; }
        
        public double PercentsToClosePosition { get; set; }
        
        public int MaxActiveInstruments { get; set; }
        
        public string DelayedStartTs { get; set; }
        
        public double TradingVolumeToStartTrade { get; set; }
        
        public double StopLimitPercentsOfPrice { get; set; }
    }
}