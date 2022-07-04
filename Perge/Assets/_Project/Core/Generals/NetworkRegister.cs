using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Core.Generals
{
    public class NetworkRegisterAttribute : Attribute
    {
    }
    
    public class PackageRegisterAttribute : Attribute
    {
        private static Dictionary<int, Type> _packageIDs = new Dictionary<int, Type>();
        private static Dictionary<Type, int> _idPackages = new Dictionary<Type, int>();

        public static int GetPackageID<T>() where T : IPackage
        {
            Type type = typeof(T);
            return _idPackages[type];
        }

        public static Type GetPackageType(int id)
        {
            return _packageIDs[id];
        }

        public static bool Equals<T>(int id) where T : IPackage
        {
            Type type1 = typeof(T);
            Type type2 = _packageIDs[id];

            return type1 == type2;
        }

        private static bool _isLoaded = false;
        
        public static void Register()
        {
            if (!_isLoaded)
            {
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(PackageRegisterAttribute))));

                int index = 0;

                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces();

                    foreach (var i in interfaces)
                    {
                        if (i == typeof(IPackage))
                        {
                            Debug.Log($"[PackageRegister] {type.Name}");

                            _packageIDs.Add(index, type);
                            _idPackages.Add(type, index);

                            index += 1;
                        }
                    }
                }
                
                _isLoaded = true;
            }
        }
    }
}