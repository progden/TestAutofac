using System;

namespace Autofac.Test
{
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