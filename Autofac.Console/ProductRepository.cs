using System;
using System.Collections.Generic;
using System.Linq;

namespace Autofac.Test
{
    public class ProductRepository: IProductRepository
    {
        public IEnumerable<ProductEntity> GetProducts(Func<bool> pred = null)
        {
            return Enumerable.Range(1, 100).Select(t => new Bogus.DataSets.Commerce(locale: "zh_TW"))
                .Select(p => new ProductEntity()
                {
                    Name = p.ProductName(),
                    Price = p.Price()
                });

        }
    }
}