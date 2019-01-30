﻿using AutoMapper;
using Core.Models;
using OpenWeatherMap;

namespace Core.Services
{
    public static class MapService
    {
        public static IMapper ConfigureMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CurrentWeatherResponse, WeatherDetails>()
                    .ForMember(dest => dest.CityName,
                        m => m.MapFrom(src => src.City.Name))
                    .ForMember(dest => dest.Description,
                        m => m.MapFrom(src => src.Weather.Value))
                    .ForMember(dest => dest.CurrentTemperature,
                        m => m.MapFrom(src => $"{src.Temperature.Value} {src.Temperature.Unit}"))
                    .ForMember(dest => dest.MinTemperature,
                        m => m.MapFrom(src => $"{src.Temperature.Min} {src.Temperature.Unit}"))
                    .ForMember(dest => dest.MaxTemperature,
                        m => m.MapFrom(src => $"{src.Temperature.Max} {src.Temperature.Unit}"));
            });
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        }
    }
}