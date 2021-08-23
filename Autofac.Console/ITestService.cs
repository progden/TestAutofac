namespace Autofac.Test
{
    public interface ITestService
    {
        public IProductRepository ProductRepo { set; get; }

        public Product GetProduct();

        public void CallMessage();

    }
}