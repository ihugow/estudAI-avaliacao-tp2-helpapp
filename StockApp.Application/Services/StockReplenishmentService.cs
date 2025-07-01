using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using StockApp.Application.Interfaces;
using StockApp.Application.Configurations;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace StockApp.Application.Services
{
    public class StockReplenishmentService : BackgroundService
    {
        private readonly ILogger<StockReplenishmentService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly StockReplenishmentSettings _settings;

        public StockReplenishmentService(
            ILogger<StockReplenishmentService> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<StockReplenishmentSettings> settings)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stock Replenishment Service running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking for low stock products...");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                    var products = await productService.GetProductsAsync();
                    var lowStockProducts = products.Where(p => p.Stock <= _settings.LowStockThreshold).ToList();

                    foreach (var product in lowStockProducts)
                    {
                        product.Stock += _settings.ReplenishQuantity;
                        await productService.Update(product);
                        _logger.LogInformation($"Product {product.Name} replenished. New stock: {product.Stock}");
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(_settings.IntervalMinutes), stoppingToken);
            }

            _logger.LogInformation("Stock Replenishment Service stopped.");
        }
    }
}