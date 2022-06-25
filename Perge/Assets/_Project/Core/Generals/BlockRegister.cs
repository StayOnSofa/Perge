using System;
using System.Collections.Generic;
using System.Linq;
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

        public static ushort Register<T>() where T : Block
        {
            ushort id = 0;
            
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

               id = (ushort)_registeredBlocks.Count;
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