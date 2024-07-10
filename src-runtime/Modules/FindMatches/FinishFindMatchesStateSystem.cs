using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS;
using MatchThree.Rack.ECS.Events;
using MatchThree.Rack.ECS.SharedDatas;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.ECS.Modules.FindMatches
{
    public class FinishFindMatchesStateSystem : IEcsInitSystem,  IEcsRunSystem
    {
        private readonly EventsBus _eventsBus;
        //readonly EcsFilterInject<Inc<FindMatchesFinish>> __state = default;

        private SharedData _sharedData;

        public FinishFindMatchesStateSystem(EventsBus eventsBus)
        {
            _eventsBus = eventsBus;
        }
        public void Init(IEcsSystems systems)
        {
            _sharedData = systems.GetShared<SharedData>();
        }

        public void Run(IEcsSystems systems)
        {
            ref StateFinishedEvent data = ref _eventsBus.NewEventSingleton<StateFinishedEvent>();
            data.Name = NameModule.FindMatches;
            data.Result = _sharedData.HasMatches ? ModuleResult.HasMatches : ModuleResult.NoMatches;
            //foreach (var entity in __state.Value)
            //{
            //    FindMatchesFinish data = __state.Pools.Inc1.Get(entity);
            //    ref StateFinishedEvent f = ref _eventsBus.NewEventSingleton<StateFinishedEvent>();
            //    f.RackState = RackState.FindMatches;

            //    bool hasMatches = false;

            //    if (data.HasMatches)
            //    {
            //        hasMatches = true;
            //        f.StateResult = StateResult.HasMatches;
            //    }
            //    else
            //        f.StateResult = StateResult.NoMatches;

            //    findMatchesFinished?.Invoke(hasMatches);

            //    systems.GetWorld().DelEntity(entity);
            //}
        }


    }
}