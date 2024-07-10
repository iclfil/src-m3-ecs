using System.Collections.Generic;
using Client.Game.Utils;
using FilConsole;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using MatchThree.Rack.Items;
using UnityEngine;

namespace MatchThree.Rack.ECS
{
    /// <summary>
    /// Довести до цели.
    /// </summary>
    public class IngredientRulesSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly IBoardFacade _board;
        private readonly OnMatchIngredient _matchIngredient;
        public TypeItems Type = TypeItems.Ingredient;
        public EcsFilterInject<Inc<Ingredient>> _ingredients = "Board";
        //Хардкод позиций
        private List<Int2> positions = new List<Int2>()
            { new Int2(2, 0),new Int2(3, 0),new Int2(4, 0),new Int2(5, 0), new Int2(6, 0), new Int2(1, 1), new Int2(7, 1), new Int2(0, 4), new Int2(8, 4) };


        public IngredientRulesSystem(IBoardFacade board, OnMatchIngredient matchIngredient)
        {
            _board = board;
            _matchIngredient = matchIngredient;
        }

        public void Init(IEcsSystems systems)
        {
        }

        public void Run(IEcsSystems systems)
        {
            var list = new List<IItem>();
            foreach (var entity in _ingredients.Value)
            {
                var item = _ingredients.Pools.Inc1.Get(entity).Item;
                var pos = _ingredients.Pools.Inc1.Get(entity).Item.Position;
                if (positions.Contains(pos))
                {
                    var e = systems.GetWorld().NewEntity();
                    var pool = systems.GetWorld().GetPool<FindIngredient>();
                    pool.Add(e);
                    list.Add(item);
                    _board.DeleteIncludeItem(item);
                    MyLog.Log("INGREDIENT RULES: найден ингредиент в нужной позиции", Color.green);
                    _matchIngredient?.Invoke(list);
                }
            }
        }
    }
}