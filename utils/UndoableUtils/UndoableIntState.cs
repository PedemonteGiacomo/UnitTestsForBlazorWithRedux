using Fluxor.Undo;

namespace UnitTestsForBlazorWithRedux.utils.UndoableUtils
{
    public sealed record UndoableIntState : Undoable<UndoableIntState, int>;
}
