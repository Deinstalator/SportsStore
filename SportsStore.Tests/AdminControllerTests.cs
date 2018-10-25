using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using Xunit;

namespace SportsStore.Tests
{
    public class AdminControllerTests
    {
        [Fact]
        public void Index_Contains_All_Products()
        {
            //Przygotownie - tworznie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2"},
                new Product { ProductID = 3, Name = "P3"},
            }.AsQueryable<Product>());

            //Przygotowanie - utworznie kontrolera
            AdminController tartget = new AdminController(mock.Object);

            //Działanie
            Product[] result =
                GetViewModel<IEnumerable<Product>>(tartget.Index())?.ToArray();

            //Asercje
            Assert.Equal(3, result.Length);
            Assert.Equal("P1", result[0].Name);
            Assert.Equal("P2", result[1].Name);
            Assert.Equal("P3", result[2].Name);
        }

        [Fact]
        public void Cat_Edit_Product()
        {
            //Przygotownie - tworznie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2"},
                new Product { ProductID = 3, Name = "P3"},
            }.AsQueryable<Product>());

            //Przygotowanie - utworznie kontrolera
            AdminController tartget = new AdminController(mock.Object);

            //Działanie
            Product p1 = GetViewModel<Product>(tartget.Edit(1));
            Product p2 = GetViewModel<Product>(tartget.Edit(2));
            Product p3 = GetViewModel<Product>(tartget.Edit(3));

            //Asercje
            Assert.Equal(1, p1.ProductID);
            Assert.Equal(2, p2.ProductID);
            Assert.Equal(3, p3.ProductID);
        }

        [Fact]
        public void Cannot_Edit_Nonexistent_Product()
        {
            //Przygotownie - tworznie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2"},
                new Product { ProductID = 3, Name = "P3"},
            }.AsQueryable<Product>());

            //Przygotowanie - utworznie kontrolera
            AdminController tartget = new AdminController(mock.Object);

            //Działanie
            Product result = GetViewModel<Product>(tartget.Edit(4));

            //Asercje
            Assert.Null(result);
        }

        [Fact]
        public void Can_Save_Valid_Changes()
        {
            //Przygotownie - tworznie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            //Przygotowanie - towrzenie imitacji kontenera TempData
            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

            //Przygotowanie - towrzenie komtrolera
            AdminController target = new AdminController(mock.Object)
            {
                TempData = tempData.Object
            };
            //Przygotownaie = tworzenie produktu
            Product product = new Product { Name = "Test" };

            //Działanie - próba zapisania produktu
            IActionResult result = target.Edit(product);

            //Asercje - sparwdzenie czy zostało wywołane repozytorium
            mock.Verify(m => m.SaveProduct(product));
            //Asercje - sparwdzenie typu zwracanego z metody
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", (result as RedirectToActionResult).ActionName);
        }

        [Fact]
        public void Cannot_Save_Invalid_Changes()
        {
            //Przygotowanie - tworzenie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            //Przygotowanie - towrzenie komtrolera
            AdminController target = new AdminController(mock.Object);

            //Przygotowanie - tworznie produktu
            Product product = new Product { Name = "Test" };

            //Przygotownaie - dodanie błędu do stanu modelu
            target.ModelState.AddModelError("error", "error");

            //Działanie - próba zapisania produktu
            IActionResult result = target.Edit(product);

            //Asercje - sprawdzenie, czy nie zostało wywołane repozytorium
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            //Asercje - sprawdzenie typu zwracanego z metody
            Assert.IsType<ViewResult>(result);
        }

        private T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as ViewResult)?.ViewData.Model as T;
        }
    }
}
