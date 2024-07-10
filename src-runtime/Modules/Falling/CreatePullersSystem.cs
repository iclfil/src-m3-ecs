using System.Collections.Generic;
using Client.Game.Utils;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS.Systems.Falling.Components;
using MatchThree.Rack.Utils;

namespace MatchThree.Rack.ECS.Systems.Falling
{
    /// <summary>
    /// Создает Puller во всех имеющихся и свободных Слотах.
    /// </summary>
    public class CreatePullersSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilterInject<Inc<FallBehavior>> f_fallingItems = "Board";
        private readonly IBoardFacade _board;
        private EcsWorld _world;
        private Dictionary<Int2, bool> _blockedPullers;//Пулеры в которые ничего не может упасть.
        private Dictionary<Int2, bool> _activatePullers;//Пулеры в которые точно упадут фишки

        //TODO - пулеру лучше указывать сразу позицию от куда он может тянуть

        public CreatePullersSystem(IBoardFacade board)
        {
            _board = board;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _blockedPullers = new Dictionary<Int2, bool>(_board.Width * _board.Height);
            _activatePullers = new Dictionary<Int2, bool>(_board.Width * _board.Height);
        }

        public void Run(IEcsSystems systems)
        {
            //Элементы не могут упасть. TODO - тут возникнет проблема с телепортом
            //if (f_fallingItems.Value.GetEntitiesCount() == 0){}
                //return;


            _blockedPullers.Clear();
            _activatePullers.Clear();
            bool createPuller = false;

            //Усложнить логику создания Пуллеров.
            //чтобы они еще на этапе создания, было понятно, где может упасть объект а где не может.
            //И тогда можно делать выход из стейта на основе созданных пуллеров, если они есть, значит должно падать.
            //Пулер создаем когда:

            //Есть свободный слот.+
            //Над слотом есть позиция в пределах доски. +
            //В слоте есть телепорт, который может сработать. ? Как узнать быстро, что так клетка может сработать.
            //Над пулером есть объект, который имеет FallBehavior+
            //Не должны создаваться в слотах со спавном+
            //Если мы в свободном слоту и над нами есть спавн

            for (int x = 0; x < _board.Width; x++)
            {
                if (createPuller == true)
                    continue;

                for (int y = _board.Height - 1; y >= 0; y--)
                {
                    var pullerPos = new Int2(x, y);

                    //Если слот не пустой
                    if (_board.IsFreeSlot(x, y) == false)
                        continue;


                    var conditon = new CreatePullerCondition(pullerPos, _board);

                    if (conditon.FirstCondition() == false)
                        continue;

                    if (conditon.SecondCondition())
                    {
                        createPuller = true;
                        CreatePuller(pullerPos, PullerDirection.Up);
                        continue;
                    }

                    //Проверяем только если сверху у нас над пулером есть твердое препятсиве или же пустая клетка

                    if (conditon.HasSolidBlockOrEmptySlot() || _blockedPullers.ContainsKey(pullerPos + new Int2(0, 1)))
                    {
                        _blockedPullers.Add(pullerPos, true);

                        if (conditon.LeftThreeCondition())
                        {
                            CreatePuller(pullerPos, PullerDirection.Left);
                            continue;
                        }

                        if (conditon.RightThreeCondition())
                        {
                            CreatePuller(pullerPos, PullerDirection.Right);
                            continue;
                        }
                    }

                    //Проверка на создание пулера. 
                    //Создание пулера.



                    //  //проверка на телепорт//Может ли сюда упасть фишка с телепорта
                    //if (_board.HasTeleportSourceInSlot(pullerPos))
                    //  {
                    //      var sourceTeleport = _board.GetTeleportSourceFromSlot(pullerPos);
                    //      MyLog.Log("System/Green", $"Source teleport{sourceTeleport} destination Teleport {pullerPos}", GetType());
                    //      if (_board.TryGetLastItem(sourceTeleport, out var sourceItem))
                    //      {
                    //          if (ItemsProperties.IsBlocksFallingDown(sourceItem.Type) == false)
                    //          {
                    //              if (_board.HasBehavior<FallBehavior>(sourceItem))
                    //              {
                    //                  CreatePuller(sourceTeleport, PullerDirection.Up);
                    //                  CreateTeleportPuller(pullerPos, sourceTeleport);
                    //                  MyLog.Log("System/Green", $"Create Teleport Puler Source {sourceTeleport} Destination {pullerPos}", GetType());
                    //              }
                    //          }
                    //      }
                    //  }






                    ////Если у нас пустая клетка сверху, то проверим на возможность тянуть слева и справа
                    //if (_board.IsEmptySlot(upperPos.X, upperPos.Y))
                    //{
                    //    var leftPos = pullerPos + new Int2(-1, 1);
                    //    if (CheckAndCreateDirectionPuller(leftPos, pullerPos, PullerDirection.Left))
                    //        continue;

                    //    var rightPos = pullerPos + new Int2(1, 1);
                    //    if (CheckAndCreateDirectionPuller(rightPos, pullerPos, PullerDirection.Right))
                    //        continue;

                    //    MyLog.Log("System/Green", $"Puller {pullerPos} Blocked Is EmptySlot in {upperPos} ", GetType());
                    //    //Мы не смогли создать пулер, который бы тянул и сверху и справа и слева. 
                    //    _blockedPullers.Add(pullerPos, true);
                    //}
                }
            }



            //Создаем пулеры сверху вниз, чтобы проверка фишек была с верхней позиции поля
            //for (int x = 0; x < _board.Width; x++)
            //{
            //    for (int y = _board.Height - 1; y >= 0; y--)
            //    {
            //        if (_board.IsEmptySlot(x, y))
            //            continue;

            //        if (_board.IsSpawnSlot(x, y))
            //            continue;

            //        if (_board.IsFreeSlot(x, y) == false)
            //            continue;

            //        var pullerPos = new Int2(x, y);

            //        //Проверка на создание пулера. 
            //        //Создание пулера.



            //        //проверка на телепорт//Может ли сюда упасть фишка с телепорта
            //        if (_board.HasTeleportSourceInSlot(pullerPos))
            //        {
            //            var sourceTeleport = _board.GetTeleportSourceFromSlot(pullerPos);
            //            MyLog.Log("System/Green", $"Source teleport{sourceTeleport} destination Teleport {pullerPos}", GetType());
            //            if (_board.TryGetLastItem(sourceTeleport, out var sourceItem))
            //            {
            //                if (ItemsProperties.IsBlocksFallingDown(sourceItem.Type) == false)
            //                {
            //                    if (_board.HasBehavior<FallBehavior>(sourceItem))
            //                    {
            //                        CreatePuller(sourceTeleport, PullerDirection.Up);
            //                        CreateTeleportPuller(pullerPos, sourceTeleport);
            //                        MyLog.Log("System/Green", $"Create Teleport Puler Source {sourceTeleport} Destination {pullerPos}", GetType());
            //                    }
            //                }
            //            }
            //        }


            //        //Создаем пуллер для тяги сверху
            //        //Не проверяем сверху, если там есть твердый элемент.
            //        var upperPos = pullerPos + new Int2(0, 1);

            //        if (_board.TryGetLastItem(upperPos, out var upperItem))
            //        {
            //            //Если у нас сверху не заблокировано, тогда можем проверить 
            //            if (ItemsProperties.IsBlocksFallingDown(upperItem.Type) == false)
            //            {
            //                if (CheckAndCreatePuller(upperPos, pullerPos, PullerDirection.Up))
            //                    break;
            //            }

            //            //Пулеры для Тяги со сторон, создаются только если падениею в клетку перекрывает твердый предмет//Нужно еще проверить блокиратор выше на клетку

            //            //Если пулер заблокирован, то мы должны проверить под ним, можем ли мы чего тянуть
            //            if (_blockedPullers.ContainsKey(upperPos))
            //            {
            //                MyLog.Log("System/Green", $"Blocked Puller {upperPos} check Left and Right", GetType());
            //                var leftPos = pullerPos + new Int2(-1, 1);
            //                if (CheckAndCreateDirectionPuller(leftPos, pullerPos, PullerDirection.Left))
            //                    continue;

            //                var rightPos = pullerPos + new Int2(1, 1);
            //                if (CheckAndCreateDirectionPuller(rightPos, pullerPos, PullerDirection.Right))
            //                    continue;

            //                continue;
            //            }

            //            //Если сверху блокиратор или пустой слот просто
            //            if (ItemsProperties.IsBlocksFallingDown(upperItem.Type))
            //            {
            //                MyLog.Log("System/Green", $"Has Solid Item {upperPos} check Left and Right", GetType());

            //                var leftPos = pullerPos + new Int2(-1, 1);
            //                if (CheckAndCreateDirectionPuller(leftPos, pullerPos, PullerDirection.Left))
            //                    continue;

            //                var rightPos = pullerPos + new Int2(1, 1);
            //                if (CheckAndCreateDirectionPuller(rightPos, pullerPos, PullerDirection.Right))
            //                    continue;

            //                MyLog.Log("System/Green", $"Puller Blocked {pullerPos}", GetType());
            //                //Мы не смогли создать пулер, который бы тянул и сверху и справа и слева. 
            //                _blockedPullers.Add(pullerPos, true);
            //            }
            //        }


            //        //Если у нас пустая клетка сверху, то проверим на возможность тянуть слева и справа
            //        if (_board.IsEmptySlot(upperPos.X, upperPos.Y))
            //        {
            //            var leftPos = pullerPos + new Int2(-1, 1);
            //            if (CheckAndCreateDirectionPuller(leftPos, pullerPos, PullerDirection.Left))
            //                continue;

            //            var rightPos = pullerPos + new Int2(1, 1);
            //            if (CheckAndCreateDirectionPuller(rightPos, pullerPos, PullerDirection.Right))
            //                continue;

            //            MyLog.Log("System/Green", $"Puller {pullerPos} Blocked Is EmptySlot in {upperPos} ", GetType());
            //            //Мы не смогли создать пулер, который бы тянул и сверху и справа и слева. 
            //            _blockedPullers.Add(pullerPos, true);
            //        }
            //    }
            //}
        }

        private bool CheckAndCreatePuller(Int2 findPos, Int2 pullerPos, PullerDirection direction)
        {
            if (_board.HasPosition(findPos) == false)
                return false;

            if (_board.IsFreeSlot(pullerPos.X, pullerPos.Y) && _board.IsSpawnSlot(findPos.X, findPos.Y))
            {
                CreatePuller(pullerPos, direction);
                return true;
            }

            //Не создаем пулеры под пуларми
            ////Перед тем как проверить есть ли там предмет который можно уронить, мы провери на наличие активного пулера, потому что если он там есть
            ////значит и предмет на следующем шаге там будет.
            if (_activatePullers.ContainsKey(findPos))
            {
                CreatePuller(pullerPos, direction);
                return true;
            }

            if (_board.TryGetLastItem(findPos, out var underItem))
            {
                if (_board.HasBehavior<FallBehavior>(underItem) == false)
                    return false;
            }

            CreatePuller(pullerPos, direction);
            return true;
        }

        private bool CheckAndCreateDirectionPuller(Int2 findPos, Int2 pullerPos, PullerDirection direction)
        {
            if (_board.HasPosition(findPos) == false)
                return false;

            //Не создаем пуллеры
            if (direction == PullerDirection.Left || direction == PullerDirection.Right)
            {
                if (_board.IsFreeSlot(findPos.X, findPos.Y))
                    return false;
            }

            if (_board.TryGetLastItem(findPos, out var underItem))
            {
                if (_board.HasBehavior<FallBehavior>(underItem) == false)
                    return false;
            }

            CreatePuller(pullerPos, direction);
            return true;
        }

        private void CreatePuller(Int2 pullerPos, PullerDirection direction)
        {
            //HACK
            if (_activatePullers.ContainsKey(pullerPos))
                return;

            _activatePullers.Add(pullerPos, true);
            var entity = _world.NewEntity();
            var pool = _world.GetPool<Puller>();
            ref var dataPuller = ref pool.Add(entity);
            dataPuller.Direction = direction;
            dataPuller.PositionPuller = pullerPos;
            MyLog.Log("System/Green", $"Create Puller  in {pullerPos}", GetType());
        }

        private void CreateTeleportPuller(Int2 pullerPos, Int2 sourcePosition)
        {
            _activatePullers.Add(pullerPos, true);
            var entity = _world.NewEntity();
            var pool = _world.GetPool<TeleportPuller>();
            ref var dataPuller = ref pool.Add(entity);
            dataPuller.PositionPuller = pullerPos;
            dataPuller.SourceTeleportPosition = sourcePosition;
            MyLog.Log("System/Green", $"Create Teleport Puller  in {pullerPos}", GetType());
        }
    }

    public class CreatePullerCondition
    {
        private readonly Int2 _pullerPos;
        private readonly IBoardFacade _board;


        public CreatePullerCondition(Int2 pullerPos, IBoardFacade board)
        {
            _pullerPos = pullerPos;
            _board = board;
        }

        public bool FirstCondition()
        {
            var x = _pullerPos.X;
            var y = _pullerPos.Y;

            if (_board.IsEmptySlot(x, y))
                return false;

            if (_board.IsSpawnSlot(x, y))
                return false;

            if (_board.IsFreeSlot(x, y) == false)
                return false;

            return true;
        }

        public bool HasSolidBlockOrEmptySlot()
        {
            var pullerPos = _pullerPos + new Int2(0, 1);

            //Смотрим просто выше
            if (_board.HasPosition(pullerPos) == false)
                return false;

            if (_board.TryGetLastItem(pullerPos, out var upperItem))
            {
                if (ItemsProperties.IsBlocksFallingDown(upperItem.Type))
                    return true;
            }

            return false;
        }

        public bool SecondCondition()
        {
            var upperPos = _pullerPos + new Int2(0, 1);


            if (_board.HasPosition(upperPos) == false)
                return false;

            //Если сверху пустой спавн слот, а под ним есть уже итем.


            if (_board.IsFreeSlot(upperPos.X, upperPos.Y) && _board.IsSpawnSlot(upperPos.X, upperPos.Y)) //Если сверху спавн слот, то мы точно должны создать пуллер, потому что там появится предмет
                return true;

            if (_board.IsEmptySlot(upperPos.X, upperPos.Y))
                return false;


            if (_board.TryGetLastItem(upperPos, out var upperItem))
            {
                if (ItemsProperties.IsBlocksFallingDown(upperItem.Type))
                    return false;

                if (_board.HasBehavior<FallBehavior>(upperItem) == false)
                    return false;
            }

            return true;

            //if (_board.TryGetLastItem(upperPos, out var upperItem))
            //{
            //    //Если у нас сверху не заблокировано, тогда можем проверить 
            //    if (ItemsProperties.IsBlocksFallingDown(upperItem.Type) == false)
            //    {
            //        if (CheckAndCreatePuller(upperPos, pullerPos, PullerDirection.Up))
            //            break;
            //    }

            //    //Пулеры для Тяги со сторон, создаются только если падениею в клетку перекрывает твердый предмет//Нужно еще проверить блокиратор выше на клетку

            //    //Если пулер заблокирован, то мы должны проверить под ним, можем ли мы чего тянуть
            //    if (_blockedPullers.ContainsKey(upperPos))
            //    {
            //        MyLog.Log("System/Green", $"Blocked Puller {upperPos} check Left and Right", GetType());
            //        var leftPos = pullerPos + new Int2(-1, 1);
            //        if (CheckAndCreateDirectionPuller(leftPos, pullerPos, PullerDirection.Left))
            //            continue;

            //        var rightPos = pullerPos + new Int2(1, 1);
            //        if (CheckAndCreateDirectionPuller(rightPos, pullerPos, PullerDirection.Right))
            //            continue;

            //        continue;
            //    }

            //    //Если сверху блокиратор или пустой слот просто
            //    if (ItemsProperties.IsBlocksFallingDown(upperItem.Type))
            //    {
            //        MyLog.Log("System/Green", $"Has Solid Item {upperPos} check Left and Right", GetType());

            //        var leftPos = pullerPos + new Int2(-1, 1);
            //        if (CheckAndCreateDirectionPuller(leftPos, pullerPos, PullerDirection.Left))
            //            continue;

            //        var rightPos = pullerPos + new Int2(1, 1);
            //        if (CheckAndCreateDirectionPuller(rightPos, pullerPos, PullerDirection.Right))
            //            continue;

            //        MyLog.Log("System/Green", $"Puller Blocked {pullerPos}", GetType());
            //        //Мы не смогли создать пулер, который бы тянул и сверху и справа и слева. 
            //        _blockedPullers.Add(pullerPos, true);
            //    }
            //}

        }

        public bool LeftThreeCondition()
        {
            var leftPos = _pullerPos + new Int2(-1, 1);

            if (_board.HasPosition(leftPos) == false)
                return false;

            if (_board.IsFreeSlot(leftPos.X, leftPos.Y) && _board.IsSpawnSlot(leftPos.X, leftPos.Y)) //Если сверху спавн слот, то мы точно должны создать пуллер, потому что там появится предмет
                return true;

            if (_board.TryGetLastItem(leftPos, out var upperItem))
            {
                if (_board.HasBehavior<FallBehavior>(upperItem) == false)
                    return false;
            }

            return true;
        }

        public bool RightThreeCondition()
        {
            var rightPos = _pullerPos + new Int2(1, 1);

            if (_board.HasPosition(rightPos) == false)
                return false;

            if (_board.IsFreeSlot(rightPos.X, rightPos.Y) && _board.IsSpawnSlot(rightPos.X, rightPos.Y)) //Если сверху спавн слот, то мы точно должны создать пуллер, потому что там появится предмет
                return true;

            if (_board.TryGetLastItem(rightPos, out var upperItem))
            {
                if (_board.HasBehavior<FallBehavior>(upperItem) == false)
                    return false;
            }

            return true;
        }
    }
}