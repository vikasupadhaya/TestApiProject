using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountOwnerServer.Controllers;
using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace api_test
{
    public class OwnerControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockRepo;
        private readonly OwnerController _controller;
        private readonly Mock<ILoggerManager> _mockLogger;
        private readonly Mock<IMapper> _mockmapper;
        public OwnerControllerTests()
        {
            _mockRepo = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILoggerManager>();
            _mockmapper = new Mock<IMapper>();
            _controller = new OwnerController(_mockLogger.Object, _mockRepo.Object, _mockmapper.Object);
        }

        [Fact]
        public async void ReturnsExactNumberOfOwners()
        {

            //Arrange
            _mockRepo.Setup(repo => repo.Owner.GetAllOwnersAsync())
                            .ReturnsAsync(GETAllOwners());

            //Act
            var result = await _controller.GetAllOwners() as OkObjectResult;

            //Assert
            var model = Assert.IsAssignableFrom<List<Owner>>(
               result.Value);
            Assert.Equal(3, model.Count);
        }

        [Fact]
        public async void Given_OwnerId_ReturnsExactOwner()
        {

            //Arrange
            _mockRepo.Setup(repo => repo.Owner.GetOwnerByIdAsync(1)).ReturnsAsync(GETAllOwners().Find(x => x.Id == 1));
            var expectedResult = GETAllOwners().Find(x => x.Id == 1);

            //Act
            var result = await _controller.GetOwnerById(1) as OkObjectResult;
            var model = Assert.IsAssignableFrom<Owner>(
                        result.Value);

            //Assert
            Assert.Equal(expectedResult.Id, model.Id);
            Assert.Equal(expectedResult.Name, model.Name);
            Assert.Equal(expectedResult.DateOfBirth, model.DateOfBirth);
        }

        [Fact]
        public async void Given_OwnerId_ReturnsOwnerWithAccountDetails()
        {

            //Arrange
            _mockRepo.Setup(repo => repo.Owner.GetOwnerWithDetailsAsync(3)).ReturnsAsync(GETAllOwners().Find(x => x.Id == 3));
            var expectedResult = GETAllOwners().Find(x => x.Id == 3);

            //Act
            var result = await _controller.GetOwnerWithDetails(3) as OkObjectResult;
            var model = Assert.IsAssignableFrom<Owner>(
                        result.Value);

            //Assert
            Assert.Equal(expectedResult.Id, model.Id);
            Assert.Equal(expectedResult.Name, model.Name);
            Assert.Equal(expectedResult.DateOfBirth, model.DateOfBirth);
            Assert.Equal(expectedResult.Accounts.Count, model.Accounts.Count);
            var expectedAccountItem = expectedResult.Accounts.Find(x => x.Id == 1);
            var actualAccountItem = model.Accounts.Find(x => x.Id == 1);
            Assert.Equal(expectedAccountItem.OwnerId, actualAccountItem.OwnerId);
            Assert.Equal(expectedAccountItem.DateCreated, actualAccountItem.DateCreated);
            Assert.Equal(expectedAccountItem.AccountType, actualAccountItem.AccountType);
        }

        [Fact]
        public async Task GetOwners_ShouldBeOfTypeList()
        {
            //Arrange
            _mockRepo.Setup(repo => repo.Owner.GetAllOwnersAsync())
                            .ReturnsAsync(GETAllOwners());

            //Act
            var result = await _controller.GetAllOwners() as OkObjectResult;

            //Assert
            var model = Assert.IsAssignableFrom<List<Owner>>(
              result.Value);

            Assert.IsAssignableFrom<List<Owner>>(model);
        }

        [Fact]
        public async Task Given_OwnerId_ExistingIdPassed_RemovesOneItem()
        {
            // Arrange
            var id = 1;
            var OwnerDto = new Owner()
            {
                Id = 1,
                Name = "Vikas Owner1",
                DateOfBirth = new DateTime(2021, 11, 28, 02, 19, 18),
                Address = "Asdress",
                Accounts = null
            };

            _mockRepo.Setup(repo => repo.Owner.GetOwnerByIdAsync(id)).ReturnsAsync(OwnerDto);
            _mockRepo.Setup(repo => repo.Owner.Delete(It.IsAny<Owner>()))
                    .Verifiable();

            // Act
            await _controller.DeleteOwner(id);

            // Assert
            _mockRepo.Verify(repo => repo.Owner.DeleteOwner(It.IsAny<Owner>()), Times.Once);
        }


        private List<Owner> GETAllOwners()
        {
            var ownersList = new List<Owner>();
            var OwnerDto = new Owner()
            {
                Id = 1,
                Name = "Vikas Owner1",
                DateOfBirth = new DateTime(2021, 11, 28, 02, 19, 18),
                Address = "Asdress",
                Accounts = null
            };
            ownersList.Add(OwnerDto);
            var OwnerDto1 = new Owner()
            {
                Id = 2,
                Name = "Vikas Owner2",
                DateOfBirth = new DateTime(2021, 11, 28, 02, 19, 18),
                Address = "Asdress",
                Accounts = null
            };
            ownersList.Add(OwnerDto);

            var Account = new Account()
            {
                Id = 1,
                OwnerId = 3,
                AccountType = "Domestic",
                DateCreated = new DateTime(2021, 11, 28, 02, 19, 18),
            };
            var AccountsList = new List<Account>();
            AccountsList.Add(Account);

            var OwnerDto2 = new Owner()
            {
                Id = 3,
                Name = "Vikas Owner3",
                DateOfBirth = new DateTime(2021, 11, 28, 02, 19, 18),
                Address = "Asdress",
                Accounts = AccountsList
            };

            ownersList.Add(OwnerDto2);
            return ownersList;
        }
    }
}
