using System.Collections.Generic;
using MatchThree.Rack;
using MatchThree.Rack.ECS;
using FilConsole;
using Leopotam.EcsLite;
using MatchThree.Rack.ECS.Behaviors;
using MatchThree.Rack.ECS.Services;
using UnityEngine;

namespace MatchThree.Rack.ECS.Utils
{
    public class EcsBoardHelper
    {
        public EcsWorld World { get; }

        private EcsItemsBuilder _builder;

        private Dictionary<int, EcsPackedEntity> _entities;

        public EcsBoardHelper(EcsWorld world)
        {
            World = world;
            _builder = new EcsItemsBuilder(World);
            _entities = new Dictionary<int, EcsPackedEntity>();
        }

        /// <summary>
        /// Создает Сущность с ссылкой на Item в мире Items
        /// </summary>
        /// <param name="item"></param>
        public void CreateEcsItem(IItem item)
        {
            var entity = _builder.CreateEntity(item);
            var packEntity = World.PackEntity(entity);
            var key = item.GetID();
            _entities.Add(key, packEntity);
            //MyLog.Log("Create Ecs Item. Counts Entities" + _entities.Count, Color.green);
        }

        public void DeleteEcsItem(IItem item)
        {
            var key = item.GetID();
            var packEntity = _entities[key];

            if (packEntity.Unpack(World, out var entity))
            {
                World.DelEntity(entity);
            }
            _entities.Remove(key);
            //MyLog.Log("Delete Entity. Counts Entities" + _entities.Count, Color.blue);
        }

        public bool HasBehavior<T>(IItem item) where T : struct
        {
            var key = item.GetID();

            if (_entities.ContainsKey(key) == false)
                return false;

            var packedEntity = _entities[key];

            if (packedEntity.Unpack(World, out var entity))
            {
                if (World.GetPool<T>().Has(entity))
                    return true;
            }

            return false;
        }

        public void SwapBehavior(IItem item, bool enable)
        {
            var key = item.GetID();
            var packedEntity = _entities[key];

            if (packedEntity.Unpack(World, out var entity))
            {
                if (enable)
                {
                    World.GetPool<SwapBehavior>().Add(entity);
                }
                else
                {
                    World.GetPool<SwapBehavior>().Del(entity);
                }

                _entities[key] = World.PackEntity(entity);
            }
        }

        public void MatchBehavior(IItem item, bool enable)
        {
            var key = item.GetID();
            var packedEntity = _entities[key];

            if (packedEntity.Unpack(World, out var entity))
            {
                if (enable)
                {
                   World.GetPool<MatchBehavior>().Add(entity);
                }
                else
                    World.GetPool<MatchBehavior>().Del(entity);

                _entities[key] = World.PackEntity(entity);
            }
        }

        public void FallBehavior(IItem item, bool enable)
        {
            var key = item.GetID();
            var packedEntity = _entities[key];

            if (packedEntity.Unpack(World, out var entity))
            {
                if (enable)
                {
                    ref var data = ref World.GetPool<FallBehavior>().Add(entity);
                }
                else
                    World.GetPool<FallBehavior>().Del(entity);

                _entities[key] = World.PackEntity(entity);
            }
        }

        public void AddTag<T>(IItem item) where T : struct
        {
            var key = item.GetID();
            var packedEntity = _entities[key];

            if (packedEntity.Unpack(World, out var entity))
            {
                World.GetPool<T>().Add(entity);

                _entities[key] = World.PackEntity(entity);
            }
        }

        public bool HasTag<T>(IItem item) where T : struct
        {
            var key = item.GetID();

            if (_entities.ContainsKey(key) == false)
                return false;
            
            var packedEntity = _entities[key];

            if (packedEntity.Unpack(World, out var entity))
            {
                if (World.GetPool<T>().Has(entity))
                {
                    _entities[key] = World.PackEntity(entity);
                    return true;
                }
            }

            return false;
        }

        public void RemoveTag<T>(IItem item) where T : struct
        {
            var key = item.GetID();
            var packedEntity = _entities[key];

            if (packedEntity.Unpack(World, out var entity))
            {
                World.GetPool<T>().Del(entity);
                _entities[key] = World.PackEntity(entity);
            }
        }
    }
}