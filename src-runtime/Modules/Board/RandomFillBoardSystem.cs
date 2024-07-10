#define LEOECSLITE_DI
using Client.MatchThree.Board.ECS;
using Leopotam.EcsLite;
using MatchThree.Rack;

namespace MatchThree.Rack.ECS.Systems
{
    /// <summary>
    /// Заполняет поле фишками в пустых клетках, исключая матчи
    /// </summary>
    public class RandomFillBoardSystem : IEcsRunSystem
    {
        private IBoardFacade _board;

        public RandomFillBoardSystem(IBoardFacade board, string handler)
        {
            _board = board;
        }

        public void Run(IEcsSystems systems)
        {
        }

        private void PushCallback(IItem item)
        {

            //////TODO - ItemViewProvider -> IItem
            //var itemView = new ItemViewProvider(item.GetID(), item.Position, item.Color, item.Type)
            //{
            //    IsInteract = false
            //};

            //_createItem?.Invoke(itemView, _handler,"RandomFillBoardSystem");
        }
    }

    //TO:DO - пока что сюда, потом это нужно привязать к весам.
    public class MyRandom
    {
        /// <summary>
        /// Отдает цвет, исключая цвет верхнего элемента и правого. 
        /// </summary>
        /// <param name="up"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static int GetRandomColor(int n, int[] x)
        {
            System.Random r = new System.Random();
            int result = r.Next(n - x.Length);

            for (int i = 1; i < x.Length; i++)
            {
                if (result < x[i])
                    return result;
                result++;
            }
            return result;
        }

        public static int GetRandomColor(int n, int up, int left)
        {

            //System.Random r = new System.Random();
            int result = UnityEngine.Random.Range(1, n + 1);

            if (result != up && result != left)
                return result;

            result = 1;

            for (int i = 1; i <= n; i++)
            {
                if (result != up && result != left)
                    return result;

                result++;
            }
            return result;
        }
    }

}


//private void CreateItemGlass(Int2 pos, int color)
//{
//    TypeItems typeItem = TypeItems.Glass;

//    // var enty_ColorChip = entityService.Value.CreateItemBlock(pos, color, typeItem);
//    _board.CreateItem(enty_ColorChip, pos);
//    PushView(enty_ColorChip, color, pos, typeItem);
//}

//private void CreateItemChip(Int2 pos, int color)
//{
//    TypeItems typeItem = TypeItems.Chip;
//    var enty_ColorChip = entityService.Value.CreateItemColorChip(pos, color, typeItem);
//    _board.CreateItem(enty_ColorChip, pos);
//    PushView(enty_ColorChip, color, pos, typeItem);
//}



//if (Colors[x, y] == 11)
//{
//    Colors[x, y] = 5;
//    typeItem = TypeItems.VerticalBomb;
//}

//if (Colors[x, y] == 10)
//{
//    Colors[x, y] = 10;
//    typeItem = TypeItems.ColorBomb;
//}

//if (Colors[x, y] == 12)
//{
//    Colors[x, y] = 3;
//    typeItem = TypeItems.LittleBomb;
//}

//if (Colors[x, y] == 16)
//{
//    CreateItemGlass(pos, 16);
//    int randColor = UnityEngine.Random.Range(1, 7);
//    CreateItemChip(pos, randColor);
//    continue;
//}

// enty_ColorChip = entityService.Value.CreateItemColorChip(pos, Colors[x, y], typeItem);

//Board.CreateItem(enty_ColorChip, pos);

//if (CreateItemdElement != null)
//    CreateItemdElement.Invoke(new ElementModel(enty_ColorChip, Colors[x, y], pos, typeItem), new ProviderIdentifier("Starting"));