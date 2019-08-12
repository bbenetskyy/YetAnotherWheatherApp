using Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using System;
using Core.Resources;
using UIKit;

namespace iOS.Views
{
    [MvxFromStoryboard]
    [MvxModalPresentation]
    public partial class WeatherDetailsView : MvxViewController<WeatherDetailsViewModel>
    {
        public WeatherDetailsView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<WeatherDetailsView, WeatherDetailsViewModel>();

            set.Bind(cityNameLabel).To(vm => vm.CityName);
            set.Bind(descriptionLabel).To(vm => vm.Description);
            set.Bind(currentTemperatureLabel).To(vm => vm.CurrentTemperature);
            set.Bind(minTemperatureLabel).To(vm => vm.MinTemperature);
            set.Bind(maxTemperatureLabel).To(vm => vm.MaxTemperature);
            set.Bind(refreshButton).To(vm => vm.RefreshWeatherCommand);
            set.Bind(backButton).To(vm => vm.BackCommand);

            set.Bind(loadingIndicator)
                .For("Visibility")
                .To(vm => vm.IsLoading)
                .WithConversion("Visibility");
            set.Bind(refreshButton)
                .For("Visibility")
                .To(vm => vm.IsLoading)
                .WithConversion("InvertedVisibility");

            set.Bind(currentTemperatureLabel)
                .For("TextColor")
                .To(vm => vm.CurrentTemperature)
                .WithConversion("TemperatureToColor");
            set.Bind(minTemperatureLabel)
                .For("TextColor")
                .To(vm => vm.MinTemperature)
                .WithConversion("TemperatureToColor");
            set.Bind(maxTemperatureLabel)
                .For("TextColor")
                .To(vm => vm.MaxTemperature)
                .WithConversion("TemperatureToColor");

            refreshButton.Layer.BorderColor = UIColor.White.CGColor;

            descriptionHint.Text = ViewModel[nameof(AppResources.DescriptionLabel)];
            currentTemperatureHint.Text = ViewModel[nameof(AppResources.TemperatureLabel)];
            minTemperatureHint.Text = ViewModel[nameof(AppResources.MinLabel)];
            maxTemperatureLabel.Text = ViewModel[nameof(AppResources.MaxLabel)];
            refreshButton.SetTitle(ViewModel[nameof(AppResources.SearchButton)], UIControlState.Normal);

            set.Apply();
        }
    }
}