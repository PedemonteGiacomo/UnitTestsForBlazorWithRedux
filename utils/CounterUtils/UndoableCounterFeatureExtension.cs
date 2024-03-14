using BlazorWithRedux.Store.Counter.Reducers;
using BlazorWithRedux.Store.Counter.State;

namespace UnitTestsForBlazorWithRedux.utils.CounterUtils
{
    public static class UndoableCounterFeatureExtensions
    {
        public static UndoableCounterState CreateInitialState(this UndoableCounterState state)
        {
            return new UndoableCounterState { Present = new CounterState { Count = 0 } };
        }

        public static UndoableCounterState WithCounter(this UndoableCounterState state, int count)
        {
            if (count == 0) return state;
            if (count > 0) return CounterReducers.OnAddCounter(state, new());
            return CounterReducers.OnSubtractCounter(state, new());

            // single line solution
            // return count == 0 ? state : count > 0 ? CounterReducers.OnAddCounter(state, new()) : CounterReducers.OnSubtractCounter(state, new());
        }
    }
}
