using BlazorWithRedux.Store.Counter.State;
using Fluxor;

namespace UnitTestsForBlazorWithRedux.utils.CounterUtils
{
    public class CounterTestsFixture
    {
        public UndoableCounterState state { get; set; }

        public CounterTestsFixture()
        {
            // Initialize the state here to avoid making the new instance of the state in the test be more heavy since we don't need anymore to set the Present property that in the Undoable library is required
            state = new UndoableCounterState();
            state.CreateInitialState();
        }
    }
}
