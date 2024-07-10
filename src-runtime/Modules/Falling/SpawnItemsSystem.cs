using System.Collections.Generic;
using Client.Game.Utils;
using FilConsole;
using Leopotam.EcsLite;
using MatchThree.Rack.Common;

namespace MatchThree.Rack.ECS.Systems.Falling
{
    public class SpawnItemsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IBoardFacade _board;
        private readonly OnCollapseItems _collapseItems;
        private readonly OnCollapseCreatingItems _creatingItems;
        private List<IItem> _items;

        public SpawnItemsSystem(IBoardFacade board, OnCollapseItems collapseItems)
        {
            _items = new List<IItem>();
            _board = board;
            _collapseItems = collapseItems;
        }

        public SpawnItemsSystem(IBoardFacade board, OnCollapseCreatingItems creatingItems)
        {
            _items = new List<IItem>();
            _board = board;
            _creatingItems = creatingItems;
        }

        public void Init(IEcsSystems systems)
        {
        }

        public void Run(IEcsSystems systems)
        {
            _items.Clear();
            SpawnItems();

            if (_items.Count > 0)
            {
                _creatingItems?.Invoke(_items);
            }
        }

        private void SpawnItems()
        {
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = _board.Height - 1; y >= 0; y--)
                {
                    if (_board.IsFreeSlot(x, y) && _board.IsSpawnSlot(x, y))
                    {
                        var position = new Int2(x, y);
                        var color = UnityEngine.Random.Range(1,
                            7); // TODO - фишки должны браться из сервиса, который распределяет цвета на поле.
                        var type = TypeItems.Chip;
                        MyLog.Log("Systems/Green", $"Spawn Item {color} in {position}", GetType());
                        var item = _board.CreateItem(position, color, type);
                       // _collapseItems?.Invoke(item, (null, Int2.Empty), "asdasd");
                        _items.Add(item);
                    }
                }
            }
        }
    }
}