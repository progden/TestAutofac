using System;
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
   }
}