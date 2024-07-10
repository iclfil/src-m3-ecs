using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Tags;

namespace MatchThree.Rack.ECS.Systems
{
    public class DestroyGlassesSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<GlassesTag, Items, DestroyTag>> _glasses = default;

        private readonly EcsFilterInject<Inc<StonesTag, Items, DestroyTag>> _stones = default;

        private readonly EcsFilterInject<Inc<DicesTag, Items, DestroyTag>> _dices = default;

        private readonly EcsFilterInject<Inc<SlimeTag, ItemData, DestroyTag>> f_destroySlimes = "Board";

        private readonly EcsFilterInject<Inc<CageTag, ItemData, DestroyTag>> f_destroyCages = "Board";

        private readonly IBoardFacade _board;
        private readonly OnDestroyBlocks _destroyBlocks;
        private EcsWorld _world;

        private List<IItem> _destroySlimes;
        private List<IItem> _destroyCages;


        public DestroyGlassesSystem(IBoardFacade board, OnDestroyBlocks destroyBlocks)
        {
            _destroyCages = new List<IItem>();
            _destroySlimes = new List<IItem>();
            _board = board;
            _destroyBlocks = destroyBlocks;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            DestroyGlasses();
            DestroyStones();
            DestroyDices();
            DestroySlimes();
            DestroyCages();
        }

        private void DestroyCages()
        {
            foreach (var entity in f_destroyCages.Value)
            {
                var item = f_destroyCages.Pools.Inc2.Get(entity).Item;

                if (_board.Has(item) == false)
                    continue;

                _destroyCages.Add(item);
                _board.DeleteIncludeItem(item);
            }

            if (_destroyCages.Count > 0)
            {
                _destroyBlocks?.Invoke(TypeItems.Cage, _destroyCages);
                _destroyCages.Clear();
            }
        }

        private void DestroySlimes()
        {
            foreach (var entity in f_destroySlimes.Value)
            {
                var slime = f_destroySlimes.Pools.Inc2.Get(entity).Item;

                if (_board.Has(slime) == false)
                    continue;

                _destroySlimes.Add(slime);
                _board.DeleteIncludeItem(slime);
                _world.DelEntity(entity);
            }

            //TODO - костыль. Если список создавать в самом методе, то список заполняется на одном проходе, не успевает отправиться, и снова создается, теряя все добавленные итем.
            if (_destroySlimes.Count > 0)
            {
                _destroyBlocks?.Invoke(TypeItems.Slime, _destroySlimes);
                _destroySlimes.Clear();
            }
        }

        private void DestroyDices()
        {
            foreach (var _Enty in _dices.Value)
            {
                var items = _dices.Pools.Inc2.Get(_Enty).MatchedItems;

                foreach (var dice in items)
                {
                    if (_board.Has(dice) == false)
                        continue;

                    _board.DeleteIncludeItem(dice);
                }

                _destroyBlocks?.Invoke(TypeItems.Dice_1, items);
                _world.DelEntity(_Enty);
            }
        }

        private void DestroyStones()
        {
            foreach (var _Enty in _stones.Value)
            {
                var items = _stones.Pools.Inc2.Get(_Enty).MatchedItems;

                foreach (var stone in items)
                {
                    if (_board.Has(stone) == false)
                        continue;

                    _board.DeleteIncludeItem(stone);
                }

                _destroyBlocks?.Invoke(TypeItems.Stone_1, items);
                _world.DelEntity(_Enty);
            }
        }

        private void DestroyGlasses()
        {
            foreach (var _Enty in _glasses.Value)
            {
                var items = _glasses.Pools.Inc2.Get(_Enty).MatchedItems;

                foreach (var glass in items)
                {
                    if (_board.Has(glass) == false)
                        continue;

                    _board.DeleteIncludeItem(glass);
                }

                _destroyBlocks?.Invoke(TypeItems.Glass_1, items);
                _world.DelEntity(_Enty);
            }
        }
    }
}