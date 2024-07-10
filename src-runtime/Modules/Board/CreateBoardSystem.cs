using System.Collections.Generic;
using Client.Game.Utils;
using Leopotam.EcsLite;
using MatchThree.Rack.Common;

namespace MatchThree.Rack.ECS.Systems
{
    public class CreateBoardSystem : IEcsRunSystem
    {
        private IBoardFacade _board;
        private readonly OnCreateRack _createRack;
        private List<IItem> _items;

        public CreateBoardSystem(IBoardFacade board, OnCreateRack createRack)
        {
            _items = new List<IItem>();
            _board = board;
            _createRack = createRack;
        }

        public void Run(IEcsSystems systems)
        {
            CreateChips();
            ExecuteCallback();
        }

        private void ExecuteCallback()
        {
            foreach (var slot in _board.Slots)
                foreach (var item in slot.Items)
                    _items.Add(item);

            _createRack?.Invoke(_items);
        }

        private void CreateChips()
        {
            //шахматное заполнените

            int[] exludeColors = new int[3] { -1, -1, -1 };
            var typeCreatedItem = TypeItems.Chip;

            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    var posItem = new Int2(x, y);

                    if (_board.IsFreeSlot(x, y) && _board.IsEmptySlot(x, y) == false)
                    {

                        var randColor = GetRandColor(posItem, exludeColors);
                        _board.CreateItem(posItem, randColor, typeCreatedItem);
                    }
                }
            }
        }

        private int GetRandColor(Int2 posItem, int[] exludeColors)
        {
            var downPos = posItem + new Int2(0, -1);
            //смотрим вверз
            if (_board.TryGetLastItem(downPos, out var downItem))
                exludeColors[0] = downItem.Color;

            //хак
            //Смотрим влево
            var leftPos = posItem + new Int2(-1, 0);
            if (_board.TryGetLastItem(leftPos, out var leftItem))
                exludeColors[1] = leftItem.Color;

            var randColor = MyRandom.GetRandomColor(6, exludeColors[0], exludeColors[1]);
            return randColor;
        }
    }
}
