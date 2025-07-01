namespace StockApp.Application.Configurations
{
    public class StockReplenishmentSettings
    {
        public int LowStockThreshold { get; set; }
        public int ReplenishQuantity { get; set; }
        public int IntervalMinutes { get; set; }
    }
}