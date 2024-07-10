using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack;
using MatchThree.Rack.Common.Services;
using MatchThree.Rack.ECS;
using MatchThree.Rack.ECS.SharedDatas;

namespace MatchThree.ECS.Modules.FindMatches
{
    public class CreateSimpleCombos : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsPoolInject<SimpleCombo> p_simpleCombo = default;
        private SharedData _sharedData;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _sharedData = systems.GetShared<SharedData>();
        }

        public void Run(IEcsSystems systems)
        {
            if (_sharedData.MatchesCombos.Count > 0)
            {
                var simple = _sharedData.MatchesCombos.FindAll(x => x.Type == ComboType.SimpleMatch);

                if (simple.Count > 0)
                {
                    foreach (var combo in simple)
                    {
                        MyLog.Log("System/Green", $"Create Simple Combo. Type: {combo.Type} Color: {combo.Color}  Items: {combo.Items.Count}", GetType());
                        ECSStaticUtils.CreateSimpleCombo(combo.Type, combo.Color, combo.Key, combo.Items);
                    }

                    foreach (var combo in simple)
                        _sharedData.MatchesCombos.Remove(combo);
                }
            }
        }

        private void CreateSimpleCombo(RCombo combo)
        {
            var entity = _world.NewEntity();
            ref var data = ref p_simpleCombo.Value.Add(entity);
            data.Combo = combo;
        }
    }
}