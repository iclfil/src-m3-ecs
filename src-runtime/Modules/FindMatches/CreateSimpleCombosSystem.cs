using System.Collections.Generic;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
namespace MatchThree.Rack.ECS.Systems
{
    public delegate void CreateSimpleCombo(ComboType type, int color, int key, List<IItem> items);

    public class CreateSimpleCombosSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<SimpleCombines>> __simpleCombines = default;
        //private readonly CreateSimpleCombo _createSimpleCombo = ECSStaticUtils.CreateCombo;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in __simpleCombines.Value)
            {
                SimpleCombines simpleCombines = __simpleCombines.Pools.Inc1.Get(entity);

                foreach (var combo in simpleCombines.Combos)
                {
                    //if (combo.Type() == ComboType.ThreeHorizontal || combo.Type() == ComboType.ThreeVertical)
                    //    CreateSimpleCombo(combo);
                }

                _world.DelEntity(entity);
            }
        }
        private void CreateSimpleCombo(MyCombo combo)
        {
            var typeCombo = combo.Type();
            var color = combo.Color;
            var key = combo.Key;
            var items = new List<IItem>(combo.Items);
            items.ForEach(x => x.IsMatch = true);
           // _createSimpleCombo(typeCombo, color, key, items);
            MyLog.Log("System/Green", $"Create Simple Combo { typeCombo} {color} {key} {items.Count}",GetType());
        }
    }
}