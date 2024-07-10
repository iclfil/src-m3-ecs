using Client.Game.Utils;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Events
{
    public struct InputSwappedEvent : IEventSingleton
    {
        public Int2 First;
        public Int2 Second;
    }

    public struct DoubleClickEvent : IEventSingleton
    {
        public Int2 ClickedItem;
    }
}
