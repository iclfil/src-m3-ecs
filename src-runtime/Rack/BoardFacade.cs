using System.Collections.Generic;
using Client.Game.Utils;
using Leopotam.EcsLite;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Utils;
using MatchThree.Rack.Utils;

namespace MatchThree.Rack.ECS
{
    public interface IBoardFacade : IBoard
    {
        #region Board
        public void Swap(IItem firstItem, IItem secondItem);
        bool Has(IItem item);
        public List<IItem> GetItems(int x, int y);
        #endregion
        public ISlot[,] GetSlots();
        public bool HasTeleportDestinationInSlot(Int2 pos);
        public bool HasTeleportSourceInSlot(Int2 pos);
        public Int2 GetTeleportSourceFromSlot(Int2 pos);
        public Int2 GetTeleportDestinationFromSlot(Int2 pos);

        //Behaviors
        public bool HasBehavior<T>(IItem item) where T : struct;
        public void SwapBehavior(IItem item, bool enable);
        public void MatchBehavior(IItem item, bool b);
        public void FallBehavior(IItem item, bool b);
        public void AddTag<T>(IItem underItem) where T : struct;
        public bool HasTag<T>(IItem item) where T : struct;
        public void RemoveTag<T>(IItem blockedItem) where T : struct;
    }

    public class BoardFacade : IBoardFacade
    {
        public int[] Colors => _board.Colors;
        public int Width { get; set; }
        public int Height { get; set; }
        public ISlot[,] Slots => _board.Slots;

        private readonly IBoard _board;
        private EcsBoardHelper _boardHelper;
        private EcsWorld _world;

        public BoardFacade(int width, int height, EcsWorld world, IBoard board)
        {
            Width = width;
            Height = height;
            _board = board;
            _world = world;
            _boardHelper = new EcsBoardHelper(_world);

            LoadItemsInEcsWorld();
        }

        private void LoadItemsInEcsWorld()
        {
            foreach (var slot in _board.Slots)
                foreach (var item in slot.Items)
                    CreateEntity(item);
        }

        /// <summary>
        /// Создает IItem и для него же присваивает ID(Entity from World).
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="color"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IItem CreateItem(Int2 pos, int color, TypeItems type)
        {
            var boardItem = _board.CreateItem(pos, color, type);
            CreateEntity(boardItem);
            return boardItem;
        }

        public void DeleteIncludeItem(IItem deleteItem)
        {
            var id = deleteItem.GetID();
            _board.DeleteIncludeItem(deleteItem); //TODO - тут скрытая реализация, Item проверяется на вложенные, и если их нет, только тогда удаляется из слота.
            //Если Item Стал пустым, удаляем его из мира
            if (deleteItem.HasIncludeItems() == false)
                DeleteEntity(deleteItem);
        }

        public void ForceDelete(IItem item)
        {
            _board.ForceDelete(item);
        }

        private void CreateEntity(IItem item)
        {
            _boardHelper.CreateEcsItem(item);

            if (ItemsProperties.IsDice(item.Type))
            {
                SwapBehavior(item, true);
                FallBehavior(item, true);
            }
        }

        private void DeleteEntity(IItem item)
        {
            _boardHelper.DeleteEcsItem(item);
        }

        public ISlot[,] GetSlots()
        {
            return _board.Slots;
        }

        public bool HasBehavior<T>(IItem item) where T : struct
        {
            return _boardHelper.HasBehavior<T>(item);
        }

        public void SwapBehavior(IItem item, bool enable)
        {
            _boardHelper.SwapBehavior(item, enable);
        }

        public void MatchBehavior(IItem item, bool enable)
        {
            _boardHelper.MatchBehavior(item, enable);
        }

        public void FallBehavior(IItem item, bool enable)
        {
            _boardHelper.FallBehavior(item, enable);
        }

        public void AddTag<T>(IItem underItem) where T : struct
        {
            _boardHelper.AddTag<T>(underItem);
        }

        public bool HasTag<T>(IItem item) where T : struct
        {
            return _boardHelper.HasTag<T>(item);
        }

        public void RemoveTag<T>(IItem blockedItem) where T : struct
        {
            _boardHelper.RemoveTag<T>(blockedItem);
        }

        public List<IItem> GetItems(int x, int y)
        {
            return _board.Slots[x, y].Items;
        }

        public void Swap(IItem firstItem, IItem secondItem)
        {
            Int2 firstSlotPos = firstItem.Position;
            Int2 secondSlotPos = secondItem.Position;
            var firstSlot = _board.Slots[firstSlotPos.X, firstSlotPos.Y];
            firstSlot.RemoveItem(firstItem);
            var secondSlot = _board.Slots[secondSlotPos.X, secondSlotPos.Y];
            secondSlot.RemoveItem(secondItem);
            firstItem.Position = secondSlotPos;
            secondItem.Position = firstSlotPos;
            firstSlot.AddItem(secondItem);
            secondSlot.AddItem(firstItem);
        }

        public bool HasPosition(int x, int y)
        {
            return _board.HasPosition(x, y);
        }

        public bool TryGetLastItem(Int2 findPos, out IItem item)
        {
            item = null;
            if (_board.TryGetLastItem(findPos, out var lastItem))
            {
                item = lastItem;
                return true;
            }

            return false;
        }

        public bool HasItem(IItem item)
        {
            throw new System.NotImplementedException();
        }

        public bool HasPosition(Int2 pos)
        {
            return _board.HasPosition(pos);
        }

        public bool IsFreeSlot(int x, int y) => _board.IsFreeSlot(x, y);
        public bool IsEmptySlot(int x, int y) => _board.IsEmptySlot(x, y);
        public void SetSpawnInSlot(int x, int y, bool spawn)
        {
            throw new System.NotImplementedException();
        }

        public bool HasSpawnInSlot(Int2 pos)
        {
            throw new System.NotImplementedException();
        }

        public void AddTeleport(Int2 start, Int2 finish)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveTeleport(Int2 start)
        {
            throw new System.NotImplementedException();
        }

        public bool HasTeleportInSlot(Int2 start)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool HasTeleportDestinationInSlot(Int2 pos)
        {
            return _board.Slots[pos.X, pos.Y].TeleportDestination.IsEmpty == false;
        }

        public bool HasTeleportSourceInSlot(Int2 pos)
        {
            return _board.Slots[pos.X, pos.Y].TeleportSource.IsEmpty == false;
        }

        public Int2 GetTeleportSourceFromSlot(Int2 pos)
        {
            return _board.Slots[pos.X, pos.Y].TeleportSource;
        }

        public Int2 GetTeleportDestinationFromSlot(Int2 pos)
        {
            return _board.Slots[pos.X, pos.Y].TeleportDestination;
        }


        public bool IsSpawnSlot(int x, int y) => _board.IsSpawnSlot(x, y);
        public void Move(IItem moveItem, Int2 targetPos)
        {
            _board.Move(moveItem, targetPos);
        }

        public bool Has(IItem item) => _board.HasItem(item);
        public bool TryGetItemUnder(IItem matchedItem, out IItem underItem)
        {
            underItem = null;

            if (_board.TryGetItemUnder(matchedItem, out var f))
            {
                underItem = f;
                return true;
            }

            return false;
        }
    }
}