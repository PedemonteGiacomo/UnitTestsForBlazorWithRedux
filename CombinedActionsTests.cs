using BlazorWithRedux.Store.Counter.State;
using BlazorWithRedux.Store.Counter.Actions;
using BlazorWithRedux.Store.Weather.State;
using BlazorWithRedux.Store.Weather.Actions;
using Fluxor;
using Fluxor.Undo;
using FluentAssertions;
using Xunit;
using UnitTestsForBlazorWithRedux.utils.CounterUtils;
using UnitTestsForBlazorWithRedux.utils.WeatherUtils;

namespace UnitTestsForBlazorWithRedux
{

    namespace UnitTestsForBlazorWithRedux
    {
        public class CombinedActionsTests(CounterTestsFixture counterFixture, WeatherTestsFixture weatherFixture) : IClassFixture<CounterTestsFixture>, IClassFixture<WeatherTestsFixture>
        {
            private readonly CounterTestsFixture _counterFixture = counterFixture;
            private readonly WeatherTestsFixture _weatherFixture = weatherFixture;

            [Fact]
            public void TestCombinedSimpleActions()
            {
                // take the fixture data
                var counterState = _counterFixture.state;
                var weatherState = _weatherFixture.state;

                // Counter state
                counterState = counterState.WithCounter(1);
                counterState.Present.Count.Should().Be(1);

                // Weather state
                var action = new WeatherSetLoadingAction(true);
                weatherState = weatherState.WithWeather(action);
                weatherState.Present.Loading.Should().BeTrue();

                // Combined state
                var combinedState = new { Counter = counterState, Weather = weatherState };
                combinedState.Counter.Present.Count.Should().Be(1);
                combinedState.Weather.Present.Loading.Should().BeTrue();
            }

            [Fact]
            public void TestCombinedComplexActions()
            {
                // take the fixture data
                var counterState = _counterFixture.state;
                var weatherState = _weatherFixture.state;

                counterState = counterState.WithCounter(1);
                counterState.Present.Count.Should().Be(1);

                var loading = new WeatherSetLoadingAction(true);
                weatherState = weatherState.WithWeather(loading);

                var action = new WeatherLoadForecastsAction();
                weatherState = weatherState.WithWeather(action);
                var forecasts = weatherState.Present.Forecasts;

                var not_loading_anymore = new WeatherSetLoadingAction(false);
                weatherState = weatherState.WithWeather(not_loading_anymore);

                var initialized = new WeatherSetInitializedAction();
                weatherState = weatherState.WithWeather(initialized);

                weatherState.Should().BeEquivalentTo(
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
                              }
                       },
                       Present = new WeatherState()
                       {
                           Initialized = true,
                           Loading = false,
                           Forecasts = forecasts.ToArray()
                       }
                   });

                // after the weather state is updated, we can continue update the counter state independently
                counterState = counterState.WithCounter(1).WithCounter(1).WithCounter(1).WithCounter(1);
                // this above doesn't affect the weather state

                // Combined state
                var combinedState = new { Counter = counterState, Weather = weatherState };
                combinedState.Counter.Present.Count.Should().Be(5);
                combinedState.Weather.Present.Forecasts.Should().HaveCount(21);
            }

            // test if the undo and redo actions are working as expected and those states are unrelated when performing this kind of action on one state
            [Fact]
            public void TestCombinedUndoRedoActions()
            {
                // take the fixture data
                var counterState = _counterFixture.state;
                var weatherState = _weatherFixture.state;

                counterState = counterState.WithCounter(1);
                counterState.Present.Count.Should().Be(1);

                var loading = new WeatherSetLoadingAction(true);
                weatherState = weatherState.WithWeather(loading);

                var action = new WeatherLoadForecastsAction();
                weatherState = weatherState.WithWeather(action);
                var forecasts = weatherState.Present.Forecasts;

                var not_loading_anymore = new WeatherSetLoadingAction(false);
                weatherState = weatherState.WithWeather(not_loading_anymore);

                var initialized = new WeatherSetInitializedAction();
                weatherState = weatherState.WithWeather(initialized);

                weatherState.Should().BeEquivalentTo(
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
                              }
                       },
                       Present = new WeatherState()
                       {
                           Initialized = true,
                           Loading = false,
                           Forecasts = forecasts.ToArray()
                       }
                   });

                // after the weather state is updated, we can continue update the counter state independently
                counterState = counterState.WithCounter(1).WithCounter(1).WithCounter(1).WithCounter(1);
                // this above doesn't affect the weather state

                // Combined state
                var combinedState = new { Counter = counterState, Weather = weatherState };
                combinedState.Counter.Present.Count.Should().Be(5);
                combinedState.Weather.Present.Forecasts.Should().HaveCount(21);

                // undo and redo actions
                var undoAction = new UndoAction<UndoableWeatherState>();
                weatherState = weatherState.WithWeather(undoAction);

                weatherState.Should().BeEquivalentTo(
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

                combinedState = new { Counter = counterState, Weather = weatherState };
                combinedState.Counter.Present.Count.Should().Be(5); // check if the undo action didn't affect the counter state

                // now undo the actions on the counter state
                counterState = counterState.WithUndoOne();
                counterState.Present.Count.Should().Be(4);

                var redoAction = new RedoAction<UndoableWeatherState>();
                weatherState = weatherState.WithWeather(redoAction);

                weatherState.Should().BeEquivalentTo(
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
                              }
                       },
                       Present = new WeatherState()
                       {
                           Initialized = true,
                           Loading = false,
                           Forecasts = forecasts.ToArray()
                       }
                   });

                combinedState = new { Counter = counterState, Weather = weatherState };
                combinedState.Weather.Present.Forecasts.Should().HaveCount(21); // check if the undo action on the counter didn't affect the weather state
                combinedState.Counter.Present.Count.Should().Be(4); // check if the redo action didn't affect the counter state
            }
        }
    }

}
