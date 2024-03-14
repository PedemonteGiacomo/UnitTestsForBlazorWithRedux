using BlazorWithRedux.Store.Counter.Actions;
using BlazorWithRedux.Store.Counter.State;
using Fluxor.Undo;

using BlazorWithRedux.Store.Counter.Reducers;

namespace UnitTestsForBlazorWithRedux.utils.CounterUtils
{
    public static class CounterReducersExtensions
    {
        public static UndoableCounterState Reduce(this CounterReducers counterReducers, UndoableCounterState state, object action)
        {
            switch (action)
            {
                case AddCounter addCounterAction:
                    return CounterReducers.OnAddCounter(state, addCounterAction);
                case SubCounter subCounterAction:
                    return CounterReducers.OnSubtractCounter(state, subCounterAction);
                case UndoAction<UndoableCounterState> undoAction:
                    return CounterReducers.ReduceUndoAction(state, undoAction);
                case RedoAction<UndoableCounterState> redoAction:
                    return CounterReducers.ReduceRedoAction(state, redoAction);
                case JumpAction<UndoableCounterState> jumpAction:
                    return CounterReducers.ReduceJumpAction(state, jumpAction);
                case UndoAllAction<UndoableCounterState> undoAllAction:
                    return CounterReducers.ReduceUndoAllAction(state, undoAllAction);
                case RedoAllAction<UndoableCounterState> redoAllAction:
                    return CounterReducers.ReduceRedoAllAction(state, redoAllAction);
                default:
                    throw new ArgumentException("Invalid action type", nameof(action));
            }
        }
    }
}
