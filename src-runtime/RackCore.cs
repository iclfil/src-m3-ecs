using Client;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using MatchThree.ECS.Modules;
using MatchThree.ECS.Modules.FindMatches;
using MatchThree.Rack.Common;
using MatchThree.Rack.Common.Services;
using MatchThree.Rack.ECS;
using MatchThree.Rack.ECS.Systems;
using MatchThree.Rack.ECS.Systems.Falling;
using MatchThree.Rack.ECS.Systems.Input;
using MatchThree.Rack.Services;
using MatchThree.Rack.Utils;
using SevenBoldPencil.EasyEvents;
using SharedData = MatchThree.Rack.ECS.SharedDatas.SharedData;

namespace MatchThree.Rack
{
    public class RackModuleNotify
    {
        public OnRackCreated RackCreated { get; set; }
        public OnFindMatchesFinished FindMatchesFinished { get; set; }
        public OnMatchesFinished MatchesFinished { get; set; }
        public OnDestroyingFinished DestroyingFinished { get; set; }
        public OnCollapseFinished CollapseFinished { get; set; }

        public void Notify(NameModule name, ModuleResult result)
        {
            if (name == NameModule.Starting)
                RackCreated?.Invoke();

            if (name == NameModule.FindMatches)
            {
                var hasMatches = result == ModuleResult.HasMatches;
                FindMatchesFinished?.Invoke(hasMatches);
            }

            if (name == NameModule.Matches)
                MatchesFinished?.Invoke();

            if (name == NameModule.Destroying)
                DestroyingFinished?.Invoke();

            if (name == NameModule.Collapse)
                CollapseFinished?.Invoke();
        }
    }

    public class RackCore : IRackCore
    {
        private IBoard _board;

        #region ECS
        private EcsWorld _world;
        private IEcsSystems _systems;

        private EcsWorld _itemsWorld;
        private IEcsSystems _itemsSystems;
        #endregion

        public OnStateUpdated StateUpdated { get; set; }
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

        public IBoardFacade Board { get; set; }

        public EventsBus EventsBus;
        private readonly RackModuleNotify moduleNotify;

        public RackCore()
        {
            EventsBus = new EventsBus();
        }

        public RackCore(EventsBus eventsBus, RackModuleNotify moduleNotify)
        {
            EventsBus = eventsBus;
            this.moduleNotify = moduleNotify;
        }

        public void Start(IBoard board)
        {
            _board = board;

            #region Init
            SharedData sharedData = new SharedData
            {
                HasMatches = false
            };

            _world = new EcsWorld();
            var itemsWorld = new EcsWorld();

            _systems = new EcsSystems(_world, sharedData);
            _systems.AddWorld(itemsWorld, "Board");
            _systems.AddWorld(EventsBus.GetEventsWorld(), "Events");

            Board = new BoardFacade(9, 9, itemsWorld, _board);

            ECSStaticUtils.World = _world; //TODO - Мир должен передаваться постоянно из нужного ядра
            ECSStaticUtils.Bus = EventsBus;
            #endregion


            _systems
                .Add(new ModulesHandlerSystem(EventsBus, moduleNotify))
                .Add(new StateEnableSystem(EventsBus))
                .Add(new SwappingHandlerSystem(Board, EventsBus, OnSwapItems))
                .Add(new UnSwappingSystem(Board, EventsBus, OnSwapItems))
                .Add(new DoubleClickHandlerSystem(EventsBus, _board))

                //Bonus Matches
                .Add(new MatchBonusesHandler(EventsBus))
                .Add(new MatchBombWithBombSystem(EventsBus, Board, OnMatchBombWithBomb))
                .Add(new MatchColorBombWithChip(EventsBus, Board, OnMatchColorBombWithChip))
                .Add(new MatchStripedWithStriped(EventsBus, Board, OnMatchStripedWithStriped))
                .Add(new MatchColorBombWithStriped(EventsBus, Board, OnMatchColorBombWithStriped))
                .Add(new MatchColorBombWithColorBomb(EventsBus, Board, OnMatchColorBombWithColorBomb))
                .Add(new MatchColorBombWithLittleBomb(EventsBus, Board, OnMatchColorBombWithLittleBomb))



                // .Add(new FindPossibleMoveSystem())
            #region Starting
                .AddGroup(str(RackState.Starting), false, null,
                          new CreateBoardSystem(Board, CreateRackCallback),
                                         new ActivateCageSystem(Board),
                                         new FinishStartingStateSystem(EventsBus))
            #endregion
            #region Waiting
                //.AddGroup(str(RackState.Waiting), false, null,
                //    new ShowHintSystem(Hint),
                //    new SwappingHandlerSystem(Board, EventsBus),
                //    new FinishWaitingStateSystem(EventsBus))
            #endregion
            #region FindMatches
                .AddGroup(str(RackState.FindMatches), false, null,
                    //Проверка на бонус свап.
                    new FindItemsForMatches(Board),
                    new FindSimpleCombines(),
                    new CreateSimpleCombos(),
                    new CreateComplexCombos(),
                    new CheckHasMatchesSystem(),
                    //new SwappingSystem(OnSwapItems),
                    new FinishFindMatchesStateSystem(EventsBus))

            #endregion
            #region SwapBonus
                .AddGroup(str(RackState.SwapBonus), false, null,
                    new FinishSwapBonusStateSystem(EventsBus))

            #endregion
            #region NoMatches

                .AddGroup(str(RackState.NoMatches), false, null,
                    //new UnSwappingSystem(Board, OnSwapItems),
                    new FinishNoMatchesSystem(EventsBus))

            #endregion
            #region Matches
                .AddGroup(str(RackState.Matches), false, null,
                    new CheckCombosSystem(),
                    new CageHandlerSystem(Board),
                    new MatchStripedBomb(Board),
                    new MatchLittleBomb(Board),
                    new FlyBombMatchesSystem(Board),
                    new GlassBlocksHandlerSystem(Board),
                    new SpawnBonusesSystem(Board, SpawnBonusCallback),
                    new SlimeBlocksHandlerSystem(Board),
                    new CheckFinishMatchesSystem(),
                    new FinishMatchesStateSystem(EventsBus))
            #endregion
            #region Destroying
                .AddGroup(str(RackState.Destroying), false, null,
                    new SlimeBlockMechanicSystem(Board, OnMechanicActivate),
                    new SetDestroyCombosSystem(),
                    new DestroyGlassesSystem(Board, DestroyBlocksCallback),
                    new MatchBonusCombosSystem(Board, MatchBonusCallback),
                    new DestroySimpleCombosSystem(Board, MatchComboCallback),
                    new FinishDestroyingStateSystem(EventsBus))

            #endregion
            #region Collapse
                .AddGroup(str(RackState.Collapse), false, null,
                    new CreatePullersSystem(Board),
                    new CheckLivePullersSystem(),
                    new PullersHandlerSystem(Board),
                    new ActivatePullersSystem(Board),
                    new IngredientRulesSystem(Board, OnMatchIngredient),
                    new FinishFallingStateSystem(CollapseItems, EventsBus))
            #endregion
            #region Rule
                // .Add(new IngredientRulesSystem(Board, OnMatchIngredient))
            #endregion

            #region Finishing
                .AddGroup(str(RackState.Finishing), false, null,
                    new ModelFinishingSystem(EventsBus))
            #endregion

            #region DebugSystems
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem("Events"))
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem("Board"))
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
            #endregion
                .Inject(new FindMatchesService())
                .Inject(new MatchesService())
                .Init();

        }
        public void Update()
        {
            _itemsSystems?.Run();
            _systems?.Run();
            // _busSystem?.Run();
        }
        public void OnDestroy()
        {
            if (Board != null)
                Board = null;

            if (_systems != null)
            {
                _systems.GetWorld("Board").Destroy();
                //_systems.GetWorld("Events").Destroy();
                // list of custom worlds will be cleared
                // during IEcsSystems.Destroy(). so, you
                // need to save it here if you need.
                _systems.Destroy();
                _systems = null;
            }

            if (_itemsSystems != null)
            {
                _itemsSystems.Destroy();
                _itemsSystems = null;
            }

            // cleanup custom worlds here.
            if (_itemsWorld != null)
            {
                _itemsWorld.Destroy();
                _itemsWorld = null;
            }
            // cleanup default world.
            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }

        #region Utils
        private string str(RackState rackStateGame)
        {
            return rackStateGame.ToString();
        }
        #endregion
    }

    public class MatchBonusesHandler : IEcsRunSystem
    {
        private readonly EventsBus eventsBus;

        public MatchBonusesHandler(EventsBus eventsBus)
        {
            this.eventsBus = eventsBus;
        }

        public void Run(IEcsSystems systems)
        {
            if (eventsBus.HasEventSingleton<MatchBonusWithBonusEvent>() == false)
                return;

            var data = eventsBus.GetEventBodySingleton<MatchBonusWithBonusEvent>();

                //TODO - кал полный, фиксить все это.

            if (data.FirstBonus.Type == TypeItems.Bomb && data.SecondBonus.Type == TypeItems.Bomb)
            {
                ref var d = ref eventsBus.NewEventSingleton<MatchBombWithBombEvent>();
                d.Bomb_1 = data.FirstBonus;
                d.Bomb_2 = data.SecondBonus;
            }

            if (data.FirstBonus.Type == TypeItems.ColorBomb && data.SecondBonus.Type == TypeItems.Chip)
            {
                ref var d = ref eventsBus.NewEventSingleton<MatchColorBombWithChipEvent>();
                d.ColorBomb = data.FirstBonus;
                d.Chip = data.SecondBonus;
            }

            if (data.FirstBonus.Type == TypeItems.Chip && data.SecondBonus.Type == TypeItems.ColorBomb)
            {
                ref var d = ref eventsBus.NewEventSingleton<MatchColorBombWithChipEvent>();
                d.Chip = data.FirstBonus;
                d.ColorBomb = data.SecondBonus;
            }

            if (data.FirstBonus.Type == TypeItems.Chip && data.SecondBonus.Type == TypeItems.VerticalBomb)
            {
                ref var d = ref eventsBus.NewEventSingleton<MatchColorBombWithChipEvent>();
                d.Chip = data.FirstBonus;
                d.ColorBomb = data.SecondBonus;
            }

            if (ItemsProperties.IsStriped(data.FirstBonus) && ItemsProperties.IsStriped(data.SecondBonus))
            {
                ref var d = ref eventsBus.NewEventSingleton<MatchStripedWithStripedEvent>();
                d.Striped_1 = data.FirstBonus;
                d.Striped_2 = data.SecondBonus;
            }

            //Color Bomb With Striped
            if (data.FirstBonus.Type == TypeItems.ColorBomb && ItemsProperties.IsStriped(data.SecondBonus))
            {
                ref var d = ref eventsBus.NewEventSingleton<MatchColorBombWithStripedEvent>();
                d.ColorBomb = data.FirstBonus;
                d.StripedBomb = data.SecondBonus;
            }

            if (data.SecondBonus.Type == TypeItems.ColorBomb && ItemsProperties.IsStriped(data.FirstBonus))
            {
                ref var d = ref eventsBus.NewEventSingleton<MatchColorBombWithStripedEvent>();
                d.ColorBomb = data.SecondBonus;
                d.StripedBomb = data.FirstBonus;
            }

            if (data.FirstBonus.Type == TypeItems.ColorBomb && data.SecondBonus.Type == TypeItems.ColorBomb)
            {
                ref var d = ref eventsBus.NewEventSingleton<MatchColorBombWithColorBombEvent>();
                d.ColorBomb_1 = data.FirstBonus;
                d.ColorBomb_2 = data.SecondBonus;
            }


            if ((data.FirstBonus.Type == TypeItems.ColorBomb && data.SecondBonus.Type == TypeItems.Bomb))
            {
                ref var d = ref eventsBus.NewEventSingleton<MatchColorBombWithLittleBombEvent>();
                d.ColorBomb = data.FirstBonus;
                d.LittleBomb = data.SecondBonus;
            }

            if ((data.FirstBonus.Type == TypeItems.Bomb && data.SecondBonus.Type == TypeItems.ColorBomb))
            {
                ref var d = ref eventsBus.NewEventSingleton<MatchColorBombWithLittleBombEvent>();
                d.ColorBomb = data.SecondBonus;
                d.LittleBomb = data.FirstBonus;
            }

            eventsBus.DestroyEventSingleton<MatchBonusWithBonusEvent>();
        }
    }
}