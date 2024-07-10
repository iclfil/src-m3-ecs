using System;
using Client.Game.Utils;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS.Systems.Falling;
using MatchThree.Rack.ECS.Systems.Falling.Components;
using MatchThree.Rack.ECS.SharedDatas;

namespace MatchThree.Rack.ECS.Systems
{
    /// <summary>
    /// Заполняет Пуллеры Items которые можно будет тянуть.
    /// </summary>
    public class PullersHandlerSystem : IEcsInitSystem, IEcsRunSystem
    {

        private readonly EcsFilterInject<Inc<Puller>> _pullers = default;
        private readonly EcsFilterInject<Inc<TeleportPuller>> _teleportPullers = default;

        private EcsWorld _world;
        private SharedData _sharedData;

        private readonly IBoardFacade _board;

        private bool _hasPull = false;

        public PullersHandlerSystem(IBoardFacade board)
        {
            _board = board;
        }

        public void Init(IEcsSystems systems)
        {
            _sharedData = systems.GetShared<SharedData>();
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var e_tp_puller in _teleportPullers.Value)
            {
                ref var data = ref _teleportPullers.Pools.Inc1.Get(e_tp_puller);

                var posPuller = data.PositionPuller;
                var sourcePuller = data.SourceTeleportPosition;

                if (_board.TryGetLastItem(sourcePuller, out var item))
                {
                    data.DroppedItem = item;
                }
            }

            //Проверка сверху всех элементов

            _hasPull = false;

            foreach (var e_puller in _pullers.Value)
            {
                var currentPos = _pullers.Pools.Inc1.Get(e_puller).PositionPuller;
                var findPos = currentPos + new Int2(0, 1);
                var typeDirectionPuller = _pullers.Pools.Inc1.Get(e_puller).Direction;

                if (typeDirectionPuller == PullerDirection.Up)
                {
                    _hasPull = TryPoolUpItem(findPos, e_puller);
                }

                if (_hasPull)
                    continue;

                if (typeDirectionPuller == PullerDirection.Left)
                {
                    findPos = currentPos + new Int2(-1, 1);
                    _hasPull = TryPoolUpItem(findPos, e_puller);
                }

                if (_hasPull)
                    continue;

                if (typeDirectionPuller == PullerDirection.Right)
                {
                    findPos = currentPos + new Int2(1, 1);
                    TryPoolUpItem(findPos, e_puller);
                }

                //if (_board.IsFreeSlot(upperPos.X, upperPos.Y))
                //    continue;

                //  _poolAbove = TryPoolUpItemFromTeleport(upperPos, currentPos);


                MyLog.Log("System/Green", $"Check Puller {currentPos} in {typeDirectionPuller} {findPos}", GetType());
            }
        }

        private bool TryPoolUpItemFromTeleport(Int2 upperPos, Int2 currentPos)
        {
            //Проверка на порталы, если сверху пустая клетка
            if (_board.IsEmptySlot(upperPos.X, upperPos.Y))
            {
                if (_board.HasTeleportInSlot(currentPos))
                {
                    //Взять у доски данные для телепорта
                    //и переместить фишку
                    throw new NotImplementedException("Teleport");
                    //TODO - должны дальше не тянуть.

                }
            }

            return false;
        }

        private bool TryPoolUpItem(Int2 upperPos, int e_puller)
        {
            //Не тянем когда: Нет FallBehavior, нет клетки вообще, просто пустая клется и Items нет.

            //Самая простая проверка. Если Item имеет FallBehavior, то мы его роняем.
            if (_board.TryGetLastItem(upperPos, out var item))
            {
                if (_board.HasBehavior<FallBehavior>(item))
                {
                    AddItemToPuller(e_puller, item);
                    return true;
                }
            }

            return false;
        }

        private void AddItemToPuller(int e_puller, IItem item)
        {
            //Сохраняем Item в Пуллер
            ref var dataPuller = ref _pullers.Pools.Inc1.Get(e_puller);
            dataPuller.DroppedItem = item;

            MyLog.Log("System/Green", $"Add item {item.Position} in Puller {dataPuller.PositionPuller}", GetType());
        }

        private void DeletePuller(int e_puller)
        {
            _world.DelEntity(e_puller);
        }

        private void CheckUpper()
        {

        }
    }
}