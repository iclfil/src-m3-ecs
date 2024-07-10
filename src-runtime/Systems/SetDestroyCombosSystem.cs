using MatchThree.Rack.ECS;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace MatchThree.Rack.ECS.Systems
{
    public class SetDestroyCombosSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ECS.Items, MatchedTag>, Exc<DestroyTag>> _matchedCombos = default;

        //private readonly EcsFilterInject<Inc<StripedBomb, Items, MatchedTag>, Exc<DestroyTag>> _stripeds = default;
        //private readonly EcsFilterInject<Inc<ColorBomb, Items, MatchedTag>, Exc<DestroyTag>> _colorsBombs = default;
        //private readonly EcsFilterInject<Inc<LittleBomb, Items, MatchedTag>, Exc<DestroyTag>> _littleBombs = default;

        private readonly EcsPoolInject<DestroyTag> _destoyTag = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var combo in _matchedCombos.Value)
            {
                Clear_MatchedTag(combo);
                Add_DestroyTag(combo);
            }
        }

        private void Add_DestroyTag(int combo)
        {
            _destoyTag.Value.Add(combo);
        }

        private void Clear_MatchedTag(int combo)
        {
            _matchedCombos.Pools.Inc2.Del(combo);
        }
    }
}