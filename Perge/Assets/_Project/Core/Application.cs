using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Core.Chunks;
using Core.Chunks.Storage;
using Core.Chunks.Storage.Arrays;
using Core.Generals;
using Core.Graphics;
using Packing;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Core
{
    public class Application : MonoBehaviour
    {
        public static Stopwatch _stopwatch = new Stopwatch();

        public static void StartTimer()
        {
            _stopwatch.Start();
        }

        public static void StopTimer(string text)
        {
            Debug.Log(text + " : " + _stopwatch.ElapsedMilliseconds);
            _stopwatch.Stop();
            _stopwatch.Reset();
        }
        
        [RuntimeInitializeOnLoadMethod]
        static void OnRuntime()
        {
            BlockRegister.Register();
            Atlas.Prepare();
            
            AppLoader.Prepare();
        }
    }
}
