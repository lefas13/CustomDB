using CustomMVC.Data;
using CustomMVC.Models;
using CustomMVC.ViewModels.FeeViewModel;
using CustomMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace Tests
{
    public class FeesControllerTests
    {
        [Fact]
        public async Task GetFeesList()
        {
            // Arrange
            var fees = TestDataHelper.GetFakeFeesList();
            var agents = TestDataHelper.GetFakeAgentsList();
            var goods = TestDataHelper.GetFakeGoodsList();
            var warehouses = TestDataHelper.GetFakeWarehousesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Fees).ReturnsDbSet(fees);
            customContextMock.Setup(x => x.Agents).ReturnsDbSet(agents);
            customContextMock.Setup(x => x.Goods).ReturnsDbSet(goods);
            customContextMock.Setup(x => x.Warehouses).ReturnsDbSet(warehouses);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var feesController = new FeesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await feesController.Index(new FilterFeeViewModel(), SortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<FeeViewModel>(viewResult.ViewData.Model);
            Assert.Equal(10, model.Fees.Count());
        }

        [Fact]
        public async Task GetFee()
        {
            var fees = TestDataHelper.GetFakeFeesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Fees).ReturnsDbSet(fees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var feesController = new FeesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await feesController.Details(21);
            var foundResult = await feesController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            var fees = TestDataHelper.GetFakeFeesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Fees).ReturnsDbSet(fees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var feesController = new FeesController(
                customContextMock.Object,
                configurationMock.Object
            );
            feesController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await feesController.Create(fee: null);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            var fees = TestDataHelper.GetFakeFeesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Fees).ReturnsDbSet(fees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var feesController = new FeesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var fee = new Fee
            {
                FeeId = 21,
                AgentId = 1,
                GoodId = 1,
                WarehouseId = 1,
                DocumentNumber = "DC21",
                Amount = 21,
                FeeAmount = 310M,
                ReceiptDate = new DateOnly(1990, 1, 15),
                PaymentDate = new DateOnly(1990, 1, 15),
                ExportDate = new DateOnly(1990, 1, 15),
                Agent = new Agent
                {
                    FullName = "С.С. Сидоров",
                    IdNumber = "ID21"
                },
                Good = new Good
                {
                    Name = "Товар21",
                    GoodTypeId = 1
                },
                Warehouse = new Warehouse
                {
                    WarehouseNumber = "6666"
                }
            };

            // Act
            var result = await feesController.Create(fee);

            // Assert: проверка перенаправления на действие Index
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }

        [Fact]
        public async Task Edit_ReturnsNotFound()
        {
            var fees = TestDataHelper.GetFakeFeesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Fees).ReturnsDbSet(fees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var feesController = new FeesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await feesController.Edit(21);
            var notfoundResult = await feesController.Edit(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<NotFoundResult>(notfoundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            var fees = TestDataHelper.GetFakeFeesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Fees).ReturnsDbSet(fees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var feesController = new FeesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var fee = new Fee
            {
                FeeId = 21,
                AgentId = 1,
                GoodId = 1,
                WarehouseId = 1,
                DocumentNumber = "DC21",
                Amount = 21,
                FeeAmount = 310M,
                ReceiptDate = new DateOnly(1990, 1, 15),
                PaymentDate = new DateOnly(1990, 1, 15),
                ExportDate = new DateOnly(1990, 1, 15),
                Agent = new Agent
                {
                    FullName = "С.С. Сидоров",
                    IdNumber = "ID21"
                },
                Good = new Good
                {
                    Name = "Товар21",
                    GoodTypeId = 1
                },
                Warehouse = new Warehouse
                {
                    WarehouseNumber = "6666"
                }
            };
            var result = await feesController.Edit(1, fee);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            var fees = TestDataHelper.GetFakeFeesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Fees).ReturnsDbSet(fees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var feesController = new FeesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var fee = new Fee
            {
                FeeId = 21,
                AgentId = 1,
                GoodId = 1,
                WarehouseId = 1,
                DocumentNumber = "DC21",
                Amount = 21,
                FeeAmount = 310M,
                ReceiptDate = new DateOnly(1990, 1, 15),
                PaymentDate = new DateOnly(1990, 1, 15),
                ExportDate = new DateOnly(1990, 1, 15),
                Agent = new Agent
                {
                    FullName = "С.С. Сидоров",
                    IdNumber = "ID21"
                },
                Good = new Good
                {
                    Name = "Товар21",
                    GoodTypeId = 1
                },
                Warehouse = new Warehouse
                {
                    WarehouseNumber = "6666"
                }
            };
            var result = await feesController.Edit(21, fee);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            var fees = TestDataHelper.GetFakeFeesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Fees).ReturnsDbSet(fees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var feesController = new FeesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await feesController.Delete(21);
            var foundResult = await feesController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            var fees = TestDataHelper.GetFakeFeesList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Fees).ReturnsDbSet(fees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var feesController = new FeesController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await feesController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }
    }
}