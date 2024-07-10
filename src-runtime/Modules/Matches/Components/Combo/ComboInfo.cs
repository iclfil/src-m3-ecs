using System.Collections.Generic;
using MatchThree.Rack;
using Client.Game.Utils;
using MatchThree.Rack.Common;

//public List<IItem> Items;

//public MyCombo MyCombo;

//ComboItem - Bonus
//Items - ������� �����.
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


    //  ����� ����� ����������, ��� ������������ ��� �������� ���� ����.

    //  Combo, CheckBonusComboTag - �������� ���������� �� ������

    //  Combo, HasBonusesTag - � ���������� ����� ������

    //  Combo, ExplosionTag, LittleBombTag -- �������� �����
    //  Combo, ExplosionTag, ColorBombTag -- �������� �����
    //  Combo, ExplosionTag, StripedBombTag -- �������� �����
    //  =>
    //  Combo, MatchedTag, LittleBombTag -- ����� �������� �� � Destroying � �� �� ���������
    //  Combo, MatchedTag, ColorBombTag -- ����� �������� �� � Destroying � �� �� ���������
    //  Combo, MatchedTag, StripedBombTag -- ����� �������� �� � Destroying � �� �� ���������

    // Combo, SpawnTag, LittleBombTag - �������� �����
    // Combo, SpawnTag, ColorBombTag - �������� �����
    // Combo, SpawnTag, StripedBombTag - �������� �����

    //  Combo, MatchedComboTag - ���������� ��� �� ��� ���������, � ������ � ��������� ���������(Destroying)

    //  Combo, MatchedComboTag, SimpleComboTag - ���������� ��� �� ��� ���������, � ������ � ��������� ���������(Destroying)

    //  Combo, DestroyTag, ... // ���������� ��� � Destroying

    //��������� - ��������

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
    /// �������� ����� ��������
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
