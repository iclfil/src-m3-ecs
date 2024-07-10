using MatchThree.ECS.Modules;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Events
{
    public struct EnableStateEvent : IEventSingleton
    {
        public NameModule RackState;
    }

    public struct StartStateEvent : IEventSingleton
    {
        public NameModule StartingState;
    }
}