using System;
using System.Collections.Generic;

namespace Autofac.Test
{
    public interface IProductRepository
    {
        IEnumerable<ProductEntity> GetProducts(Func<bool> pred = null);
    }
}