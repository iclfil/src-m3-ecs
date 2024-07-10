using Client.Game.Utils;
using Client.MatchThree.Board.ECS;
using FilConsole;
using Leopotam.EcsLite;
using MatchThree.Rack;

namespace MatchThree.Rack.ECS.Systems
{
    public class LoadBoardSystem : IEcsRunSystem
    {

        private readonly IBoardFacade _board;
        private readonly string _handler;
     

        public LoadBoardSystem(IBoardFacade board,string handler)
        {
            _board = board;
            _handler = handler;
        }

        public void Run(IEcsSystems systems)
        {
            int[] exludeColors = new int[3] { -1, -1, -1 };

            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    var pos = new Int2(x, y);

                    var items = _board.GetItems(x, y);

                    foreach (var item in items)
                    {
                        var type = item.Type;
                        var color = item.Color;
                        var id = item.GetID();
                        //_board.LoadItem(item);
                        PushView(item);
                    }
                }
            }

            MyLog.Log("System", "Load Board Completed", GetType());
        }

        private void PushView(IItem item)
        {
            //_createItemDelegate.Invoke(item, _handler, "LoadBoardSystem");

            //////TODO - ItemViewProvider -> IItem
            //var itemView = new ItemViewProvider(item.GetID(), item.Position, item.Color, item.Type)
            //{
            //    IsInteract = false
            //};
            //// _delegatesHandler.Add(() => _createItemDelegate(itemView, "Starting", "LoadBoardSystem"));
            //_createItemDelegate?.Invoke(itemView, _handler, "LoadBoardSystem");
        }
    }
}