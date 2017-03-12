using System.Collections.Generic;

namespace CorpoGameApp.Controllers
{
    public sealed class GameMessageType
    {
        public enum Enum
        {
            CreateGameAlreadyInProgressError,
            CreateGameUnknownError,
            CreateGameSuccess,
            None
        }

        public static readonly SortedList<Enum, GameMessageType> Values = new SortedList<Enum, GameMessageType>();

        public static readonly GameMessageType None = new GameMessageType(
            Enum.None,
            string.Empty,
            true);
        public static readonly GameMessageType CreateGameAlreadyInProgressError = new GameMessageType(
            Enum.CreateGameAlreadyInProgressError,
            "Cannot create new game - game already in progress", 
            false);
        public static readonly GameMessageType CreateGameUnknownError = new GameMessageType(
            Enum.CreateGameUnknownError,
            "Cannot create new game - unknown error", 
            false);
        public static readonly GameMessageType CreateGameSuccess = new GameMessageType(
            Enum.CreateGameSuccess,
            "New game created successfully", 
            true);

        public readonly Enum Value;
        public readonly string Text;
        public readonly bool Success;

        private GameMessageType(Enum value, string text, bool success)
        {
            Value = value;
            Text = text;
            Success = success;
            Values.Add(value, this);
        }

        public static implicit operator GameMessageType(Enum value)
        {
            return Values[value];
        }

        public static implicit operator Enum(GameMessageType value)
        {
            return value.Value;
        }
    }
}