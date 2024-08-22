using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using DemoWebApplication.Constants;
using DemoWebApplication.Controllers;
using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceImplementation;
using DemoWebApplication.Service.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebTestAppication.ShopApp.Tests.Controller
{
    public class LoginControllerTests
    {

        private Mock<IUserInterface> _userInterfaceMock;
        private Mock<IAuthServiceInterface> _iAuthServiceInterfaceMock;
        private Mock<IMapper> _iMapperMock;
        private LoginController _loginController;


        public LoginControllerTests()
        {
            _userInterfaceMock = new Mock<IUserInterface>();
            _iAuthServiceInterfaceMock = new Mock<IAuthServiceInterface>();
            _iMapperMock = new Mock<IMapper>();

            _loginController = new LoginController(_userInterfaceMock.Object, _iAuthServiceInterfaceMock.Object,_iMapperMock.Object);
        }

        [Fact]
        public async Task LoginController_Index_Returns_UserCredentialsError_WhenInvalisCredentials()
        {
            var loginDetails = new DemoWebApplication.Models.Login
            {
                Username = "alex123",
                Password = "Alex_1234567"
            };

            _iAuthServiceInterfaceMock.Setup(asi => asi.CheckUserCredentials(loginDetails)).ReturnsAsync(false);

            var result = await _loginController.Index(loginDetails) as ObjectResult;

            Assert.NotNull(result);

            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal( Message.InvalidCredentialsMessage,result.Value);
        }



        [Fact]
        public async Task LoginController_Index_Returns_GetUserByUsernameError_WhenUserNotFound()
        {
            var loginDetails = new DemoWebApplication.Models.Login
            {
                Username = "alex123",
                Password = "Alex_1234567"
            };

            _iAuthServiceInterfaceMock.Setup(asi => asi.CheckUserCredentials(loginDetails)).ReturnsAsync(true);

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(loginDetails.Username)).Returns(null as PersonDto);
            var result = await _loginController.Index(loginDetails) as ObjectResult;

            Assert.NotNull(result);

            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(Message.UserNotFound, result.Value);
        }
        [Fact]
        public async Task LoginController_Index_ReturnsJson_WhenLoginSuccess()
        {
            var loginDetails = new DemoWebApplication.Models.Login
            {
                Username = "alex123",
                Password = "Alex_1234567"
            };
            var personDto = new PersonDto
            {
                Id = "12345",
                CustomerName = "John Doe",
                Address = "123 Elm Street, Springfield, IL, 62704",
                Username = "johndoe",
                Password = "P@ssw0rd123",
                Balance = 1500.75,
                FilePath = "/images/profile/johndoe.jpg"
            };


            var person = new Person
            {
                Id = "12345",
                CustomerName = "John Doe",
                Address = "123 Elm Street, Springfield, IL, 62704",
                UserName = "johndoe",
                Password = "P@ssw0rd123",
                Balance = 1500.75,
                FilePath = "/images/profile/johndoe.jpg"
            };

            string token = "asjvneinwqn1423nvndsd";
            _iAuthServiceInterfaceMock.Setup(asi => asi.CheckUserCredentials(loginDetails)).ReturnsAsync(true);

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(loginDetails.Username)).Returns(personDto);

            _iMapperMock.Setup(mapper => mapper.Map<Person>(personDto)).Returns(person);
            _iAuthServiceInterfaceMock.Setup(asi => asi.GenerateTokenString(person)).ReturnsAsync(token);

            _userInterfaceMock.Setup(ui => ui.getRoleofUser(personDto.Id)).ReturnsAsync("admin");
            var result = await _loginController.Index(loginDetails) as JsonResult;
            Assert.NotNull(result);

            dynamic response = result?.Value;

            //Assert.Equal("johndoe", (string)response.username);
            //Assert.Equal(token, (string)response.token);
            //Assert.Equal("admin", (string)response.role);
        }

        [Fact]
        public async Task LoginController_Index_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            
            var loginDetails = new DemoWebApplication.Models.Login
            {
                Username = "alex123",
                Password = "Alex_1234567"
            };

            _iAuthServiceInterfaceMock.Setup(asi => asi.CheckUserCredentials(loginDetails)).ReturnsAsync(true);

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(loginDetails.Username))
                              .Throws(new Exception("Simulated exception"));

            var result = await _loginController.Index(loginDetails) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.Equal(Message.InternalServerError, result.Value);
        }

    }
}
