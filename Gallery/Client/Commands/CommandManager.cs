using System.Collections.Generic;

namespace Client.Commands
{
    public static class CommandManager
    {
        public static readonly Stack<Command> _undoStack = new Stack<Command>();
        public static  readonly Stack<Command> _redoStack = new Stack<Command>();

        public static void ExecuteCommand(Command command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public static  void Undo()
        {
            if (_undoStack.Count > 0)
            {
                Command command = _undoStack.Pop();
                command.UnExecute();
                _redoStack.Push(command);
            }
        }

        public static void Redo()
        {
            if (_redoStack.Count > 0)
            {
                Command command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
        }
    }
}
