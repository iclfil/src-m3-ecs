using System.Collections.Generic;
using Client.Game.Utils;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common.Services;
using MatchThree.Rack.ECS.Behaviors;
using UnityEngine;

namespace MatchThree.Rack.ECS.Systems
{
    public class ShowHintSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<ItemData, MatchBehavior>> _chips = "Board";

        private FindPossibleMovesService _findPossibleMoves;

        private readonly OnHint _hintCallback;


        public ShowHintSystem(OnHint hintCallback)
        {
            _hintCallback = hintCallback;
        }

        public void Init(IEcsSystems systems)
        {
            _findPossibleMoves = new FindPossibleMovesService();
        }

        public float TimeToHint = 4;

        public void Run(IEcsSystems systems)
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.H))
            {
                var positions = new List<Int2>();
                var items = GetColorsItems(1);
                var dic = new Dictionary<Int2, IItem>();
                items.ForEach((item =>
                {
                    dic.Add(item.Position, item);
                    positions.Add(item.Position);
                }));

                var lines = _findPossibleMoves.CheckMoves(positions);

                if (lines.Count > 0)
                {
                    var rand = UnityEngine.Random.Range(0, lines.Count - 1);
                    var first = lines[rand];
                    var tipItems = new List<IItem>();

                    foreach (var pos in first.Points)
                        tipItems.Add(dic[pos]);

                    _hintCallback?.Invoke(tipItems);

                    MyLog.Log("Show Hint", Color.green);
                }
                else
                {
                    MyLog.Log("No Hints", Color.red);
                }
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