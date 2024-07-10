using System.Collections.Generic;
using MatchThree.Rack;
using MatchThree.Rack.ECS;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.CoreECS.Tags;
#if UNITY_EDITOR

#endif
using UnityEngine;

namespace MatchThree.Rack.ECS.Systems
{
    /// <summary>
    /// Data => Combo, CheckBonusComboTag
    /// </summary>
    public class CreateSimpleBonusCombinesSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<SimpleCombines, CheckBonusesTag>> __combinesFilter = default;

        private readonly EcsPoolInject<ComboInfo> _combo = default;
        private readonly EcsPoolInject<FindTypeBonusInComboTag> _bonus = default;

        private EcsWorld _world;
        private HashSet<MyCombo> findedCombos;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var _Enty in __combinesFilter.Value)
            {
                // MyConsole.LogWithPrefix("Systems", "Find Simple Bonuses Combines", GetType());
                ref SimpleCombines combines = ref __combinesFilter.Pools.Inc1.Get(_Enty);
                findedCombos = new HashSet<MyCombo>();

                foreach (var combo in combines.Combos)
                {
                    //  MyConsole.Log("COMBO TYPE" + combo.Type(), Color.magenta);
                    //if (combo.Type() != ComboType.ThreeHorizontal && combo.Type() != ComboType.ThreeVertical)
                    //{
                    //    findedCombos.Add(combo);
                    //    CreateSimpleBonusCombo(combo, combines);
                    //}
                }

                //foreach (var c in findedCombos)
                //{
                //    combines.Combos.Remove(c);
                //}

                Clear_CheckBonusesTag(_Enty);
            }
        }

        private void Clear_CheckBonusesTag(int _Enty)
        {
            __combinesFilter.Pools.Inc2.Del(_Enty);
        }

        private void CreateSimpleBonusCombo(MyCombo combo, SimpleCombines simpleCombines)
        {
            var entity = _world.NewEntity();
            ref ComboInfo comboInfoData = ref _combo.Value.Add(entity);
            comboInfoData.Color = combo.Color;
            comboInfoData.ComboType = combo.Type();
            comboInfoData.Key = combo.Key;

            _bonus.Value.Add(entity);

            //  ECSStaticUtils.CreateCheckedCombo<BonusComboTag>(myCombo);
            //    MyConsole.LogWithPrefix("Systems", "Create Simple Bonus Combo" + myCombo.Type(), GetType());
        }


    }
}