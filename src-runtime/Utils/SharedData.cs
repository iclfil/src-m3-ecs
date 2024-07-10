using System.Collections.Generic;
using Client.Game.Utils;
using MatchThree.Rack.Common.Services;
using MatchThree.Rack.ECS.Systems;

namespace MatchThree.Rack.ECS.SharedDatas
{
    public class SharedData
    {
        public bool HasMatches = false;

        public bool SlimeActivate = false;
        //Collapse
        public List<PullItem> PullerItems = new List<PullItem>();

        public List<RCombo> MatchesCombos = new List<RCombo>();

        public Dictionary<Int2, IItem> Matches = new Dictionary<Int2, IItem>();
    }
}