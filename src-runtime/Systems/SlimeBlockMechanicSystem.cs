using System.Linq;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.SharedDatas;
using MatchThree.Rack.ECS.Tags;
using MatchThree.Rack.Utils;

namespace MatchThree.Rack.ECS.Systems
{
    public class SlimeBlockMechanicSystem : IEcsRunSystem
    {
        private readonly IBoardFacade _board;
        private readonly OnMechanicActivate mechanic;
        private readonly EcsFilterInject<Inc<SlimeTag, ItemData>> f_slimes = "Board";
        private readonly EcsFilterInject<Inc<SlimeTag, DestroyTag>> f_destroySlimes = "Board";

        public SlimeBlockMechanicSystem(IBoardFacade board, OnMechanicActivate mechanic)
        {
            _board = board;
            this.mechanic = mechanic;
        }

        public void Run(IEcsSystems systems)
        {
            if (f_slimes.Value.GetEntitiesCount() == 0)
                return;

            //Если есть хотя бы один слайм, что будет удален
            if (f_destroySlimes.Value.GetEntitiesCount() >= 1)
                return;

            //если таким слаймов нет, то срабатывает механика
            foreach (var entity in f_slimes.Value)
            {
                var slime = f_slimes.Pools.Inc2.Get(entity).Item;

                if (ItemsHandlerUtils.TryGetPositions(slime, out var foundItems, _board))
                {
                    var chip = foundItems.FirstOrDefault(x => x.Type == TypeItems.Chip && x.IsMatched == false);

                    if(chip == null)
                        continue;
                   
                    
                    var pos = chip.Position;
                    _board.DeleteIncludeItem(chip);
                    var createdSlime = _board.CreateItem(pos, slime.Color, slime.Type);
                    mechanic?.Invoke(TypeMechanic.SlimeActivated, chip, createdSlime);
                    return;
                }
            }

            //if (systems.GetShared<SharedData>().SlimeActivate)
            //{
            //    systems.GetShared<SharedData>().SlimeActivate = false;


            //    foreach (var e_slime in _slimes.Value)
            //    {
            //        var itemSlime = _slimes.Pools.Inc2.Get(e_slime).Item;

            //        if (ItemsHandlerUtils.TryGetPositions(itemSlime, out var findedItem, _board))
            //        {
            //            var chip = findedItem.ToList().Find(x => x.Type == TypeItems.Chip);

            //            if (chip == null)
            //                continue;

            //            var pos = chip.Position;

            //            //Удаляем фишку и создаем слайм
            //            _board.DeleteIncludeItem(chip);

            //            MyLog.Log("System/Green", $"Slime Delete Item {chip.Type} {chip.Position}", GetType());

            //          //  _deleteItem?.Invoke(new ItemViewProvider(chip.GetID(), pos, chip.Color, chip.Type), "SlimeActivated");

            //            var slime = _board.CreateItem(pos, itemSlime.Color, TypeItems.Slime);
            //          //  _createItem?.Invoke(new ItemViewProvider(slime.GetID(), pos, slime.Color, slime.Type), "SlimeActivated");

            //            MyLog.Log("System/Green", $"Slime Activated {slime.Type} {slime.Position}", GetType());

            //            return;
            //        }
            //    }
            //}
        }
    }
}