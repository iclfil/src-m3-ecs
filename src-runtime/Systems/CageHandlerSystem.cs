using MatchThree.Rack.ECS.Tags;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;

namespace MatchThree.Rack.ECS.Systems
{
    public class CageHandlerSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ComboInfo, Items, MatchedTag>> f_matchedItems = default;
        private readonly EcsFilterInject<Inc<CageTag, ItemData>> f_cages = "Board";
        private readonly EcsPoolInject<DestroyTag> p_destroyTag = default;
        private readonly IBoardFacade _board;

        public CageHandlerSystem(IBoardFacade board)
        {
            _board = board;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in f_matchedItems.Value)
            {
                ref var items = ref f_matchedItems.Pools.Inc2.Get(entity).MatchedItems;

                for (int i = items.Count - 1; i >= 0; i--)
                {

                    if (_board.HasTag<CageBlockedItemTag>(items[i]))
                    {
                        //Удаляем клетку.
                        var blockedItem = items[i];

                        if (_board.TryGetLastItem(blockedItem.Position, out var cage))
                        {
                            if (cage.Type == TypeItems.Cage)
                            {
                                if (_board.HasTag<DestroyTag>(cage) == false)
                                    _board.AddTag<DestroyTag>(cage);
                            }
                        }

                        _board.FallBehavior(blockedItem, true);
                        _board.SwapBehavior(blockedItem, true);
                        _board.RemoveTag<CageBlockedItemTag>(blockedItem);

                        MyLog.Log("System/Green", $"Cage Block Item {blockedItem.Position} {blockedItem.Type}", GetType());

                        items.Remove(blockedItem);

                        //помечаем клетку на удаление
                    }
                }
            }
        }
    }
}