using CustomMVC.Data;
using CustomMVC.Models;
using CustomMVC.ViewModels.GoodTypeViewModel;
using CustomMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;

namespace Tests
{
    public class GoodTypesControllerTests
    {
        [Fact]
        public async Task GetGoodTypesList()
        {
            // Arrange
            var goodTypes = TestDataHelper.GetFakeGoodTypesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.GoodTypes).ReturnsDbSet(goodTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodTypesController = new GoodTypesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await goodTypesController.Index(new FilterGoodTypeViewModel(), SortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<GoodTypeViewModel>(viewResult.ViewData.Model);
            Assert.Equal(3, model.GoodTypes.Count());
        }

        [Fact]
        public async Task GetGoodType()
        {
            var goodTypes = TestDataHelper.GetFakeGoodTypesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.GoodTypes).ReturnsDbSet(goodTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodTypesController = new GoodTypesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await goodTypesController.Details(4);
            var foundResult = await goodTypesController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            var goodTypes = TestDataHelper.GetFakeGoodTypesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.GoodTypes).ReturnsDbSet(goodTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodTypesController = new GoodTypesController(
                customContextMock.Object,
                configurationMock.Object
            );
            goodTypesController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await goodTypesController.Create(goodType: null);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            var goodTypes = TestDataHelper.GetFakeGoodTypesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.GoodTypes).ReturnsDbSet(goodTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodTypesController = new GoodTypesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var goodType = new GoodType
            {
                GoodTypeId = 4,
                Name = "Транспорт",
                Measurement = "шт.",
                AmountOfFee = 120M
            };

            // Act
            var result = await goodTypesController.Create(goodType);

            // Assert: проверка перенаправления на действие Index
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }

        [Fact]
        public async Task Edit_ReturnsNotFound()
        {
            var goodTypes = TestDataHelper.GetFakeGoodTypesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.GoodTypes).ReturnsDbSet(goodTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodTypesController = new GoodTypesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await goodTypesController.Edit(4);
            var notfoundResult = await goodTypesController.Edit(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<NotFoundResult>(notfoundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            var goodTypes = TestDataHelper.GetFakeGoodTypesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.GoodTypes).ReturnsDbSet(goodTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodTypesController = new GoodTypesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var goodType = new GoodType
            {
                GoodTypeId = 4,
                Name = "Транспорт",
                Measurement = "шт.",
                AmountOfFee = 120M
            };
            var result = await goodTypesController.Edit(1, goodType);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            var goodTypes = TestDataHelper.GetFakeGoodTypesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.GoodTypes).ReturnsDbSet(goodTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodTypesController = new GoodTypesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var goodType = new GoodType
            {
                GoodTypeId = 4,
                Name = "Транспорт",
                Measurement = "шт.",
                AmountOfFee = 120M
            };
            var result = await goodTypesController.Edit(4, goodType);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            var goodTypes = TestDataHelper.GetFakeGoodTypesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.GoodTypes).ReturnsDbSet(goodTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodTypesController = new GoodTypesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await goodTypesController.Delete(4);
            var foundResult = await goodTypesController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            var goodTypes = TestDataHelper.GetFakeGoodTypesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.GoodTypes).ReturnsDbSet(goodTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var goodTypesController = new GoodTypesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await goodTypesController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }
    }
}