namespace PumpMonitor.Domains
{
    public class PriceChangedModel
    {
        public string Instrument { get; set; }
        
        public decimal Price { get; set; }
    }
}