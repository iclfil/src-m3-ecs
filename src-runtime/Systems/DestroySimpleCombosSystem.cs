using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.ECS.Modules.FindMatches;

namespace MatchThree.Rack.ECS.Systems
{
    public class DestroySimpleCombosSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<ComboInfo, Items, DestroyTag, SimpleCombo>> _simpleCombos = default;
        private readonly IBoardFacade _board;
        private readonly OnDestroyCombo _destroyCombo;
        private EcsWorld _world;

        public DestroySimpleCombosSystem(IBoardFacade board, OnDestroyCombo destroyCombo)
        {
            _board = board;
            _destroyCombo = destroyCombo;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var combo in _simpleCombos.Value)
            {
                
                var matchedItems = _simpleCombos.Pools.Inc2.Get(combo).MatchedItems;

                for (int i = 0; i < matchedItems.Count; i++)
                {
                    var item = matchedItems[i];
                    if (_board.Has(item) == false)
                    {
                        matchedItems.Remove(item);
                        continue;
                    }

                    _board.DeleteIncludeItem(item);
                }


                _destroyCombo?.Invoke(ComboType.SimpleMatch, matchedItems);
                _world.DelEntity(combo);
            }
        }
    }
}

