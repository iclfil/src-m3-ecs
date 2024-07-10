using System.Collections.Generic;
using MatchThree.Rack.ECS;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.CoreECS.Tags;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS.Tags;

namespace MatchThree.Rack.ECS
{
    public class FlyBombMatchesSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<FlyBomb, MatchesTag>> _bonusesMatches = default;
        private readonly EcsFilterInject<Inc<ItemData, MatchBehavior, ChipTag>> _chips = "Board";

        private readonly IBoardFacade _board;
        private EcsWorld _world;


        public FlyBombMatchesSystem(IBoardFacade board)
        {
            _board = board;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _bonusesMatches.Value)
            {
                var bomb = _bonusesMatches.Pools.Inc1.Get(entity).Bonus;
                Explosion(bomb);
                _world.DelEntity(entity);
            }
        }

        private void Explosion(IItem bomb)
        {
            IItem chip1 = null;
            IItem chip2 = null;

            var itemsCounts = _chips.Value.GetEntitiesCount();

            var entity1 = _chips.Value.GetRawEntities()[UnityEngine.Random.Range(0, itemsCounts)];
            var entity2 = _chips.Value.GetRawEntities()[UnityEngine.Random.Range(0, itemsCounts)];

            chip1 = _chips.Pools.Inc1.Get(entity1).Item;
            chip2 = _chips.Pools.Inc1.Get(entity2).Item;

            var matchesItems = new List<IItem>(2);
            matchesItems.Add(chip1);
            matchesItems.Add(chip2);

            MatchedBonus(bomb, matchesItems);
            MyLog.Log("Collapse Fly Bomb" + bomb);
        }

        private void MatchedBonus(IItem bonus, List<IItem> matchedItems)
        {
            var entity = _world.NewEntity();
            var pool1 = _world.GetPool<FlyBomb>();
            ref FlyBomb data = ref pool1.Add(entity);
            data.Bonus = bonus;

            var pool2 = _world.GetPool<ECS.Items>();
            ref ECS.Items itemsData = ref pool2.Add(entity);
            itemsData.MatchedItems = new List<IItem>(matchedItems);

            var pool3 = _world.GetPool<MatchedTag>();
            pool3.Add(entity);

            var pool4 = _world.GetPool<CheckBonusesTag>();
            pool4.Add(entity);

            MyLog.Log("System/Green", $"Create Matches Bonus {bonus.Type} {bonus.Position} CountItems {matchedItems.Count} ", GetType());
        }
    }
}