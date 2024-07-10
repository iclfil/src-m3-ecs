using MatchThree.Rack;
using Client.Game.Utils;
using SevenBoldPencil.EasyEvents;

namespace Client.MatchThree.Board.ECS.Events
{
    public struct InputEvent : IEventSingleton
    {
        public Int2 Selected;
        public Int2 Direction;
        public InputType Type;
    }

    public struct InputSwappedEvent : IEventSingleton
    {
        public Int2 First;
        public Int2 Second;
    }
}
