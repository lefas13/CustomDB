using CustomMVC.Data;
using CustomMVC.Models;
using CustomMVC.ViewModels.WarehouseViewModel;
using CustomMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;

namespace Tests
{
    public class WarehousesControllerTests
    {
        [Fact]
        public async Task GetWarehousesList()
        {
            // Arrange
            var warehouses = TestDataHelper.GetFakeWarehousesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Warehouses).ReturnsDbSet(warehouses);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var warehousesController = new WarehousesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await warehousesController.Index(new FilterWarehouseViewModel(), SortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<WarehouseViewModel>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Warehouses.Count());
        }

        [Fact]
        public async Task GetWarehouse()
        {
            var warehouses = TestDataHelper.GetFakeWarehousesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Warehouses).ReturnsDbSet(warehouses);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var warehousesController = new WarehousesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await warehousesController.Details(4);
            var foundResult = await warehousesController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            var warehouses = TestDataHelper.GetFakeWarehousesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Warehouses).ReturnsDbSet(warehouses);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var warehousesController = new WarehousesController(
                customContextMock.Object,
                configurationMock.Object
            );
            warehousesController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await warehousesController.Create(warehouse: null);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            var warehouses = TestDataHelper.GetFakeWarehousesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Warehouses).ReturnsDbSet(warehouses);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var warehousesController = new WarehousesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var warehouse = new Warehouse
            {
                WarehouseId = 4,
                WarehouseNumber = "4444"
            };

            // Act
            var result = await warehousesController.Create(warehouse);

            // Assert: проверка перенаправления на действие Index
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }

        [Fact]
        public async Task Edit_ReturnsNotFound()
        {
            var warehouses = TestDataHelper.GetFakeWarehousesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Warehouses).ReturnsDbSet(warehouses);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var warehousesController = new WarehousesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await warehousesController.Edit(4);
            var notfoundResult = await warehousesController.Edit(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<NotFoundResult>(notfoundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            var warehouses = TestDataHelper.GetFakeWarehousesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Warehouses).ReturnsDbSet(warehouses);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var warehousesController = new WarehousesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var warehouse = new Warehouse
            {
                WarehouseId = 4,
                WarehouseNumber = "4444"
            };
            var result = await warehousesController.Edit(1, warehouse);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            var warehouses = TestDataHelper.GetFakeWarehousesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Warehouses).ReturnsDbSet(warehouses);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var warehousesController = new WarehousesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var warehouse = new Warehouse
            {
                WarehouseId = 4,
                WarehouseNumber = "4444"
            };
            var result = await warehousesController.Edit(4, warehouse);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            var warehouses = TestDataHelper.GetFakeWarehousesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Warehouses).ReturnsDbSet(warehouses);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var warehousesController = new WarehousesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await warehousesController.Delete(4);
            var foundResult = await warehousesController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            var warehouses = TestDataHelper.GetFakeWarehousesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Warehouses).ReturnsDbSet(warehouses);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var warehousesController = new WarehousesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await warehousesController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }
    }
}