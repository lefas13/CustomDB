using Microsoft.Extensions.Caching.Memory;
using CustomApplication.Models;

namespace CustomApplication.Service
{
    public class CachedDataService
    {
        private readonly CustomContext _context;
        private readonly IMemoryCache _cache;
        private const int RowCount = 20;

        public CachedDataService(CustomContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public IEnumerable<Agent> GetAgents()
        {
            if (!_cache.TryGetValue("Agents", out IEnumerable<Agent> agents))
            {
                Console.WriteLine("123");
                agents = _context.Agents.Take(RowCount).ToList();
                _cache.Set("Agents", agents, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(244)
                });
            }
            return agents;
        }

        public IEnumerable<Warehouse> GetWarehouses()
        {
            if (!_cache.TryGetValue("Warehouses", out IEnumerable<Warehouse> warehouses))
            {
                warehouses = _context.Warehouses.Take(RowCount).ToList();
                _cache.Set("Warehouses", warehouses, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(244)
                });
            }
            return warehouses;
        }

        public IEnumerable<GoodType> GetGoodTypes()
        {
            if (!_cache.TryGetValue("GoodTypes", out IEnumerable<GoodType> goodTypes))
            {
                goodTypes = _context.GoodTypes.Take(RowCount).ToList();
                _cache.Set("GoodTypes", goodTypes, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(244)
                });
            }
            return goodTypes;
        }


        public IEnumerable<Good> GetGoods()
        {
            if (!_cache.TryGetValue("Goods", out IEnumerable<Good> goods))
            {
                goods = _context.Goods.Take(RowCount).ToList();
                _cache.Set("Goods", goods, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(244)
                });
            }
            return goods;
        }

        public IEnumerable<Fee> GetFees()
        {
            if (!_cache.TryGetValue("Fees", out IEnumerable<Fee> fees))
            {
                fees = _context.Fees.Take(RowCount).ToList();
                _cache.Set("Fees", fees, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(244)
                });
            }
            return fees;
        }
    }

}

