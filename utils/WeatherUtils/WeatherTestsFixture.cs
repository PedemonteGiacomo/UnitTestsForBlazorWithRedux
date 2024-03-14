using BlazorWithRedux.Store.Weather.State;
using BlazorWithRedux.Store.Weather.State;
using Fluxor;

namespace UnitTestsForBlazorWithRedux.utils.WeatherUtils
{
    public class WeatherTestsFixture
    {
        public UndoableWeatherState state { get; set; }

        public WeatherTestsFixture()
        {
            // Initialize the state here to avoid making the new instance of the state in the test be more heavy since we don't need anymore to set the Present property that in the Undoable library is required
            state = new UndoableWeatherState();
            state.CreateInitialState();
        }
    }
}
