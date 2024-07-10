using System.Collections.Generic;
using System.Linq;
using Client.Game.Utils;
using MatchThree.Rack.Common;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.ECS.SharedDatas;
using MatchThree.Rack.ECS.Tags;
using MatchThree.Rack.Utils;

namespace MatchThree.Rack.ECS.Systems
{

    public class SlimeBlocksHandlerSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<ItemData, SlimeTag>> f_slimes = "Board";
        private readonly EcsPoolInject<DestroyTag> p_destroyTag = "Board";

        private readonly IBoardFacade _board;

        private List<IItem> _matchedSlimes;
        private EcsWorld _world;


        public SlimeBlocksHandlerSystem(IBoardFacade board)
        {
            _matchedSlimes = new List<IItem>();
            _board = board;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (f_slimes.Value.GetEntitiesCount() == 0)
                return;

            systems.GetShared<SharedData>().SlimeActivate = true;

            foreach (var entity in f_slimes.Value)
            {
                if (p_destroyTag.Value.Has(entity))
                    continue;

                var slime = f_slimes.Pools.Inc1.Get(entity).Item;
                if (FindMatchedItemAroundSlime(slime))
                {
                    MyLog.Log("System/Green", $"Matches Around Slime {slime.Position}", GetType());
                    p_destroyTag.Value.Add(entity);
                    systems.GetShared<SharedData>().SlimeActivate = false;
                }
            }
        }

        private bool FindMatchesAroundSlimes(IItem slime)
        {
            //Если ни одна фишка не уничтожает слайм, то он должен создать свою копию.
            if (ItemsHandlerUtils.TryGetPositions(slime, out var findItems, _board))
            {
                return findItems.ToList().Exists(x => x.IsMatched);
            }
            return false;
        }

        private bool FindMatchedItemAroundSlime(IItem slime)
        {
            var pos = slime.Position;

            var up = pos + new Int2(0, 1);

            if (FindMatchedItem(up)) return true;

            var left = pos + new Int2(-1, 0);
            if (FindMatchedItem(left)) return true;

            var right = pos + new Int2(1, 0);
            if (FindMatchedItem(right)) return true;

            var down = pos + new Int2(0, -1);
            if (FindMatchedItem(down)) return true;

            return false;
        }

        private bool FindMatchedItem(Int2 up)
        {
            if (_board.TryGetLastItem(up, out var lastItem))
            {
                if (lastItem.Type != TypeItems.Slime)
                    if (lastItem.IsMatched)
                        return true;
            }

            return false;
        }
    }
}