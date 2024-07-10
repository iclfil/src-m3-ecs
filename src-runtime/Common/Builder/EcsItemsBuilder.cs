using MatchThree.Rack.ECS;
using MatchThree.Configs;
using Leopotam.EcsLite;
using MatchThree.Rack.Common;
using MatchThree.Rack.ECS.Tags;
using MatchThree.Rack.Items;
using MatchThree.Rack.Utils;

namespace MatchThree.Rack.ECS.Services
{
    public class EcsItemsBuilder
    {
        private EcsWorld _world { get; }

        private readonly EntityItemsFactory _flyBombFactory;
        private readonly EntityItemsFactory _chipFactory;
        private readonly EntityItemsFactory _littleBombFactory;
        private readonly EntityItemsFactory _colorBombFactory;
        private readonly EntityItemsFactory _stripedBombFactory;
        private readonly EntityItemsFactory _defaultSquareFactory;
        private readonly EntityItemsFactory _ingredientCreator;



        public EcsItemsBuilder(EcsWorld world)
        {
            _world = world;
            _flyBombFactory = new FlyBombFactory();
            _chipFactory = new ChipFactory();
            _littleBombFactory = new LittleBombFactory();
            _colorBombFactory = new ColorBombFactory();
            _stripedBombFactory = new StripedBombFactory();
            _defaultSquareFactory = new DefaultSquareFactory();
            _ingredientCreator = new IngredientFactory();
        }

        public int CreateEntity(IItem item)
        {
            if (ItemsProperties.IsBlock(item.Type))
                return CreateBlock(item);

            return CreateChip(item);
        }

        private int CreateChip(IItem item)
        {
            if (item.Type == TypeItems.Ingredient)
                return _ingredientCreator.CreateEntityByItem(item, _world);

            if (item.Type == TypeItems.FlyBomb)
                return _flyBombFactory.CreateEntityByItem(item, _world);

            if (item.Type == TypeItems.Bomb)
                return _littleBombFactory.CreateEntityByItem(item, _world);

            if (item.Type == TypeItems.ColorBomb)
                return _colorBombFactory.CreateEntityByItem(item, _world);

            if (item.Type == TypeItems.VerticalBomb || item.Type == TypeItems.HorizontalBomb)
                return _stripedBombFactory.CreateEntityByItem(item, _world);

            return _chipFactory.CreateEntityByItem(item, _world);
        }


        private int CreateBlock(IItem item)
        {
            if (item.Type == TypeItems.DefaultSquare)
                return _defaultSquareFactory.CreateEntityByItem(item, _world);

            var entity = _world.NewEntity();
            ref var itemData = ref _world.GetPool<ItemData>().Add(entity);
            itemData.Item = item;
            itemData.Entity = _world.PackEntity(entity);

            var pool1 = _world.GetPool<BlockTag>().Add(entity);

            if (item.Type == TypeItems.Slime)
                AddSlimeTag(entity);

            if (item.Type == TypeItems.Glass_1) //TODO ѕеределать в нормальное распределение тегов.
                CreateGlass(ref entity);

            if (item.Type == TypeItems.Cage)
                AddCageTag(entity);

            return entity;
        }

        private void AddCageTag(int entity)
        {
            _world.GetPool<CageTag>().Add(entity);
        }

        private void AddSlimeTag(int entity)
        {
            _world.GetPool<SlimeTag>().Add(entity);
        }


        private void CreateGlass(ref int entity)
        {
            var pool2 = _world.GetPool<GlassTag>().Add(entity);
        }

        private void AddChipTag(int entity)
        {
            _world.GetPool<ChipTag>().Add(entity);
        }
    }
}