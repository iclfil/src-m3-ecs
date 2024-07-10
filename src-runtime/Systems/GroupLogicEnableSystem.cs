using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.ExtendedSystems;
using MatchThree.Rack.ECS.Events;
using SevenBoldPencil.EasyEvents;
using UnityEngine;

namespace MatchThree.Rack.ECS.Systems
{
    public class StateEnableSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsWorld world;
        public EventsBus Bus { get; }

        public StateEnableSystem(EventsBus bus)
        {
            this.Bus = bus;
        }

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (Bus.HasEventSingleton<DisableStateEvent>())
            {
                string name = Bus.GetEventBodySingleton<DisableStateEvent>().Name.ToString();
                EnableSystems(false, name);
                Bus.DestroyEventSingleton<DisableStateEvent>();
            }

            if (Bus.HasEventSingleton<EnableStateEvent>())
            {
                string name = Bus.GetEventBodySingleton<EnableStateEvent>().RackState.ToString();
                EnableSystems(true, name);
                Bus.DestroyEventSingleton<EnableStateEvent>();
            }
        }

        private void EnableSystems(bool enable, string name)
        {
#if UNITY_EDITOR
            if (enable)
            {
                MyLog.Log("State/Enter", $"Enter Module {name}", GetType());
            }
            else
            {
                MyLog.Log("State/Exit", $"Exit Module {name}", GetType());
            }

#endif

            var entity = world.NewEntity();
            ref var evt = ref world.GetPool<EcsGroupSystemState>().Add(entity);
            evt.Name = name;
            evt.State = enable;
        }
    }
}