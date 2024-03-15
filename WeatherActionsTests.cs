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
using static System.Net.WebRequestMethods;
using System.Runtime.InteropServices;
using System.IO;

namespace UnitTestsForBlazorWithRedux
{
    public class WeatherActionsTests(WeatherTestsFixture fixture) : IClassFixture<WeatherTestsFixture>
    {
        private readonly WeatherTestsFixture _fixture = fixture;

        [Fact]
        public void TestWeatherBeforeInitialization()
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

        [Fact]
        public void TestWeatherInitializationDone()
        {
            // after the data are loaded, the state should be updated

            var state = _fixture.state;

            // to simulate the load effect we will call the reducer after the retrieving of the forecats is done
            var action = new WeatherSetLoadingAction(true);
            state = state.WithWeather(action);

            state.Should().BeEquivalentTo(
               new UndoableWeatherState
               {
                   // in the past should be the loading false
                   Past = new[]
                   {
                          new WeatherState()
                          {
                            Initialized = false,
                            Loading = false,
                            Forecasts = Array.Empty<WeatherForecast>()
                          }
                   },
                   Present = new WeatherState()
                   {
                       Initialized = false,
                       Loading = true,
                       Forecasts = Array.Empty<WeatherForecast>()
                   }
               });

            var action2 = new WeatherLoadForecastsAction();
            state = state.WithWeather(action2);

            // so the forecasts should be not empty
            state.Present.Forecasts.Should().NotBeEmpty(); //!!!
            var forecasts = state.Present.Forecasts;

            var action3 = new WeatherSetLoadingAction(false);
            state = state.WithWeather(action3);

            var action4 = new WeatherSetInitializedAction();
            state = state.WithWeather(action4);

            // the following method to check if the state is updated is the simplest one
            // we can also check the state of the forecasts, but this is enough for now.
            // This, since the forecasts randomly changes.
            state.Should().NotBeEquivalentTo(
               new UndoableWeatherState
               {
                   Present = new WeatherState()
                   {
                       Initialized = false,
                       Loading = false,
                       Forecasts = Array.Empty<WeatherForecast>()
                   }
               });
            // then check if the state is not loading anymore since it is initialized
            state.Present.Initialized.Should().BeTrue();
            state.Present.Loading.Should().BeFalse();


            // now we can go back undoing actions
            var action5 = new UndoAction<UndoableWeatherState>();
            state = state.WithWeather(action5);

            state.Should().BeEquivalentTo(
               new UndoableWeatherState
               {
                   Past = new[]
                   {
                          new WeatherState()
                          {
                            Initialized = false,
                            Loading = false,
                            Forecasts = Array.Empty<WeatherForecast>()
                          },
                          new WeatherState()
                          {
                                Initialized = false,
                                Loading = true,
                                Forecasts = Array.Empty<WeatherForecast>()
                          },
                          new WeatherState()
                          {
                               Initialized = false,
                               Loading = true,
                               Forecasts = forecasts.ToArray()
                          },
                   },
                   Present = new WeatherState()
                   {
                       Initialized = false,
                       Loading = false,
                       Forecasts = forecasts.ToArray()
                   },
                   Future = new[]
                   {
                        new WeatherState()
                        {
                           Initialized = true,
                           Loading = false,
                           Forecasts = forecasts.ToArray()
                        }
                   }
               });

            var action6 = new RedoAction<UndoableWeatherState>();
            state = state.WithWeather(action6);

            state.Should().BeEquivalentTo(
               new UndoableWeatherState
               {
                   Past = new[]
                   {
                          new WeatherState()
                          {
                            Initialized = false,
                            Loading = false,
                            Forecasts = Array.Empty<WeatherForecast>()
                          },
                          new WeatherState()
                          {
                                Initialized = false,
                                Loading = true,
                                Forecasts = Array.Empty<WeatherForecast>()
                          },
                          new WeatherState()
                          {
                               Initialized = false,
                               Loading = true,
                               Forecasts = forecasts.ToArray()
                          },
                          new WeatherState()
                          {
                               Initialized = false,
                               Loading = false,
                               Forecasts = forecasts.ToArray()
                          },
                   },
                   Present = new WeatherState()
                   {
                       Initialized = true,
                       Loading = false,
                       Forecasts = forecasts.ToArray()
                   },
               });

            var action7 = new WeatherSetLoadingAction(true);
            state = state.WithWeather(action7);

            var action8 = new WeatherLoadForecastsAction();
            state = state.WithWeather(action8);

            var action9 = new WeatherSetLoadingAction(false);
            state = state.WithWeather(action9);

            var new_forecasts = state.Present.Forecasts;

            state.Present.Forecasts.Should().NotBeEmpty();

            // since we have performed a new action, the forecast should be different
            state.Present.Forecasts.Should().BeEquivalentTo(new_forecasts);

            var action10 = new UndoAllAction<UndoableWeatherState>();
            state = state.WithWeather(action10);

            state.Should().BeEquivalentTo(
               new UndoableWeatherState
               {

                   Present = new WeatherState()
                   {
                       Initialized = false,
                       Loading = false,
                       Forecasts = Array.Empty<WeatherForecast>()
                   },
                   Future = new[]
                   {
                          new WeatherState()
                          {
                                Initialized = false,
                                Loading = true,
                                Forecasts = Array.Empty<WeatherForecast>()
                          },
                          new WeatherState()
                          {
                               Initialized = false,
                               Loading = true,
                               Forecasts = forecasts.ToArray()
                          },
                          new WeatherState()
                          {
                               Initialized = false,
                               Loading = false,
                               Forecasts = forecasts.ToArray()
                          },
                          new WeatherState()
                          {
                               Initialized = true,
                               Loading = false,
                               Forecasts = forecasts.ToArray()
                          },
                          new WeatherState()
                          {
                               Initialized = true,
                               Loading = true,
                               Forecasts = forecasts.ToArray()
                          },
                          new WeatherState()
                          {
                               Initialized = true,
                               Loading = true,
                               Forecasts = new_forecasts.ToArray()
                          },
                          new WeatherState()
                          {
                               Initialized = true,
                               Loading = false,
                               Forecasts = new_forecasts.ToArray()
                          },
                   }
               });

            var action11 = new RedoAllAction<UndoableWeatherState>();
            state = state.WithWeather(action11);

            state.Should().BeEquivalentTo(
               new UndoableWeatherState
               {
                   Past = new[]
                   {
                          new WeatherState()
                            {
                                Initialized = false,
                                Loading = false,
                                Forecasts = Array.Empty<WeatherForecast>()
                          },
                          new WeatherState()
                          {
                                Initialized = false,
                                Loading = true,
                                Forecasts = Array.Empty<WeatherForecast>()
                          },
                          new WeatherState()
                          {
                               Initialized = false,
                               Loading = true,
                               Forecasts = forecasts.ToArray()
                          },
                          new WeatherState()
                          {
                               Initialized = false,
                               Loading = false,
                               Forecasts = forecasts.ToArray()
                          },
                          new WeatherState()
                          {
                               Initialized = true,
                               Loading = false,
                               Forecasts = forecasts.ToArray()
                          },
                          new WeatherState()
                          {
                               Initialized = true,
                               Loading = true,
                               Forecasts = forecasts.ToArray()
                          },
                          new WeatherState()
                          {
                               Initialized = true,
                               Loading = true,
                               Forecasts = new_forecasts.ToArray()
                          },
                   },
                   Present = new WeatherState()
                   {
                       Initialized = true,
                       Loading = false,
                       Forecasts = new_forecasts.ToArray()
                   }
               });
        }
    }
}
