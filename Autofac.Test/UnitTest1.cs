using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Autofac.Test
{
    public class Tests
    {
        ServiceCollection services = new ServiceCollection();

        [SetUp]
        public void Setup()
        {
            services.AddLogging();
        }
        private IServiceProvider GetProvider(Action<ContainerBuilder> buildBy)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);
            if(buildBy != null)
                buildBy(builder);
            
            var container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        [Test]
        public void TestResolve()
        {
            var provider = GetProvider(b =>
            {
                b.RegisterType<TestService>().AsSelf().As<ITestService>();
            });
            using (var scope = provider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<ITestService>();
                var p = service.GetProduct();
                Assert.NotNull(p);
                Assert.NotNull(p.Name);
                Assert.NotNull(p.Price);
            }
            Assert.Pass();
        }

        [Test]
        public void TestPropertyInjection()
        {
            var provider = GetProvider(b =>
            {
                b.RegisterType<ProductRepository>().As<IProductRepository>();
                b.RegisterType<TestService>().PropertiesAutowired().AsSelf().As<ITestService>();
            });
            using (var scope = provider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<ITestService>();
                Assert.NotNull(service.ProductRepo);
                var products = service.ProductRepo.GetProducts();
                Assert.NotNull(products);
                Assert.AreEqual(100, products.ToList().Count);
                var p = service.GetProduct();
                Assert.NotNull(p);
                Assert.NotNull(p.Name);
                Assert.NotNull(p.Price);
            }
            Assert.Pass();
        }

        [Test]
        public void TestMethodInjection()
        {
            var provider = GetProvider(b =>
            {
                b.RegisterType<ProductRepository>().As<IProductRepository>();
                b.RegisterType<TestService>().PropertiesAutowired()
                    .OnActivated(e =>
                    {
                        e.Instance.CallMessage();
                    })
                    .AsSelf().As<ITestService>();
            });
            using (var scope = provider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<ITestService>();
                Assert.NotNull(service.ProductRepo);
                var products = service.ProductRepo.GetProducts();
                Assert.NotNull(products);
                Assert.AreEqual(100, products.ToList().Count);
                var p = service.GetProduct();
                Assert.NotNull(p);
                Assert.NotNull(p.Name);
                Assert.NotNull(p.Price);
            }
            Assert.Pass();

        }


        public interface ITestService
        {
            public IProductRepository ProductRepo { set; get; }

            public Product GetProduct();

            public void CallMessage();

        }


        public class Product
        {
            public string Name { set; get; }
            public int Price { set; get; }
        }

        public class TestService:ITestService
        {
            public TestService()
            {
                Console.WriteLine("Write in constructor");
            }

            public IProductRepository ProductRepo { get; set; }

            public Product GetProduct()
            {
                return new Bogus.Faker<Product>()
                    .RuleFor(t => t.Name, f => f.Commerce.ProductName())
                    .RuleFor(t => t.Price, f => 500);

            }

            public void CallMessage()
            {
                Console.WriteLine("Write in method call");
            }
        }
    }

    public interface IProductRepository
    {
        IEnumerable<ProductEntity> GetProducts(Func<bool> pred = null);
    }
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

    public class ProductEntity
    {
        public string Name { set; get; }
        public string Price { set; get; }
    }
}