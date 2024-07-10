using System.Collections.Generic;
using MatchThree.Rack;
using MatchThree.Rack.ECS;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using MatchThree.Rack.Utils;

namespace MatchThree.Rack.ECS.Systems
{
    /// <summary>
    /// GlassesTag, Items, MatchedTag
    /// </summary>
    public class GlassBlocksHandlerSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<Items, MatchedTag>> _simpleCombos = default;

        private readonly EcsPoolInject<GlassesTag> _pool = default;
        private readonly EcsPoolInject<StonesTag> _pool2 = default;
        private readonly EcsPoolInject<DicesTag> _pool3 = default;


        private readonly IBoardFacade _board;
        private EcsWorld _world;
        private HashSet<IItem> _glasses;
        private HashSet<IItem> _stones;
        private HashSet<IItem> _dices;

        public GlassBlocksHandlerSystem(IBoardFacade board)
        {
            _board = board;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (_simpleCombos.Value.GetEntitiesCount() == 0)
                return;

            _glasses = new HashSet<IItem>();
            _stones = new HashSet<IItem>();
            _dices = new HashSet<IItem>();

            foreach (var _Enty in _simpleCombos.Value)
            {
                var items = _simpleCombos.Pools.Inc1.Get(_Enty).MatchedItems;

                foreach (var item in items)
                {
                    DicesAndStonesHandler(item);
                    GlassesHandler(item);
                }
            }

            if (_stones.Count > 0)
            {
                CreateMatchedStones(_stones);
                _stones.Clear();
            }

            if (_glasses.Count > 0)
            {
                CreateMatchedGlasses(_glasses);
                _glasses.Clear();
            }

            if (_dices.Count > 0)
            {
                CreateMatchedDices(_dices);
                _dices.Clear();
            }
        }


        private void CreateMatchedStones(HashSet<IItem> stones)
        {
            var entity = _world.NewEntity();
            _pool2.Value.Add(entity);
            ref var Items = ref _simpleCombos.Pools.Inc1.Add(entity);
            Items.MatchedItems = new List<IItem>(stones);
            _simpleCombos.Pools.Inc2.Add(entity);
        }

        private void CreateMatchedDices(HashSet<IItem> dices)
        {
            var entity = _world.NewEntity();
            _pool3.Value.Add(entity);
            ref var Items = ref _simpleCombos.Pools.Inc1.Add(entity);
            Items.MatchedItems = new List<IItem>(dices);
            _simpleCombos.Pools.Inc2.Add(entity);
        }

        private void DicesAndStonesHandler(IItem matchedItem)
        {
            //»щет камни по бока
            if (ItemsHandlerUtils.TryGetPositions(matchedItem, out var items, _board))
            {
                foreach (var item in items)
                {
                    if (ItemsProperties.IsStone(item.Type) && item.IsMatched == false)
                    {
                        _stones.Add(item);
                    }

                    if (ItemsProperties.IsDice(item.Type))
                        _dices.Add(item);
                }
            }
        }




        private void GlassesHandler(IItem matchedItem)
        {
            if (!_board.TryGetItemUnder(matchedItem, out var underItem)) return;

            if (underItem.Type == TypeItems.Glass_1 || underItem.Type == TypeItems.Glass_2)
            {
                MyLog.Log("System/Green", $"Find Glass for Match {underItem.Type} {underItem.Position}", GetType());
                _glasses.Add(underItem);
            }
        }

        private void CreateMatchedGlasses(HashSet<IItem> glasses)
        {
            var entity = _world.NewEntity();
            _pool.Value.Add(entity);
            ref var Items = ref _simpleCombos.Pools.Inc1.Add(entity);
            Items.MatchedItems = new List<IItem>(glasses);
            _simpleCombos.Pools.Inc2.Add(entity);

            MyLog.Log("System/Green", $"Create MatchedGlasses Count {glasses.Count}", GetType());
        }
    }
}