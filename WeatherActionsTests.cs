using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestsForBlazorWithRedux.utils.WeatherUtils;
using BlazorWithRedux.Store.Weather.State;
using BlazorWithRedux.Store.Weather.Actions;
using Fluxor;
using Fluxor.Undo;
using FluentAssertions;

namespace UnitTestsForBlazorWithRedux
{
    public class WeatherActionsTests(WeatherTestsFixture fixture) : IClassFixture<WeatherTestsFixture>
    {
        private readonly WeatherTestsFixture _fixture = fixture;

        [Fact]
        public void TestWeatherInitialization()
        {

            var state = _fixture.state;

            state.Should().BeEquivalentTo(
               new UndoableWeatherState
               {
                   Present = new WeatherState()
                   {
                       Initialized = false,
                       Loading = false,
                       Forecasts = Array.Empty<WeatherForecast>()
                   }
               });
        }
    }
}
