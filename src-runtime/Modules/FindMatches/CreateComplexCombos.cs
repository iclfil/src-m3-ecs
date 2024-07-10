using System.Linq;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack;
using MatchThree.Rack.Common.Services;
using MatchThree.Rack.ECS;
using MatchThree.Rack.ECS.SharedDatas;

namespace MatchThree.ECS.Modules.FindMatches
{
    public class CreateComplexCombos : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsPoolInject<ComplexCombo> p_complexCombo = default;
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
                var complex = _sharedData.MatchesCombos.FindAll(x => x.Type != ComboType.SimpleMatch);

                if (complex.Count > 0)
                {
                    foreach (var combo in complex)
                    {
                        MyLog.Log("System/Green", $"Create Bonus Combo. Type: {combo.Type} Color: {combo.Color}  Items: {combo.Items.Count}", GetType());
                        ECSStaticUtils.CreateBonusCombo(combo.Type, combo.Color, combo.Key, combo.Items);
                    }

                    foreach (var combo in complex)
                        _sharedData.MatchesCombos.Remove(combo);
                }
            }
        }

        private void CreateComplexCombo(RCombo combo)
        {
            var entity = _world.NewEntity();
            ref var data = ref p_complexCombo.Value.Add(entity);
            data.Combo = combo;
        }
    }
}