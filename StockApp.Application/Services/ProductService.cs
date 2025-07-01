using AutoMapper;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StockApp.Application.Services
{
    public class ProductService : IProductService
    {
        private IProductRepository _productRepository;
        private IMapper _mapper;
        private readonly ICacheService _cacheService;

        public ProductService(IProductRepository productRepository, IMapper mapper, ICacheService cacheService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task Add(ProductDTO productDto)
        {
            var productEntity = _mapper.Map<Product>(productDto);
            await _productRepository.Create(productEntity);
            await _cacheService.ClearCacheAsync("products_all"); // Clear cache on add
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            var cacheKey = "products_all";
            var cachedProducts = await _cacheService.GetCacheValueAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedProducts))
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(cachedProducts);
            }

            var productsEntity = await _productRepository.GetProducts();
            var productDTOs = _mapper.Map<IEnumerable<ProductDTO>>(productsEntity);

            await _cacheService.SetCacheValueAsync(cacheKey, JsonConvert.SerializeObject(productDTOs), TimeSpan.FromMinutes(5));

            return productDTOs;
        }

        public async Task<ProductDTO> GetByIdAsync(int? id)
        {
            var cacheKey = $"product_{id}";
            var cachedProduct = await _cacheService.GetCacheValueAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedProduct))
            {
                return JsonConvert.DeserializeObject<ProductDTO>(cachedProduct);
            }

            var productEntity = await _productRepository.GetById(id);
            var productDTO = _mapper.Map<ProductDTO>(productEntity);

            if (productDTO != null)
            {
                await _cacheService.SetCacheValueAsync(cacheKey, JsonConvert.SerializeObject(productDTO), TimeSpan.FromMinutes(5));
            }

            return productDTO;
        }

        public async Task Remove(int? id)
        {
            var productEntity = _productRepository.GetById(id).Result;
            if (productEntity != null)
            {
                await _productRepository.Remove(productEntity);
                await _cacheService.ClearCacheAsync($"product_{id}"); // Clear cache for specific product
                await _cacheService.ClearCacheAsync("products_all"); // Clear all products cache
            }
        }

        public async Task Update(ProductDTO productDto)
        {
            var productEntity = _mapper.Map<Product>(productDto);
            await _productRepository.Update(productEntity);
            await _cacheService.ClearCacheAsync($"product_{productDto.Id}"); // Clear cache for specific product
            await _cacheService.ClearCacheAsync("products_all"); // Clear all products cache
        }
    }
}
