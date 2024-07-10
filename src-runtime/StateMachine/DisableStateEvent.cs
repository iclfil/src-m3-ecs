using MatchThree.ECS.Modules;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Events
{
    public struct DisableStateEvent : IEventSingleton
    {
        public NameModule Name;
    }
}