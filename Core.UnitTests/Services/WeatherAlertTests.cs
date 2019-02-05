﻿using API;
using Core.Services;
using Core.UnitTests.TestData;
using InteractiveAlert;
using Moq;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Tests;
using NUnit.Framework;
using Plugin.Connectivity.Abstractions;
using Shouldly;
using System;
using System.Threading.Tasks;

namespace Core.UnitTests.Services
{
    [TestFixture]
    public class WeatherAlertTests : MvxIoCSupportingTest
    {
        private Mock<IApiClient> apiMock;
        private Mock<IInteractiveAlerts> interactiveMock;
        private Mock<IConnectivity> connectivityMock;

        protected override void AdditionalSetup()
        {
            MvxSingletonCache.Instance
                .Settings
                .AlwaysRaiseInpcOnUserInterfaceThread = false;

            var helper = new MvxUnitTestCommandHelper();
            Ioc.RegisterSingleton<IMvxCommandHelper>(helper);

            apiMock = new Mock<IApiClient>();
            apiMock.Setup(a => a.GetWeatherByCityNameAsync(It.IsAny<string>()))
                .Throws<AggregateException>();
            apiMock.Setup(a => a.GetWeatherByCityNameAsync(null))
                .Throws<ArgumentException>();
            apiMock.Setup(a => a.GetWeatherByCityNameAsync(CurrentWeatherTestData.FakeCurrentWeather.City.Name))
                .ReturnsAsync(CurrentWeatherTestData.FakeCurrentWeather);
            Ioc.RegisterSingleton<IApiClient>(apiMock.Object);

            connectivityMock = new Mock<IConnectivity>();
            connectivityMock.Setup(c => c.IsConnected)
                .Returns(true);
            Ioc.RegisterSingleton<IConnectivity>(connectivityMock.Object);

            interactiveMock = new Mock<IInteractiveAlerts>();
            Ioc.RegisterSingleton<IInteractiveAlerts>(interactiveMock.Object);
        }

        [Test]
        public async Task GetWeather_Should_Call_Api_And_Return_Weather()
        {
            //Arrange 
            base.Setup();
            var alertService = new WeatherAlertService();
            var cityName = CurrentWeatherTestData.FakeCurrentWeather.City.Name;

            //Act
            var currentWeather = await alertService.GetWeatherAsync(cityName, null);

            //Assert
            currentWeather.ShouldNotBeNull();
            currentWeather.City.Name.ShouldBe(CurrentWeatherTestData.FakeCurrentWeather.City.Name);
            apiMock.Verify(a => a.GetWeatherByCityNameAsync(cityName), Times.Once);
            interactiveMock.Verify(i => i.ShowAlert(It.IsAny<InteractiveAlertConfig>()), Times.Never);
        }

        [TestCase("London2")]
        [TestCase("asdasd")]
        public async Task GetWeather_Should_Call_Api_And_Show_Error_Alert(string cityName)
        {
            //Arrange 
            base.Setup();
            var alertService = new WeatherAlertService();

            //Act
            var currentWeather = await alertService.GetWeatherAsync(cityName, null);

            //Assert
            currentWeather.ShouldBeNull();
            apiMock.Verify(a => a.GetWeatherByCityNameAsync(cityName), Times.Once);
            interactiveMock.Verify(i => i.ShowAlert(It.IsAny<InteractiveAlertConfig>()), Times.Once);
        }

        [Test]
        public void IsInternetConnection_Should_Call_Return_True_If_Interntet_Available()
        {
            //Arrange 
            base.Setup();
            var alertService = new WeatherAlertService();

            //Act
            var isInternet = alertService.IsInternetConnection();

            //Assert
            isInternet.ShouldBeTrue();
            connectivityMock.Verify(c => c.IsConnected, Times.Once);
            apiMock.Verify(a => a.GetWeatherByCityNameAsync(It.IsAny<string>()), Times.Never);
            interactiveMock.Verify(i => i.ShowAlert(It.IsAny<InteractiveAlertConfig>()), Times.Never);
        }

        [Test]
        public void IsInternetConnection_Should_Call_Return_False_And_Warning_Alert_If_No_Interntet()
        {
            //Arrange 
            base.Setup();
            connectivityMock.Setup(c => c.IsConnected)
                .Returns(false);
            var alertService = new WeatherAlertService();

            //Act
            var isInternet = alertService.IsInternetConnection();

            //Assert
            isInternet.ShouldBeFalse();
            connectivityMock.Verify(c => c.IsConnected, Times.Once);
            apiMock.Verify(a => a.GetWeatherByCityNameAsync(It.IsAny<string>()), Times.Never);
            interactiveMock.Verify(i => i.ShowAlert(It.IsAny<InteractiveAlertConfig>()), Times.Once);
        }
    }
}