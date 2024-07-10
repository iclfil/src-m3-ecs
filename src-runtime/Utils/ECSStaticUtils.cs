using System;
using System.Collections.Generic;
using System.Linq;
using Client.Game.Utils;
using Leopotam.EcsLite;
using MatchThree.ECS.Modules.FindMatches;
using MatchThree.Rack.Common;
using MatchThree.Rack.CoreECS.Tags;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS
{
    public static class ECSStaticUtils
    {
        private static int defaultColor = -1;
        private static Int2 defaultPosition = new Int2(-1, -1);
        private static TypeItems defaultTypeItems = TypeItems.None;
        public static EcsWorld World;
        public static EventsBus Bus;

        //public static void StateFinish(NameModule name, ModuleResult result = ModuleResult.Default)
        //{
        //    ref StateFinishedEvent _a = ref Bus.NewEventSingleton<StateFinishedEvent>();
        //    _a.Name = name;
        //    _a.Result = result;
        //}

        /// <summary>
        /// From bonus.Type() add Component: StripedBomb, LittleBomb, ColorBomb
        /// Add MatchesTag
        /// </summary>
        /// <param name="bonus"></param>
        public static void CreateMatchesBonus(IItem bonus, int range = 1)
        {
            if (bonus.IsBonus == false)
                return;

            var entity = World.NewEntity();

            if (bonus.Type == TypeItems.FlyBomb)
            {
                var pool1 = World.GetPool<FlyBomb>();
                ref FlyBomb data = ref pool1.Add(entity);
                data.Bonus = bonus;
            }


            if (bonus.Type == TypeItems.HorizontalBomb || bonus.Type == TypeItems.VerticalBomb)
            {
                var pool1 = World.GetPool<StripedBomb>();
                ref StripedBomb data = ref pool1.Add(entity);
                data.Bonus = bonus;
            }

            if (bonus.Type == TypeItems.Bomb)
            {
                var pool1 = World.GetPool<LittleBomb>();
                ref LittleBomb data = ref pool1.Add(entity);
                data.Bonus = bonus;
                data.Range = range;
            }

            if (bonus.Type == TypeItems.ColorBomb)
            {
                var pool1 = World.GetPool<ColorBomb>();
                ref ColorBomb data = ref pool1.Add(entity);
                data.Bonus = bonus;
            }

            var pool2 = World.GetPool<MatchesTag>();
            pool2.Add(entity);
        }

        public static void CreateBonusCombo(ComboType comboType, int color, int key, HashSet<IItem> items)
        {
            var entity = World.NewEntity();
            var pool1 = World.GetPool<ComboInfo>();
            ref var comboData = ref pool1.Add(entity);
            comboData.Color = color;
            comboData.ComboType = comboType;
            comboData.Key = key;

            var pool2 = World.GetPool<ECS.Items>();
            ref ECS.Items itemsData = ref pool2.Add(entity);
            itemsData.MatchedItems = new List<IItem>(items);

            var pool3 = World.GetPool<SpawnBonus>();
            ref SpawnBonus s = ref pool3.Add(entity);
            s.Color = color;
            s.Position = GetRandomPos(items);
            s.Type = GetTypeBonusItemByComboType(comboType);

            var pool4 = World.GetPool<CheckBonusesTag>();
            pool4.Add(entity);

            var pool5 = World.GetPool<ComplexCombo>();
            pool5.Add(entity);
        }

        private static Int2 GetRandomPos(HashSet<IItem> items)
        {
            int rand = UnityEngine.Random.Range(0, items.Count);
            IItem element = items.ElementAt(rand);
            return element.Position;
        }


        public static void CreateSimpleCombo(ComboType comboType, int color, int key, HashSet<IItem> items)
        {
            var entity = World.NewEntity();

            var pool1 = World.GetPool<ComboInfo>();
            ref var comboData = ref pool1.Add(entity);
            comboData.Color = color;
            comboData.ComboType = comboType;
            comboData.Key = key;

            var pool2 = World.GetPool<Items>();
            ref Items itemsData = ref pool2.Add(entity);
            itemsData.MatchedItems = new List<IItem>(items);

            var pool4 = World.GetPool<CheckBonusesTag>();
            pool4.Add(entity);

            var pool5 = World.GetPool<SimpleCombo>();
            pool5.Add(entity);
        }

        public static TypeItems GetTypeBonusItemByComboType(ComboType comboType)
        {
            Enum.TryParse(comboType.ToString(), out TypeItems res);
            return res;
        }

        public static void MatchBonus(IItem bonus, EcsWorld world)
        {
            var entity = World.NewEntity();

            if (bonus.Type == TypeItems.FlyBomb)
            {
                var pool1 = World.GetPool<FlyBomb>();
                ref FlyBomb data = ref pool1.Add(entity);
                data.Bonus = bonus;
            }


            if (bonus.Type == TypeItems.HorizontalBomb || bonus.Type == TypeItems.VerticalBomb)
            {
                var pool1 = World.GetPool<StripedBomb>();
                ref StripedBomb data = ref pool1.Add(entity);
                data.Bonus = bonus;
            }

            if (bonus.Type == TypeItems.Bomb)
            {
                var pool1 = World.GetPool<LittleBomb>();
                ref LittleBomb data = ref pool1.Add(entity);
                data.Bonus = bonus;
                data.Range = 1;
            }

            if (bonus.Type == TypeItems.ColorBomb)
            {
                var pool1 = World.GetPool<ColorBomb>();
                ref ColorBomb data = ref pool1.Add(entity);
                data.Bonus = bonus;
            }

            var pool2 = World.GetPool<MatchesTag>();
            pool2.Add(entity);
        }
    }
}


