using System.Collections.Generic;
using Client.Game.Utils;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;

namespace MatchThree.Rack.ECS.Systems
{
    public class SpawnBonusesSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<ComboInfo, Items, SpawnBonus, MatchedTag>> _spawnBonuses = default;

        private IBoardFacade _board;
        private readonly OnSpawnBonus _spawnBonus;
        private EcsWorld _world;
        public SpawnBonusesSystem(IBoardFacade board, OnSpawnBonus spawnBonus)
        {
            _board = board;
            _spawnBonus = spawnBonus;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }
        public void Run(IEcsSystems systems)
        {
            foreach (var _Enty in _spawnBonuses.Value)
            {
                var comboData = _spawnBonuses.Pools.Inc1.Get(_Enty);
                var items = _spawnBonuses.Pools.Inc2.Get(_Enty).MatchedItems;
                var spawnPos = _spawnBonuses.Pools.Inc3.Get(_Enty);
                SpawnBonus(comboData, items, spawnPos);
                _world.DelEntity(_Enty);
            }
        }
        private void SpawnBonus(ComboInfo comboInfo, List<IItem> matchedItems, SpawnBonus spawnPos)
        {
            var bonusColor = comboInfo.Color;
            var bonusSpawnPos = spawnPos.Position;
            var bonusType = ECSStaticUtils.GetTypeBonusItemByComboType(comboInfo.ComboType);

            if (matchedItems.Count == 0)
            {
                CreateBonus(bonusSpawnPos, bonusColor, bonusType);
                return;
            }

            for (int i = 0; i < matchedItems.Count; i++)
            {
                var item = matchedItems[i];
                if (_board.Has(item) == false)
                {
                    matchedItems.Remove(item);
                    continue;
                }
                _board.DeleteIncludeItem(item);
            }

            var createdBonus = _board.CreateItem(bonusSpawnPos, bonusColor, bonusType);
            _spawnBonus?.Invoke(createdBonus, matchedItems);
        }

        private void CreateBonus(Int2 bonusSpawnPos, int bonusColor, TypeItems bonusType)
        {
            var createdBonus = _board.CreateItem(bonusSpawnPos, bonusColor, bonusType);
            _spawnBonus?.Invoke(createdBonus, null);
        }


    }
}