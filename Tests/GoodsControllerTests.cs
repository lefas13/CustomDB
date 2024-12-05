using CustomMVC.Data;
using CustomMVC.Models;
using CustomMVC.ViewModels.GoodViewModel;
using CustomMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace Tests
{
    public class GoodsControllerTests
    {
        [Fact]
        public async Task GetGoodsList()
        {
            // Arrange
            var goods = TestDataHelper.GetFakeGoodsList();
            var goodTypes = TestDataHelper.GetFakeGoodTypesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Goods).ReturnsDbSet(goods);
            customContextMock.Setup(x => x.GoodTypes).ReturnsDbSet(goodTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodsController = new GoodsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await goodsController.Index(new FilterGoodViewModel(), SortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<GoodViewModel>(viewResult.ViewData.Model);
            Assert.Equal(10, model.Goods.Count());
        }

        [Fact]
        public async Task GetGood()
        {
            var goods = TestDataHelper.GetFakeGoodsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Goods).ReturnsDbSet(goods);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodsController = new GoodsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await goodsController.Details(21);
            var foundResult = await goodsController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            var goods = TestDataHelper.GetFakeGoodsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Goods).ReturnsDbSet(goods);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodsController = new GoodsController(
                customContextMock.Object,
                configurationMock.Object
            );
            goodsController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await goodsController.Create(good: null);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            var goods = TestDataHelper.GetFakeGoodsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Goods).ReturnsDbSet(goods);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodsController = new GoodsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var good = new Good
            {
                GoodId = 11,
                GoodTypeId = 1,
                Name = "Товар11",
                GoodType = new GoodType
                {
                    Name = "Транспорт",
                    Measurement = "шт.",
                    AmountOfFee = 120M
                }
            };

            // Act
            var result = await goodsController.Create(good);

            // Assert: проверка перенаправления на действие Index
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }

        [Fact]
        public async Task Edit_ReturnsNotFound()
        {
            var goods = TestDataHelper.GetFakeGoodsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Goods).ReturnsDbSet(goods);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodsController = new GoodsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await goodsController.Edit(11);
            var notfoundResult = await goodsController.Edit(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<NotFoundResult>(notfoundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            var goods = TestDataHelper.GetFakeGoodsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Goods).ReturnsDbSet(goods);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodsController = new GoodsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var good = new Good
            {
                GoodId = 11,
                GoodTypeId = 1,
                Name = "Товар11",
                GoodType = new GoodType
                {
                    Name = "Транспорт",
                    Measurement = "шт.",
                    AmountOfFee = 120M
                }
            };
            var result = await goodsController.Edit(1, good);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            var goods = TestDataHelper.GetFakeGoodsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Goods).ReturnsDbSet(goods);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodsController = new GoodsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var good = new Good
            {
                GoodId = 11,
                GoodTypeId = 1,
                Name = "Товар11",
                GoodType = new GoodType
                {
                    Name = "Транспорт",
                    Measurement = "шт.",
                    AmountOfFee = 120M
                }
            };
            var result = await goodsController.Edit(11, good);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            var goods = TestDataHelper.GetFakeGoodsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Goods).ReturnsDbSet(goods);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodsController = new GoodsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await goodsController.Delete(11);
            var foundResult = await goodsController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            var goods = TestDataHelper.GetFakeGoodsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Goods).ReturnsDbSet(goods);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodsController = new GoodsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await goodsController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }
    }
}
