using CustomMVC.Data;
using CustomMVC.Models;
using CustomMVC.ViewModels.AgentViewModel;
using CustomMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;

namespace Tests
{
    public class AgentsControllerTests
    {
        [Fact]
        public async Task GetAgentsList()
        {
            // Arrange
            var agents = TestDataHelper.GetFakeAgentsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Agents).ReturnsDbSet(agents);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var agentsController = new AgentsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = agentsController.Index(new FilterAgentViewModel(), SortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<AgentViewModel>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Agents.Count());
        }

        [Fact]
        public async Task GetAgent()
        {
            var agents = TestDataHelper.GetFakeAgentsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Agents).ReturnsDbSet(agents);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var agentsController = new AgentsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await agentsController.Details(4);
            var foundResult = await agentsController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            var agents = TestDataHelper.GetFakeAgentsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Agents).ReturnsDbSet(agents);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var agentsController = new AgentsController(
                customContextMock.Object,
                configurationMock.Object
            );
            agentsController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await agentsController.Create(agent: null);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            var agents = TestDataHelper.GetFakeAgentsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Agents).ReturnsDbSet(agents);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var agentsController = new AgentsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var agent = new Agent
            {
                AgentId = 4,
                FullName = "С.С. Сидоров",
                IdNumber = "ID4",
            };

            // Act
            var result = await agentsController.Create(agent);

            // Assert: проверка перенаправления на действие Index
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }

        [Fact]
        public async Task Edit_ReturnsNotFound()
        {
            var agents = TestDataHelper.GetFakeAgentsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Agents).ReturnsDbSet(agents);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var agentsController = new AgentsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await agentsController.Edit(4);
            var notfoundResult = await agentsController.Edit(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<NotFoundResult>(notfoundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            var agents = TestDataHelper.GetFakeAgentsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Agents).ReturnsDbSet(agents);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var agentsController = new AgentsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var agent = new Agent
            {
                AgentId = 4,
                FullName = "С.С. Сидоров",
                IdNumber = "ID4",
            };
            var result = await agentsController.Edit(1, agent);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            var agents = TestDataHelper.GetFakeAgentsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Agents).ReturnsDbSet(agents);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var agentsController = new AgentsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var agent = new Agent
            {
                AgentId = 4,
                FullName = "С.С. Сидоров",
                IdNumber = "ID4",
            };
            var result = await agentsController.Edit(4, agent);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
           customContextMock.Verify();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            var agents = TestDataHelper.GetFakeAgentsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Agents).ReturnsDbSet(agents);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var agentsController = new AgentsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await agentsController.Delete(4);
            var foundResult = await agentsController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            var agents = TestDataHelper.GetFakeAgentsList();
            var customContextMock = new Mock<CustomContext>();
            customContextMock.Setup(x => x.Agents).ReturnsDbSet(agents);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация AdditionalServicesController
            var agentsController = new AgentsController(
                customContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await agentsController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            customContextMock.Verify();
        }
    }
}