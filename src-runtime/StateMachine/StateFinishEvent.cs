using MatchThree.Rack;
using MatchThree.Rack.Common;
using SevenBoldPencil.EasyEvents;

namespace Client.MatchThree.Board.ECS.Events
{
    public struct StateFinishEvent : IEventSingleton
    {
        public RackState RackState;
        public ModuleResult ModuleResult;
    }
}
