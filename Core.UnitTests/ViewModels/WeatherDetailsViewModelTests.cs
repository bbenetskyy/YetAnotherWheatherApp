﻿using AutoMapper;
using Core.Constants;
using Core.Services;
using Core.Services.Interfaces;
using Core.UnitTests.TestData;
using Core.ViewModels;
using Moq;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Tests;
using NUnit.Framework;
using OpenWeatherMap;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;

namespace Core.UnitTests.ViewModels
{
    [TestFixture]
    public class WeatherDetailsViewModelTests : MvxIoCSupportingTest
    {
        private Mock<IAlertService> alertMock;
        private Mock<IMvxNavigationService> navigationMock;

        protected override void AdditionalSetup()
        {
            MvxSingletonCache.Instance
                .Settings
                .AlwaysRaiseInpcOnUserInterfaceThread = false;

            var helper = new MvxUnitTestCommandHelper();
            Ioc.RegisterSingleton<IMvxCommandHelper>(helper);

            alertMock = new Mock<IAlertService>();
            alertMock.Setup(a => a.GetWeatherAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((CurrentWeatherResponse)null);
            alertMock.Setup(a => a.IsInternetConnection())
                .Returns(true);
            alertMock.Setup(a => a.GetWeatherAsync(CurrentWeatherTestData.FakeCurrentWeather.City.Name, It.IsAny<string>()))
                .ReturnsAsync(CurrentWeatherTestData.FakeCurrentWeather);
            Ioc.RegisterSingleton<IAlertService>(alertMock.Object);

            navigationMock = new Mock<IMvxNavigationService>();
            navigationMock.Setup(n => n.Navigate<SearchViewModel>(null, default(CancellationToken)))
                .ReturnsAsync(true);
            Ioc.RegisterSingleton<IMvxNavigationService>(navigationMock.Object);

            Ioc.RegisterSingleton<IMapper>(MapService.ConfigureMapper);
        }

        [Test]
        public void ViewModel_Should_Show_WeatherDetails_Correctly()
        {
            //Arrange
            base.Setup();
            var vm = Ioc.IoCConstruct<WeatherDetailsViewModel>();

            //Act
            vm.Prepare(WeatherDetailsTestData.FakeWeatherDetails);

            //Assert
            vm.CityName.ShouldBe(WeatherDetailsTestData.FakeWeatherDetails.CityName);
            vm.CurrentTemperature.ShouldBe(WeatherDetailsTestData.FakeWeatherDetails.CurrentTemperature);
            vm.MaxTemperature.ShouldBe(WeatherDetailsTestData.FakeWeatherDetails.MaxTemperature);
            vm.MinTemperature.ShouldBe(WeatherDetailsTestData.FakeWeatherDetails.MinTemperature);
            vm.Description.ShouldBe(WeatherDetailsTestData.FakeWeatherDetails.Description);
        }

        [Test]
        public async Task RefreshWeatherCommand_Should_Call_Api_With_Same_CityName()
        {
            //Arrange
            base.Setup();
            var vm = Ioc.IoCConstruct<WeatherDetailsViewModel>();
            vm.Prepare(WeatherDetailsTestData.FakeWeatherDetails);

            //Act
            await vm.RefreshWeatherCommand.ExecuteAsync();

            //Assert
            alertMock.Verify(a => a.IsInternetConnection(), Times.Once);
            alertMock.Verify(a => a.GetWeatherAsync(vm.CityName, It.IsAny<string>()), Times.Once);
            navigationMock.Verify(n => n.Navigate<SearchViewModel>(null, default(CancellationToken)),
                Times.Never);
        }

        [Test]
        public async Task RefreshWeatherCommand_Should_Navigate_To_SearchViewModel_If_Api_Throw_An_Error()
        {
            //Arrange
            base.Setup();
            var vm = Ioc.IoCConstruct<WeatherDetailsViewModel>();

            //Act
            await vm.RefreshWeatherCommand.ExecuteAsync();

            //Assert
            alertMock.Verify(a => a.IsInternetConnection(), Times.Once);
            alertMock.Verify(a => a.GetWeatherAsync(vm.CityName, It.IsAny<string>()), Times.Once);
            navigationMock.Verify(n => n.Navigate<SearchViewModel>(null, default(CancellationToken)),
                Times.Once);
        }

        [Test]
        public async Task RefreshWeatherCommand_Should_Navigate_To_SearchViewModel_If_No_Internet()
        {
            //Arrange
            base.Setup();
            alertMock.Setup(a => a.IsInternetConnection())
                .Returns(false);
            var vm = Ioc.IoCConstruct<WeatherDetailsViewModel>();

            //Act
            await vm.RefreshWeatherCommand.ExecuteAsync();

            //Assert
            alertMock.Verify(a => a.IsInternetConnection(), Times.Once);
            alertMock.Verify(a => a.GetWeatherAsync(vm.CityName, It.IsAny<string>()), Times.Never);
            navigationMock.Verify(n => n.Navigate<SearchViewModel>(null, default(CancellationToken)),
                Times.Once);
        }

        [Test]
        public async Task BackCommand_Should_Navigate_To_SearchViewModel()
        {
            //Arrange
            base.Setup();
            var vm = Ioc.IoCConstruct<WeatherDetailsViewModel>();

            //Act
            await vm.BackCommand.ExecuteAsync();

            //Assert
            alertMock.Verify(a => a.IsInternetConnection(), Times.Never);
            alertMock.Verify(a => a.GetWeatherAsync(vm.CityName, It.IsAny<string>()), Times.Never);
            navigationMock.Verify(n => n.Navigate<SearchViewModel>(null, default(CancellationToken)),
                Times.Once);
        }

        [Test]
        public void CurrentTemperatureColor_Should_Return_Chilly_Color()
        {
            //Arrange
            base.Setup();
            var vm = Ioc.IoCConstruct<WeatherDetailsViewModel>();

            //Act
            vm.Prepare(WeatherDetailsTestData.FakeWeatherDetails);

            //Assert
            vm.CurrentTemperatureColor.ShouldBe(Colors.Chilly);
        }

        [Test]
        public void MinTemperatureColor_Should_Return_Chilly_Color()
        {
            //Arrange
            base.Setup();
            var vm = Ioc.IoCConstruct<WeatherDetailsViewModel>();

            //Act
            vm.Prepare(WeatherDetailsTestData.FakeWeatherDetails);

            //Assert
            vm.MinTemperatureColor.ShouldBe(Colors.Chilly);
        }

        [Test]
        public void MaxTemperatureColor_Should_Return_Chilly_Color()
        {
            //Arrange
            base.Setup();
            var vm = Ioc.IoCConstruct<WeatherDetailsViewModel>();

            //Act
            vm.Prepare(WeatherDetailsTestData.FakeWeatherDetails);

            //Assert
            vm.MaxTemperatureColor.ShouldBe(Colors.Warm);
        }
    }
}
