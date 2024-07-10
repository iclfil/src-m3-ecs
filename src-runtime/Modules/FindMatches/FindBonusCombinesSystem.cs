using System.Collections.Generic;
using MatchThree.Rack;
using MatchThree.Rack.ECS;
using Client.Game.Utils;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common.Services;
using MatchThree.Rack.CoreECS.Tags;

namespace MatchThree.Rack.ECS.Systems
{
    /// <summary>
    /// Ищет комбинации для Спавна Бонусов
    /// </summary>
    public class FindBonusCombinesSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<SimpleCombines>> __simpleCombines = default;
        private readonly EcsCustomInject<FindMatchesService> _matchesServicePool = default;

        private FindMatchesService _findMatchesService;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _findMatchesService = _matchesServicePool.Value;
        }
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in __simpleCombines.Value)
            {
                ref SimpleCombines simpleCombines = ref __simpleCombines.Pools.Inc1.Get(entity);

                if (FindComplexBonusCombo(ref simpleCombines) == false)
                    FindSimpleBonusCombo(ref simpleCombines);
            }
        }
        private void FindSimpleBonusCombo(ref SimpleCombines simpleCombines)
        {
            //List<MyCombo> combos = simpleCombines.Combos.FindAll(x =>
            //    x.Type() != ComboType.ThreeHorizontal && x.Type() != ComboType.ThreeVertical);

            //if (combos.Count == 0)
            //    return;

            //foreach (var combo in combos)
            //{
            //    CreateCombo(combo);
            //    MyLog.Log("System/Green", $"Create Simple Bonus Combo Type {combo.Type()}", GetType());
            //    simpleCombines.Combos.Remove(combo);
            //}
        }
        private bool FindComplexBonusCombo(ref SimpleCombines simpleCombines)
        {
           // List<MyComplexCombo> combos = _matchesService.ComplexCombos(simpleCombines.Combos);

            //if (combos.Count == 0)
            //    return false;

            //foreach (var combo in combos)
            //{
            //    CreateCombo(combo);
            //    simpleCombines.Combos.Remove(combo.FirstMyCombo);
            //    simpleCombines.Combos.Remove(combo.SecondMyCombo);
            //    MyLog.Log("System/Green", $"Create Complex Bonus Combo Type {combo.Type()}", GetType());
            //}

            return true;
        }
        private void CreateCombo(MyCombo myCombo)
        {
            var comboType = myCombo.Type();
            var color = myCombo.Color;
            var key = myCombo.Key;
            var items = new List<IItem>(myCombo.Items);

            var entity = _world.NewEntity();
            var pool1 = _world.GetPool<ComboInfo>();
            ref var comboData = ref pool1.Add(entity);
            comboData.Color = color;
            comboData.ComboType = comboType;
            comboData.Key = key;

            var pool2 = _world.GetPool<ECS.Items>();
            ref ECS.Items itemsData = ref pool2.Add(entity);
            itemsData.MatchedItems = new List<IItem>(items);

            var pool3 = _world.GetPool<SpawnBonus>();
            ref SpawnBonus s = ref pool3.Add(entity);
            s.Color = color;
            s.Position = GetRandomPos(items);
            s.Type = ECSStaticUtils.GetTypeBonusItemByComboType(comboType);

            var pool4 = _world.GetPool<CheckBonusesTag>();
            pool4.Add(entity);

            //   MyConsole.LogWithPrefix("Systems", "Create Combo" + comboType, GetType());

        }

        private Int2 GetRandomPos(List<IItem> items)
        {
            int rand = UnityEngine.Random.Range(0, items.Count);
            IItem randItem = items[rand];
            return randItem.Position;
        }
    }
}
