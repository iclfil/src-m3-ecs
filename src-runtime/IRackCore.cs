using System.Collections;
using System.Collections.Generic;
using MatchThree.ECS.Modules;
using MatchThree.Rack.Common;

namespace MatchThree.Rack
{
    public enum TypeMechanic
    {
        SlimeActivated
    }

    public interface IRackCoreNotify
    {
        public OnHint Hint { get; set; }
        public OnCollapseItems CollapseItems { get; set; }
        public OnFinishPlaying FinishPlaying { get; set; }
        public OnDestroyBlocks DestroyBlocksCallback { get; set; }
        public OnCreateRack CreateRackCallback { get; set; }
        public OnSwapItems OnSwapItems { get; set; }
        public OnMatchBonus MatchBonusCallback { get; set; }
        public OnDestroyCombo MatchComboCallback { get; set; }
        public OnSpawnBonus SpawnBonusCallback { get; set; }
        public OnMatchBonusCombo MatchBonusComboCallback { get; set; }
        public OnMatchIngredient OnMatchIngredient { get; set; }
        public OnMechanicActivate OnMechanicActivate { get; set; }
        public OnMechanicsActivate OnMechanicsActivate { get; set; }

        public OnMatchBombWithBomb OnMatchBombWithBomb { get; set; }
        public OnMatchColorBombWithChip OnMatchColorBombWithChip { get; set; }
        public OnMatchStripedWithStriped OnMatchStripedWithStriped { get; set; }
        public OnMatchColorBombWithStriped OnMatchColorBombWithStriped { get; set; }
        public OnMatchColorBombWithColorBomb OnMatchColorBombWithColorBomb { get; set; }
        public OnMatchColorBombWithLittleBomb OnMatchColorBombWithLittleBomb { get; set; }

    }

    public delegate void OnMechanicActivate(TypeMechanic typeMechanic, IItem destroyedItem, IItem createdItem);

    public delegate void OnMechanicsActivate(TypeMechanic typeMechanic, IEnumerable<IItem> destroyedItems, IEnumerable<IItem> createdItems);

    public delegate void OnModuleFinished(NameModule nameModule, ModuleResult result);

    public delegate void OnRackCreated();
    public delegate void OnUnSwapFinished();

    public delegate void OnCollapseFinished();

    public delegate void OnDestroyingFinished();

    public delegate void OnMatchesFinished();

    public delegate void OnFindMatchesFinished(bool hasMatches);

    public interface IRackCore2
    {
        public void Start();
        public void Update();
        public void Destroy();
    }

    public interface IEnemyRackCore : IRackCore2, IRackCoreNotify
    {
        public void MadeMove();
    }

    public interface IPlayerRackCore : IRackCore2
    {

    }

    public interface IRackCore : IRackCoreNotify
    {
        public void Start(IBoard board = null);
        public void Update();
        public void OnDestroy();
    }
}