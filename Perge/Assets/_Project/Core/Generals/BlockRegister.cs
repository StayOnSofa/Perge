using System;
using System.Collections.Generic;
using System.Linq;
using Core.World;
using UnityEngine;

namespace Core.Generals
{
    public class BlockRegisterAttribute : Attribute
    {
        public static void Invoke()
        {
            var methods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes()
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttributes(typeof(BlockRegisterAttribute), false).FirstOrDefault() != null));

            foreach (var method in methods)
            {
                var obj = Activator.CreateInstance(method.DeclaringType);
                method.Invoke(obj, null);
            }
        }
    }
    
    public static class BlockRegister
    {
        private static Dictionary<ushort, Block> _registeredBlocks 
            = new Dictionary<ushort, Block>();

        private static Dictionary<Type, Block> _fastSearch 
            = new Dictionary<Type, Block>();

        private static ushort _basicBlock = 0;
        private static ushort _specialBlock = ushort.MaxValue/2;
        private static ushort _tickBlock = ushort.MaxValue/2 + ushort.MaxValue/4;
        public static ushort Register<T>() where T : Block
        {
            ushort id = 0;

            if (typeof(T).IsSubclassOf(typeof(BlockMesh)))
            {
                id = RegisterBlock<T>(_specialBlock);
                _specialBlock += 1;
            }
            else
            {
                if (typeof(T).IsSubclassOf(typeof(BlockStructure)))
                {
                    id = RegisterBlock<T>(_tickBlock);
                    _tickBlock += 1;
                }
                else
                {
                    id = RegisterBlock<T>(_basicBlock);
                    _basicBlock += 1;
                }
            }

            return id;
        }

        public static bool IsAirOrSpecialBlock(ushort blockID)
        {
            return blockID == 0 || (blockID >= ushort.MaxValue / 2);
        }

        public static bool IsSpecialBlock(ushort blockID)
        {
            return (blockID >= ushort.MaxValue / 2);
        }

        public static bool IsTickBlock(ushort blockID)
        {
            return (blockID >= (ushort.MaxValue/2 + ushort.MaxValue/4));
        }

        public static ushort RegisterBlock<T>(ushort id) where T : Block
        {
            bool inRegister = false;
            
            foreach (var block in _registeredBlocks.Values)
            {
                if (block is T)
                {
                    id = block.GetBlockID();
                    
                    inRegister = true;
                    break;
                }
            }

            if (!inRegister)
            {
                var block = (T)Activator.CreateInstance(typeof(T));
                
                block.Init(id);

                _registeredBlocks.Add(id, block);
                _fastSearch.Add(typeof(T), block);
            }

            return id;
        }

        public static Block GetBlock<T>() where T : Block
        {
            var type = typeof(T);
            if (_fastSearch.ContainsKey(type))
            {
                return _fastSearch[type];   
            }

            return null;
        }

        public static Block GetBlock(ushort id)
        {
            if (_registeredBlocks.ContainsKey(id))
            {
                return _registeredBlocks[id];
            }

            return null;
        }

        private static bool _isLoaded = false;

        public static void Register()
        {
            if (!_isLoaded)
            {
                BlockRegisterAttribute.Invoke();
                _isLoaded = true;
            }
        }

        public static IEnumerable<Block> GetRegisteredBlocks()
        {
            Register();
            return _registeredBlocks.Values;
        }
    }
}