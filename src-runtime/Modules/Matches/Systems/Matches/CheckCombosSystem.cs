using System.Collections.Generic;
using MatchThree.Rack;
using MatchThree.Rack.ECS;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using MatchThree.Rack.CoreECS.Tags;

namespace MatchThree.Rack.ECS.Systems
{
    delegate void CreateBonusForCheckMatches(IItem bonus, int range);
    /// <summary>
    /// ѕока что провер€ем только на наличие бонусов, но если потребуетс€ еще дл€ каких либо проверок, то отсюда можно начать провер€ть комбы
    /// </summary>
    public class CheckCombosSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<Items, CheckBonusesTag>> f_items = default; // ¬се Items которые попали в матч попадютс€ сюда

        private readonly EcsPoolInject<MatchedTag> p_matchedTag = default;

        private readonly CreateBonusForCheckMatches createBonusForCheckMatches = ECSStaticUtils.CreateMatchesBonus;

        private HashSet<IItem> findedBonuses;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            findedBonuses = new HashSet<IItem>();
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            FindBonusInCombos();
        }

        private void FindBonusInCombos()
        {
            foreach (var entity in f_items.Value)
            {
                FindBonusesInCombo(entity);
                ClearCheckComboTag(entity);
                SetComboMatchedTag(entity);
            }
        }

        private void FindBonusesInCombo(int _Enty)
        {
            ref Items itemsData = ref f_items.Pools.Inc1.Get(_Enty);

            foreach (var item in itemsData.MatchedItems)
            {
                if (item.IsBonus)
                {
                    var bonusItem = item;
                    CreateMatchesBonus(bonusItem); //≈с*/ли находим некоторый бонус, то мы создаем новую комбинацию в которой есть этот бонус и мы должны ее взорвать.
                    findedBonuses.Add(item);
                    continue;
                }

                item.IsMatched = true;
                //if (item.IsMatched)
                //continue;
            }

            if (findedBonuses.Count == 0)
                return;

            foreach (var item in findedBonuses)
                itemsData.MatchedItems.Remove(item);

            findedBonuses.Clear();
        }

        private void CreateMatchesBonus(IItem bonus)
        {
            MyLog.Log("System/Green", $"Find Bonus in Combo and Create Bonus {bonus.Type} {bonus.Position}", GetType());
            createBonusForCheckMatches(bonus, 1);
        }

        private void SetComboMatchedTag(int _Enty)
        {
            if (p_matchedTag.Value.Has(_Enty) == false)
            {
                p_matchedTag.Value.Add(_Enty);

                //ѕомечаем Items Matched

            }
        }

        private void ClearCheckComboTag(int _Enty)
        {
            f_items.Pools.Inc2.Del(_Enty);
        }
    }
}
