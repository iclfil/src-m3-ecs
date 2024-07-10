using System.Collections.Generic;
using MatchThree.Rack;
using Client.Game.Utils;
using MatchThree.Rack.Common;

//public List<IItem> Items;

//public MyCombo MyCombo;

//ComboItem - Bonus
//Items - обычные фишки.
namespace MatchThree.Rack.ECS
{
    public struct ComboInfo
    {
        public ComboType ComboType;
        public int Color;
        public int Key;
    }
    public struct Items
    {
        public List<IItem> MatchedItems;
    }

    //Combo, ComboItems - Matches, Ckeck, Destroying

    //  Combo { Color, Items}
    //  SpawnBonus { TypeBonus }
    //  Out => Combo, SpawnTag, LittleBombTag

    //  Combo { Color, Items }
    //  ExplosionBonus { TypeBonus }
    //  Out = Combo, MatchedTag, LittleBombTag

    //  Combo { Color, Items}
    //  SimpleComboTag {}

    public struct SimpleComboTag
    {

    }

    #region Bomb Bonuses

    #endregion


    //  Комбу можно уничтожить, или использовать для создания чего либо.

    //  Combo, CheckBonusComboTag - Проверка комбинации на Бонусы

    //  Combo, HasBonusesTag - В комбинации нашли бонусы

    //  Combo, ExplosionTag, LittleBombTag -- взрываем бомбу
    //  Combo, ExplosionTag, ColorBombTag -- взрываем бомбу
    //  Combo, ExplosionTag, StripedBombTag -- взрываем бомбу
    //  =>
    //  Combo, MatchedTag, LittleBombTag -- Чтобы передать ее в Destroying и он ее обработал
    //  Combo, MatchedTag, ColorBombTag -- Чтобы передать ее в Destroying и он ее обработал
    //  Combo, MatchedTag, StripedBombTag -- Чтобы передать ее в Destroying и он ее обработал

    // Combo, SpawnTag, LittleBombTag - Спавнить Бомбу
    // Combo, SpawnTag, ColorBombTag - Спавнить Бомбу
    // Combo, SpawnTag, StripedBombTag - Спавнить Бомбу

    //  Combo, MatchedComboTag - Комбинация уже на все проверена, и готова к следующей обработке(Destroying)

    //  Combo, MatchedComboTag, SimpleComboTag - Комбинация уже на все проверена, и готова к следующей обработке(Destroying)

    //  Combo, DestroyTag, ... // Уничтожаем уже в Destroying

    //Состояние - Свойство

    public struct HasBonusesTag
    {

    }

    public struct SpawnBonus
    {
        public TypeItems Type;
        public Int2 Position;
        public int Color;
    }

    struct FindTypeBonusInComboTag
    {
        // add your data here.
    }

    public struct ColorBombWithChipCombo
    {
        public IItem ColorBomb;
        public IItem Chip;
    }

    //GlassesTag, Items, MatchedTag -> DestroyTag

    public struct GlassesTag
    {

    }

    public struct StonesTag
    {

    }

    public struct DicesTag
    {

    }

    public struct SlimesTag
    {

    }

    /// <summary>
    /// Сущность можно сматчить
    /// </summary>
    public struct MatchesTag 
    {

    }

    public struct MatchedTag
    {

    }

    public struct DestroyTag
    {

    }
}
