using BlazorWithRedux.Store.Weather.Actions;
using BlazorWithRedux.Store.Weather.Reducers;
using BlazorWithRedux.Store.Weather.State;
using BlazorWithRedux.Store.Weather.Reducers;
using BlazorWithRedux.Store.Weather.State;
using Fluxor.Undo;
using BlazorWithRedux.Store.Weather.Effects;
using Fluxor;
using static System.Net.WebRequestMethods;

namespace UnitTestsForBlazorWithRedux.utils.WeatherUtils
{
    public static class UndoableWeatherFeatureExtensions
    {
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
    }
}
