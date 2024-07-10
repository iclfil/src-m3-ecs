using System.Collections.Generic;
using Client;
using Client.Game.Utils;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MatchThree.Rack.Common;
using SevenBoldPencil.EasyEvents;

namespace MatchThree.Rack.ECS.Systems
{
    public class MatchBombWithBombSystem : IEcsRunSystem, IEcsInitSystem
    {
        public OnMatchBombWithBomb Callback { get; }
        private readonly IBoardFacade _board;
        public EventsBus Bus { get; }
        private EcsWorld _world;

        public MatchBombWithBombSystem(EventsBus bus, IBoardFacade board, OnMatchBombWithBomb callback)
        {
            _board = board;
            Bus = bus;
            Callback = callback;
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
        }

        public void Run(IEcsSystems systems)
        {
            if (Bus.HasEventSingleton<MatchBombWithBombEvent>() == false)
                return;

            var data = Bus.GetEventBodySingleton<MatchBombWithBombEvent>();

            var explosion_1 = Explosion(data.Bomb_1, 2);
            var explosion_2 = Explosion(data.Bomb_2, 2);

            var matchedItems = new List<IItem>();
            matchedItems.AddRange(explosion_1);
            matchedItems.AddRange(explosion_2);

            Callback?.Invoke(new MatchBombWithBombContext()
            {
                Bomb_1 = data.Bomb_1,
                Bomb_2 = data.Bomb_2,
                MatchedItems = matchedItems,
            });

            Bus.DestroyEventSingleton<MatchBombWithBombEvent>();
        }

        public List<IItem> Explosion(IItem bomb, int range = 2)
        {
            bomb.IsMatched = true;
            var bonusPos = bomb.Position;
            var color = bomb.Color;

            var matchedItems = new List<IItem>();

            for (int x = bonusPos.X - range; x <= bonusPos.X + range; x++)
            {
                for (int y = bonusPos.Y - range; y <= bonusPos.Y + range; y++)
                {
                    var deletePos = new Int2(x, y);

                    if (_board.TryGetLastItem(deletePos, out var item))
                    {
                        if (item.IsMatched)
                            continue;

                        //Удалем только фишкик, и потом пометим, что здесь матч, чтобы остальные системы отработали
                        if (item.Type == TypeItems.Chip)
                        {
                            _board.DeleteIncludeItem(item);
                            matchedItems.Add(item);
                        }
                    }
                }
            }

            _board.DeleteIncludeItem(bomb);

            return matchedItems;
        }
    }
}