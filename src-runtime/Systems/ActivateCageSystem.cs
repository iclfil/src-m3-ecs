using MatchThree.Rack.ECS;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS.Tags;
using UnityEngine;

namespace MatchThree.Rack.ECS.Systems
{
    /// <summary>
    /// Убирает у всех Items под Cage поведение SwapBehavior и FallBehavior
    /// </summary>
    public class ActivateCageSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ItemData, CageTag>> _cages = "Board";//TODO - мне нужна только позиция этого объекта. Можно позицияю хранить на сущности.
        private readonly IBoardFacade _board;

        public ActivateCageSystem(IBoardFacade board)
        {
            _board = board;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var e_cage in _cages.Value)
            {
                //Берем объект под клеткой, если он там есть. 
                //Отключаем у него поведение SwapBehavior, FallBehavior

                var cage = _cages.Pools.Inc1.Get(e_cage).Item;

                if (_board.TryGetItemUnder(cage, out var underItem))
                {
                    if (_board.HasBehavior<SwapBehavior>(underItem) && _board.HasBehavior<FallBehavior>(underItem))
                    {
                        _board.SwapBehavior(underItem, false);
                        _board.FallBehavior(underItem, false);
                        _board.AddTag<CageBlockedItemTag>(underItem);

                        MyLog.Log("System", $"Setup Cage on {underItem.Type} {underItem.Position}", GetType());
                    }
                }
            }
        }
    }
}