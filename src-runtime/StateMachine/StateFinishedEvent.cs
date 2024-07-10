using MatchThree.ECS.Modules;
using MatchThree.Rack.Common;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Events
{
    public struct StateFinishedEvent : IEventSingleton
    {
        public NameModule Name;
        public ModuleResult Result;
    }

    public struct StateFinish : IEventReplicant
    {
        public int Kisa;
    }
}
