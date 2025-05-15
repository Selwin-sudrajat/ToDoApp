using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;
using ToDoApp.Controllers;
using ToDoApp.Models;   

namespace ToDoAppTest
{
    [TestFixture]
    public class Tests
    {
        private Mock<DbSet<ToDo>> _mockSet;
        private Mock<ToDoDbContext> _mockContext;
        private Mock<ILogger<HomeController>> _mockLogger;
        private List<ToDo> _data;
        [SetUp]
        public void Setup()
        {
            _data = new List<ToDo>
            {
                new ToDo { Id = 1, Description = "Task 1", Check = true },
                new ToDo { Id = 2, Description = "Task 2", Check = false },
                new ToDo { Id = 3, Description = "Task 3", Check = true }
            };
            var queryable = _data.AsQueryable();

            _mockSet = new Mock<DbSet<ToDo>>();
            _mockSet.As<IQueryable<ToDo>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockSet.As<IQueryable<ToDo>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockSet.As<IQueryable<ToDo>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockSet.As<IQueryable<ToDo>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            _mockContext = new Mock<ToDoDbContext>(new DbContextOptions<ToDoDbContext>());
            _mockContext.Setup(c => c.ToDo).Returns(_mockSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            _mockLogger = new Mock<ILogger<HomeController>>();
        }

        [Test]
        public void DeleteToDo_RemovesItemAndRedirects()
        {
            // Arrange
            var controller = new HomeController(_mockLogger.Object, _mockContext.Object );

            // Act
            var result = controller.DeleteToDo(1) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));

            _mockSet.Verify(m => m.Remove(It.Is<ToDo>(t => t.Id == 1)), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);

        }

        [Test]
        public void DeleteToDo_Exception()
        {
            _mockContext.Setup(c => c.ToDo).Throws(new Exception("DB error"));
            var controller = new HomeController(_mockLogger.Object, _mockContext.Object);

            // Act
            var result = controller.DeleteToDo(1) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));

            _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Error deleting todo")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once );
        }

        [Test]
        public void CheckToDo_UpdatesItemAndRedirects()
        {
            // Arrange
            var controller = new HomeController(_mockLogger.Object, _mockContext.Object);

            // Act
            var result = controller.CheckToDo(2) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_data.First().Check, Is.True);

            _mockSet.Verify(m => m.Update(It.Is<ToDo>(t => t.Id == 2)), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);

        }

        [Test]
        public void CheckToDo_Exception()
        {
            // Arrange
            _mockContext.Setup(c => c.ToDo).Throws(new Exception("DB error"));
            var controller = new HomeController(_mockLogger.Object, _mockContext.Object);

            // Act
            var result = controller.CheckToDo(2) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_data.First().Check, Is.True);

            _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Error updating todo")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once);
        }

        [Test]
        public void CreateEditToDo_Edit()
        {
            var controller = new HomeController(_mockLogger.Object, _mockContext.Object);

            var result = controller.CreateEditToDo(2) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.TypeOf<ToDo>());
            Assert.That(((ToDo)result.Model).Id, Is.EqualTo(2));

        }

        [Test]
        public void CreateEditToDo_Create()
        {
            var controller = new HomeController(_mockLogger.Object, _mockContext.Object);

            var result = controller.CreateEditToDo(null) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.Null);
        }

        [Test]
        public void CreateEditForm_Create()
        {
            var dataTest = new List<ToDo>
            {
                new ToDo { Id = 0, Description = "TestAdd", Check = false },

            };

            var controller = new HomeController(_mockLogger.Object, _mockContext.Object);

            var result = controller.CreateEditForm(dataTest.FirstOrDefault()) as ViewResult;

            _mockSet.Verify(m => m.Add(It.Is<ToDo>(t => t.Id == 0)), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void CreateEditForm_Create_Exception()
        {
            _mockContext.Setup(c => c.ToDo).Throws(new Exception("DB error"));
            var dataTest = new List<ToDo>
            {
                new ToDo { Id = 0, Description = "", Check = false },

            };

            var controller = new HomeController(_mockLogger.Object, _mockContext.Object);

            var result = controller.CreateEditForm(dataTest.FirstOrDefault()) as ViewResult;

            _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Error adding todo")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once);
        }

        [Test]
        public void CreateEditForm_Edit()
        {
            var controller = new HomeController(_mockLogger.Object, _mockContext.Object);

            var result = controller.CreateEditForm(_data.Where(x => x.Id == 3).FirstOrDefault()) as ViewResult;

            _mockSet.Verify(m => m.Update(It.Is<ToDo>(t => t.Id == 3)), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void CreateEditForm_Edit_Exception()
        {
            _mockContext.Setup(c => c.ToDo).Throws(new Exception("DB error"));
            var controller = new HomeController(_mockLogger.Object, _mockContext.Object);

            var result = controller.CreateEditForm(_data.Where(x => x.Id == 3).FirstOrDefault()) as ViewResult;

            _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Error editing todo")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once);
        }
    }
}