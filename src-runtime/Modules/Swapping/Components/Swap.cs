using MatchThree.Rack;

namespace MatchThree.Rack.ECS
{
    public struct Swap
    {
        public SwapType SwapType;
        public BonusSwapType BonusSwapType;
        public IItem FirstItem;
        public IItem SecondItem;
    }

    public struct UnSwapTag
    {

    }
}