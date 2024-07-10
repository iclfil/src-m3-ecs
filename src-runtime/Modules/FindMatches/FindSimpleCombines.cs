using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack;
using MatchThree.Rack.Common.Services;
using MatchThree.Rack.ECS;
using MatchThree.Rack.ECS.SharedDatas;

namespace MatchThree.ECS.Modules.FindMatches
{
    //Ищет простые комбинации в множествах.
    internal class FindSimpleCombines : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ColoredItems>> f_coloredItems = default;

        private readonly EcsPoolInject<SimpleCombines> _pool1 = default;
        private readonly EcsCustomInject<FindMatchesService> _matchesServicePool = default;

        private FindMatchesService _findMatchesService;

        private EcsWorld _world;
        private List<RCombo> _findCombines;
        private List<IItem> _items;

        private SharedData _sharedData;

        public void Init(IEcsSystems systems)
        {
            _sharedData = systems.GetShared<SharedData>();
            _findCombines = new List<RCombo>();
            _world = systems.GetWorld();
            _findMatchesService = _matchesServicePool.Value;
        }

        public void Run(IEcsSystems systems)
        {
            _sharedData.MatchesCombos = new List<RCombo>();
            _findCombines.Clear();

            foreach (var entity in f_coloredItems.Value)
            {
                var color = f_coloredItems.Pools.Inc1.Get(entity).Color;
                _items = f_coloredItems.Pools.Inc1.Get(entity).Items;

                if (HasSimpleCombines(_items, color, out _findCombines))
                {
                    foreach (var combo in _findCombines)
                        _sharedData.MatchesCombos.Add(combo);
                }

                _world.DelEntity(entity); ;
            }
        }
        private bool HasSimpleCombines(List<IItem> items, int color, out List<RCombo> allCombos)
        {
            var combos = _findMatchesService.FindAllCombos(items, color);
            allCombos = combos;
            return combos.Count > 0;
        }
        //private void CreateCombo(RCombo combo)
        //{
        //    if (combo.Type != ComboType.SimpleMatch)
        //    {
        //        MyLog.Log("System/Green", $"Create Bonus Combo  Color: {combo.Color}  Items {combo.Items.Count}", GetType());
        //        ECSStaticUtils.CreateBonusCombo(combo.Type, combo.Color, combo.Key, combo.Items);
        //        return;
        //    }

        //    MyLog.Log("System/Green", $"Create Simple Combo  Color: {combo.Color}  Items {combo.Items.Count}", GetType());

        //    //ПЕревод обратно
        //    ECSStaticUtils.CreateSimpleCombo(combo.Type, combo.Color, combo.Key, combo.Items);
        //}
    }

    //private void CreateSimpleCombines(List<RCombo> combos, int color)
    //{
    //    int entity = _world.NewEntity();
    //    ref SimpleCombines data = ref _pool1.Value.Add(entity);
    //    data.Color = color;
    //    data.Combos = new List<RCombo>(_findCombines);
    //    MyLog.Log("System/Green", $"Create Simple Combine  Color: {data.Color}  Combos {data.Combos.Count}", GetType());
    //}
}