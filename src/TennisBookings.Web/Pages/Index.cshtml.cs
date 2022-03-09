﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TennisBookings.Web.Services;

namespace TennisBookings.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IGreetingService _greetingService;
        private readonly IConfiguration _config;
        private readonly IWeatherForecaster _weatherForecaster;

        public IndexModel(IGreetingService greetingService, IConfiguration config, IWeatherForecaster weatherForecaster)
        {
            _greetingService = greetingService;
            _config = config;
            _weatherForecaster = weatherForecaster;
        }

        public string Greeting { get; private set; }
        public bool ShowGreeting => !string.IsNullOrEmpty(Greeting);
        public string ForecastSectionTitle { get; private set; }
        public string WeatherDescription { get; private set; }
        public bool ShowWeatherForecast { get; private set; }

        public async Task OnGet()
        {
            var features = new Features();
            _config.Bind("Features:HomePage", features);

            if (features.EnableGreeting)
            {
                Greeting = _greetingService.GetRandomGreeting();
            }

            ShowWeatherForecast = features.EnableWeatherForecast
                && _weatherForecaster.ForecastEnabled;

            if (!ShowWeatherForecast)
                return;

            var title = features.ForecastSectionTitle;
            ForecastSectionTitle = string.IsNullOrEmpty(title) ? "How is the weather?" : title;

            var currentWeather = await _weatherForecaster.GetCurrentWeatherAsync();

            if (currentWeather == null)
                return;

            switch (currentWeather.Description)
            {
                case "Sun":
                    WeatherDescription = "It's sunnaeaeeeyyyy!";
                    break;

                case "Rain":
                    WeatherDescription = "It's raining... No tennis today :(";
                    break;

                case "Cloud":
                    WeatherDescription = "Cloudy, no worries";
                    break;

                default:
                    WeatherDescription = "Weatherforecast is not determined yet";
                    break;
            }
        }

        private class Features
        {
            public bool EnableGreeting { get; set; }
            public bool EnableWeatherForecast { get; set; }
            public string ForecastSectionTitle { get; set; }
        }
    }
}
