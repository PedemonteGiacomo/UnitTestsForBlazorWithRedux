using BlazorWithRedux.Store.Weather.Actions;
using BlazorWithRedux.Store.Weather.Reducers;
using BlazorWithRedux.Store.Weather.State;
using BlazorWithRedux.Store.Weather.Reducers;
using BlazorWithRedux.Store.Weather.State;
using Fluxor.Undo;
using BlazorWithRedux.Store.Weather.Effects;
using Fluxor;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace UnitTestsForBlazorWithRedux.utils.WeatherUtils
{
    public static class UndoableWeatherFeatureExtensions
    {
        private static WeatherForecast[] forecasts;
        public static UndoableWeatherState CreateInitialState(this UndoableWeatherState state)
        {
            return new()
            {
                Present = new WeatherState()
                {
                    Initialized = false,
                    Loading = false,
                    Forecasts = Array.Empty<WeatherForecast>()
                }
            };
        }

        public static UndoableWeatherState WithWeather(this UndoableWeatherState state, object action)
        {
            switch (action) // This is the action that we are going to use to test the state
            {
                // Depending on the action type, we are going to call the respective reducer
                case WeatherSetForecastsAction setForecastsAction:
                    return WeatherReducers.OnSetForecasts(state, setForecastsAction); // to reproduce the load effect we will call this reducer after the retrieving of the forecats is done
                case WeatherSetLoadingAction setLoadingAction:
                    return WeatherReducers.OnSetLoading(state, setLoadingAction);
                case WeatherSetInitializedAction setInitializedAction:
                    return WeatherReducers.OnSetInitialized(state);
                case WeatherLoadForecastsAction loadForecastsAction:
                    return WithNewForecasts(state);
                case UndoAction<UndoableWeatherState> undoAction:
                    return WeatherReducers.ReduceUndoAction(state, undoAction);
                case RedoAction<UndoableWeatherState> redoAction:
                    return WeatherReducers.ReduceRedoAction(state, redoAction);
                case JumpAction<UndoableWeatherState> jumpAction:
                    return WeatherReducers.ReduceJumpAction(state, jumpAction);
                case UndoAllAction<UndoableWeatherState> undoAllAction:
                    return WeatherReducers.ReduceUndoAllAction(state, undoAllAction);
                case RedoAllAction<UndoableWeatherState> redoAllAction:
                    return WeatherReducers.ReduceRedoAllAction(state, redoAllAction);
                default:
                    throw new ArgumentException("Invalid action type", nameof(action));
            }
        }

        // since now the json file is not in the project, we will use the jsonbin.io to retrieve the data
        // the data are stored in a json file that is publicly available and start with record that contains the array
        // so we need those classes to retrieve correctly the "dataset"
        public class WeatherData
        {
            public WeatherForecast[] Record { get; set; }
            public Metadata Metadata { get; set; }
        }

        public class Metadata
        {
            public string Id { get; set; }
            public bool Private { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public static async Task LoadForecasts(this UndoableWeatherState state)
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetFromJsonAsync<WeatherData>("https://api.jsonbin.io/v3/b/65f31dfe1f5677401f3d79f1");
            var forecasts_var = response?.Record; // Extract the forecasts from the JSON data
            if (forecasts_var != null)
            {
                var randomForecasts = GetRandomForecasts(forecasts_var); // Get a randomly set of forecasts
                forecasts = randomForecasts;
            }
        }


        // return a state with the randomForecasts as the new forecasts
        public static UndoableWeatherState WithNewForecasts(this UndoableWeatherState state)
        {
            Task.Run(async () =>
            {
                await LoadForecasts(state);
                state = state.WithWeather(new WeatherSetForecastsAction(forecasts));
            }).Wait();

            return state;
        }

        private static WeatherForecast[] GetRandomForecasts(WeatherForecast[] forecasts)
        {
            var random = new Random();
            var randomForecasts = forecasts.OrderBy(x => random.Next()).ToArray();
            return randomForecasts;
        }
    }
}
