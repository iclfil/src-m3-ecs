using System.Collections.Generic;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common.Services;
using MatchThree.Rack.ECS.Behaviors;
using UnityEngine;

namespace MatchThree.Rack.ECS.Systems
{
    public class FindPossibleMoveSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ItemData, MatchBehavior>> _chips = "Board";
        //private readonly EcsFilterInject<Inc<ItemData, MatchBehavior>> _chips = "Board";

        private FindMatchesService _findMatchesService;
        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _findMatchesService = new FindMatchesService();
        }

        public void Run(IEcsSystems systems)
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {

                MyLog.Log("Find Tip", Color.white);
                _findMatchesService.FindMaybeMoves(GetColorsItems(1));
            }
        }

        private List<IItem> GetColorsItems(int color)
        {
            var items = new List<IItem>();
            foreach (var e_chip in _chips.Value)
            {
                if (color == -1)
                {
                    var item = _chips.Pools.Inc1.Get(e_chip).Item;
                    items.Add(item);
                    continue;
                }

                if (_chips.Pools.Inc1.Get(e_chip).Item.Color == color)
                {
                    var item = _chips.Pools.Inc1.Get(e_chip).Item;
                    items.Add(item);
                }
            }

            return items;
        }
    }
}