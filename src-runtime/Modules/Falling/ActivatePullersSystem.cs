using System.Collections.Generic;
using Client.Game.Utils;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.SharedDatas;
using MatchThree.Rack.ECS.Systems.Falling.Components;

namespace MatchThree.Rack.ECS.Systems
{
    /// <summary>
    /// Берет каждый пуллер и двигает из него итем в позицию пуллера
    /// </summary>
    public class ActivatePullersSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Puller>> _pullers = default;
        private readonly EcsFilterInject<Inc<TeleportPuller>> _teleportPullers = default;
        private EcsWorld _world;
        private readonly IBoardFacade _board;

        public ActivatePullersSystem(IBoardFacade board)
        {
            _board = board;
        }

        private List<PullItem> _items;
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _items = systems.GetShared<SharedData>().PullerItems;
        }

        public void Run(IEcsSystems systems)
        {
            MoveItems();
            SpawnItems();
        }

        private void MoveItems()
        {
            foreach (var e_tp_puller in _teleportPullers.Value)
            {
                var data = _teleportPullers.Pools.Inc1.Get(e_tp_puller);
                var item = data.DroppedItem;
                var targetPos = data.PositionPuller;
                _board.Move(item, targetPos);
                _world.DelEntity(e_tp_puller);
            }

            foreach (var e_puller in _pullers.Value)
            {
                var puller = _pullers.Pools.Inc1.Get(e_puller);
                var targetPos = puller.PositionPuller;
                var item = puller.DroppedItem;

                if (item == null) //Пулеры перебирают предметы в Handler и сюда приходит пустой элемент
                {
                    MyLog.Log("System/Red", $"Puller is Empty", GetType());
                    _world.DelEntity(e_puller);
                    continue;
                }

                MyLog.Log("System/Green", $"Move Item {item.Position} to {targetPos}", GetType());


                var pullItem = _items.Find(x => x.Id == item.GetID());

                if (pullItem.Item == null)
                {
                    var p = new PullItem()
                    {
                        Item = item,
                        Id = item.GetID(),
                        StartPos = item.Position,
                        AllPath = new HashSet<Int2> { targetPos },
                        IsLastSpawn =  false
                    };

                    _items.Add(p);
                }
                else
                {
                    pullItem.AllPath.Add(targetPos);
                    pullItem.IsLastSpawn = false;
                }

                _board.Move(item, targetPos);
                _world.DelEntity(e_puller);
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

                        _items.Add(new PullItem()
                        {
                            Item = item,
                            Id= item.GetID(),
                            IsSpawn = true,
                            IsLastSpawn = true, //Чтобы последний элемент двинуть на позицию во View
                            StartPos = position,
                            AllPath = new HashSet<Int2>()
                        });

                    }

                }
            }
        }
    }

    public struct PullItem
    {
        public int Id;
        public bool IsSpawn;
        public bool IsLastSpawn;
        public IItem Item;
        public Int2 StartPos;
        public Int2 FinishPos;
        public HashSet<Int2> AllPath;
    }
}