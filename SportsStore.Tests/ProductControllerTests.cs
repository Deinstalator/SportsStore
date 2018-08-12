using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SportsStore.Tests
{
    public class ProductControllerTests
    {
        [Fact]
        public void Can_Paginate()
        {
            //Przygotownaie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 1, Name = "P1"},
            }).AsQueryable<Product>());

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Działanie
            IEnumerable<Product> result =
                controller.List(2).ViewData.Model as IEnumerable<Product>;

            //Asercje
            Product[] prodArray = result.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }
    }
}
