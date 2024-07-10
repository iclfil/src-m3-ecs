using System.Collections.Generic;
using Client.Game.Utils;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS.Events;
using SevenBoldPencil.EasyEvents;
using UnityEngine;

namespace MatchThree.Rack.ECS.Systems
{
    public class SwappingHandlerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ItemData, SwapBehavior>> _swappedItems = "Board";

        private readonly EcsPoolInject<Swap> _swapTag = default;

        private EcsWorld _world;
        private readonly IBoardFacade _board;

        private Dictionary<(TypeItems, TypeItems), SwapType> _swapTypes;
        private Dictionary<(TypeItems, TypeItems), BonusSwapType> _bonusSwapTypes;

        private readonly EventsBus _eventBus;
        private readonly OnSwapItems swapItems;

        public SwappingHandlerSystem(IBoardFacade board, EventsBus eventsBus, OnSwapItems swapItems)
        {
            CreateSwapTypes();
            CreateBonusSwapTypes();
            _board = board;
            _eventBus = eventsBus;
            this.swapItems = swapItems;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (_swappedItems.Value.GetEntitiesCount() == 0)
                return;

            if (_eventBus.HasEventSingleton<InputSwappedEvent>())
            {
                var firstPos = _eventBus.GetEventBodySingleton<InputSwappedEvent>().First;
                var secondPos = _eventBus.GetEventBodySingleton<InputSwappedEvent>().Second;
                var swapType = SwapType.NoSwap;
                var bonusSwap = BonusSwapType.None;

                if (GetSwappedItems(firstPos, secondPos, out var firstItem, out var secondItem))
                {
                    MyLog.Log("SwappingHandlerSystem: Execute Swap", Color.white);

                    swapType = GetTypeSwap(firstItem.Type, secondItem.Type);
                    bonusSwap = GetTypeBonusSwap(firstItem.Type, secondItem.Type);

                    _board.Swap(firstItem, secondItem);
                    var swapContext = new SwapContext()
                    {
                        FirstItem = firstItem,
                        FirstTargetPosition = secondPos,
                        SecondItem = secondItem,
                        SecondTargetPosition = firstPos,
                        Type = swapType,
                        
                    };

                    swapItems?.Invoke(swapContext);
                }

                Finish(swapType, firstItem, secondItem);
                _eventBus.DestroyEventSingleton<InputSwappedEvent>();
            }
        }

        private bool GetSwappedItems(Int2 firstPos, Int2 secondPos, out IItem firstItem, out IItem secondItem)
        {
            firstItem = null;
            secondItem = null;

            foreach (var e_item in _swappedItems.Value)
            {
                var pos = _swappedItems.Pools.Inc1.Get(e_item).Item.Position;

                if (pos == firstPos)
                    firstItem = _swappedItems.Pools.Inc1.Get(e_item).Item;

                if (pos == secondPos)
                    secondItem = _swappedItems.Pools.Inc1.Get(e_item).Item;
            }

            if (firstItem != null && secondItem != null)
                return true;

            return false;
        }

        private void Finish(SwapType swapType, IItem firstItem, IItem secondItem)
        {
            var entity = _world.NewEntity();
            ref var data = ref _swapTag.Value.Add(entity);
            data.SwapType = swapType;
            data.FirstItem = firstItem;
            data.SecondItem = secondItem;
        }

        public SwapType GetTypeSwap(TypeItems first, TypeItems second)
        {
            if (_swapTypes.TryGetValue((first, second), out var tRes))
                return tRes;

            if (_swapTypes.TryGetValue((second, first), out var sRes))
                return sRes;

            return SwapType.NoSwap;
        }

        public BonusSwapType GetTypeBonusSwap(TypeItems first, TypeItems second)
        {
            if (_bonusSwapTypes.TryGetValue((first, second), out var tRes))
                return tRes;

            if (_bonusSwapTypes.TryGetValue((second, first), out var sRes))
                return sRes;

            return BonusSwapType.None;
        }

        private void CreateSwapTypes()
        {
            _swapTypes = new Dictionary<(TypeItems, TypeItems), SwapType>
            {
                { (TypeItems.Chip, TypeItems.Chip), SwapType.Simple },
                { (TypeItems.Chip, TypeItems.Dice_1), SwapType.Simple },
                { (TypeItems.Chip, TypeItems.Dice_2), SwapType.Simple },
                { (TypeItems.Chip, TypeItems.Dice_3), SwapType.Simple },
                { (TypeItems.Bomb, TypeItems.Chip), SwapType.Simple },
                { (TypeItems.HorizontalBomb, TypeItems.Chip), SwapType.Simple },
                { (TypeItems.VerticalBomb, TypeItems.Chip), SwapType.Simple },
                { (TypeItems.FlyBomb, TypeItems.Chip), SwapType.Simple}
            };
        }
        private void CreateBonusSwapTypes()
        {
            _bonusSwapTypes = new Dictionary<(TypeItems, TypeItems), BonusSwapType>
            {
                { (TypeItems.ColorBomb, TypeItems.Chip), BonusSwapType.ColorBombVsChip },
                { (TypeItems.ColorBomb, TypeItems.Bomb), BonusSwapType.ColorBombVsLittleBomb },
                { (TypeItems.ColorBomb, TypeItems.HorizontalBomb), BonusSwapType.ColorBombVsStripedBomb },
                { (TypeItems.ColorBomb, TypeItems.VerticalBomb), BonusSwapType.ColorBombVsStripedBomb },
                { (TypeItems.ColorBomb, TypeItems.ColorBomb), BonusSwapType.ColorBombWithColorBomb },
                { (TypeItems.VerticalBomb, TypeItems.HorizontalBomb), BonusSwapType.StripedWithStriped },
                { (TypeItems.VerticalBomb, TypeItems.VerticalBomb), BonusSwapType.StripedWithStriped },
                { (TypeItems.HorizontalBomb, TypeItems.VerticalBomb), BonusSwapType.StripedWithStriped },
                { (TypeItems.HorizontalBomb, TypeItems.HorizontalBomb), BonusSwapType.StripedWithStriped },
                { (TypeItems.Bomb, TypeItems.Bomb), BonusSwapType.LittleBombWithLittleBomb },
            };
        }
    }
}
