using lab6.Controllers;
using lab6.Data;
using lab6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;

namespace TestCustom.Tests
{
    public class AgentsControllerTests
    {
        private readonly AgentsApiController _controller;
        private readonly Mock<DbSet<Agent>> _mockSet;
        private readonly Mock<CustomContext> _mockContext;

        public AgentsControllerTests()
        {
            // Создание DbSet
            _mockSet = new Mock<DbSet<Agent>>();

            _mockContext = new Mock<CustomContext>();
            _mockContext.Setup(m => m.Agents).Returns(_mockSet.Object);

            _controller = new AgentsApiController(_mockContext.Object);
        }


        [Fact]
        public async Task GetAgent_ReturnsNotFound_WhenAgentDoesNotExist()
        {
            // Arrange
            var agentId = 999; 
            _mockContext.Setup(m => m.Agents.FindAsync(agentId)).ReturnsAsync((Agent)null);

            // Act
            var result = await _controller.GetAgent(agentId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostAgent_CreatesAgent()
        {
            // Arrange
            var newAgent = new Agent { FullName = "Alice Green", IdNumber = "Id1" };
            _mockContext.Setup(m => m.Agents.AddAsync(newAgent, default)).ReturnsAsync((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Agent>)null);

            // Act
            var result = await _controller.PostAgent(newAgent);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetAgent", createdAtActionResult.ActionName);
            Assert.Equal(newAgent, createdAtActionResult.Value);
        }

        [Fact]
        public async Task DeleteAgent_ReturnsNoContent_WhenAgentIsDeleted()
        {
            // Arrange
            var agentId = 1;
            var agent = new Agent { AgentId = agentId, FullName = "John Doe", IdNumber = "Idf" };
            _mockContext.Setup(m => m.Agents.FindAsync(agentId)).ReturnsAsync(agent);

            // Act
            var result = await _controller.DeleteAgent(agentId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }


        [Fact]
        public void Fee_Validation_ShouldFail_WhenRequiredFieldsAreNull()
        {
            // Arrange
            var fee = new Fee
            {
                ReceiptDate = null,
                DocumentNumber = null,
                PaymentDate = null,
                ExportDate = null
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(fee, new ValidationContext(fee), validationResults, true);

            // Assert
            Assert.False(isValid); 
            Assert.Equal(4, validationResults.Count);
        }

        [Fact]
        public void Fee_Validation_ShouldPass_WhenAllFieldsAreValid()
        {
            // Arrange
            var fee = new Fee
            {
                WarehouseId = 1,
                GoodId = 1,
                AgentId = 1,
                ReceiptDate = new DateOnly(2001,1,1),
                Amount = 1,
                DocumentNumber = "D1",
                FeeAmount = 1,
                PaymentDate = new DateOnly(2001, 1, 1),
                ExportDate = new DateOnly(2001, 1, 1)
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(fee, new ValidationContext(fee), validationResults, true);

            // Assert
            Assert.True(isValid); 
            Assert.Empty(validationResults); 
        }

        [Fact]
        public void Fee_ShouldHaveValidForeignKeyRelationships()
        {
            // Arrange
            var fee = new Fee
            {
                WarehouseId = 1,
                GoodId = 2,
                AgentId = 3,
            };

            Assert.Equal(1, fee.WarehouseId);
            Assert.Equal(2, fee.GoodId);
            Assert.Equal(3, fee.AgentId);
        }


        [Fact]
        public void Warehouse_Validation_ShouldFail_WhenRequiredFieldsAreNull()
        {
            // Arrange
            var warehouse = new Warehouse
            {
                WarehouseNumber = null
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(warehouse, new ValidationContext(warehouse), validationResults, true);

            // Assert
            Assert.False(isValid); 
            Assert.Single(validationResults); 
        }

        [Fact]
        public void Warehouse_Validation_ShouldPass_WhenAllFieldsAreValid()
        {
            // Arrange
            var warehouse = new Warehouse
            {
                WarehouseNumber = "Num12"
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(warehouse, new ValidationContext(warehouse), validationResults, true);

            // Assert
            Assert.True(isValid); 
            Assert.Empty(validationResults); 
        }
    }
}