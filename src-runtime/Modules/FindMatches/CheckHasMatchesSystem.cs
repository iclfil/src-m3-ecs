using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack;
using MatchThree.Rack.ECS;
using MatchThree.Rack.ECS.SharedDatas;

namespace MatchThree.ECS.Modules.FindMatches
{
    public class CheckHasMatchesSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<SimpleCombo>> f_simpleCombo = default;
        private readonly EcsFilterInject<Inc<ComplexCombo>> f_complexCombo = default;

        //private readonly EcsFilterInject<Inc<SwapTag>> _swap = default;
        //private readonly EcsPoolInject<UnSwapTag> _unswapTag = default;

        //readonly EcsFilterInject<Inc<ComboInfo>> __hasCombos = default;
        //readonly EcsFilterInject<Inc<MatchesTag>> __hasMatches = default;
        //readonly EcsFilterInject<Inc<FindIngredient>> __hasFindedeIngredient = default;

        private EcsWorld _world;
        private SharedData _sharedData;


        public void Init(IEcsSystems systems)
        {
            _sharedData = systems.GetShared<SharedData>();
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (HasMatches())
                _sharedData.HasMatches = true;
            else
                _sharedData.HasMatches = false;
        }

        private bool HasMatches()
        {
            if (f_simpleCombo.Value.GetEntitiesCount() > 0)
                return true;

            if (f_complexCombo.Value.GetEntitiesCount() > 0)
                return true;

            //if (__hasFindedeIngredient.Value.GetEntitiesCount() > 0)
            //{
            //    _world.DelEntity(__hasFindedeIngredient.Value.GetRawEntities()[0]);
            //    return true;
            //}

            //if (__hasCombos.Value.GetEntitiesCount() > 0)
            //    return true;

            //if (__hasMatches.Value.GetEntitiesCount() > 0)
            //    return true;

            return false;
        }

        //private void Matches()
        //{
        //    MyLog.Log("System/Green", $"Find Matches", GetType());
        //    int entity = _world.NewEntity();
        //    ref FindMatchesFinish w = ref _world.GetPool<FindMatchesFinish>().Add(entity);
        //    w.HasMatches = true;
        //}

        //private void NoMatches()
        //{
        //    //foreach (var entity in _swap.Value)
        //    //{
        //    //    _unswapTag.Value.Add(entity);
        //    //}

        //    MyLog.Log("System/Red", $"Matches not Finded", GetType());
        //    int eW = _world.NewEntity();
        //    ref FindMatchesFinish w = ref _world.GetPool<FindMatchesFinish>().Add(eW);
        //    w.HasMatches = false;
        //}
    }
}