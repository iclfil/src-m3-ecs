using Leopotam.EcsLite;
using MatchThree.Rack.ECS.Events;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public class RackNotifySystem : IEcsRunSystem
    {
        private readonly RackModuleNotify notify;
        public EventsBus Bus { get; set; }


        public RackNotifySystem(RackModuleNotify notify, EventsBus bus)
        {
            Bus = bus;
            this.notify = notify;
        }

        public void Run(IEcsSystems systems)
        {
            if (Bus.HasEventSingleton<StateFinishedEvent>())
            {
                var finishedEvent = Bus.GetEventBodySingleton<StateFinishedEvent>();
                var name = finishedEvent.Name;
                var result = finishedEvent.Result;
                notify.Notify(name, result);
            }
        }
    }
}