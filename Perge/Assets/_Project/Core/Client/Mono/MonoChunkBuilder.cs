using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Chunks;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Mono
{
    public class MonoChunkBuilder : MonoSingleton<MonoChunkBuilder>
    {
        private const int _jobQuote = 48;
        private const int _renderDistance = 16;

        private Dictionary<KeyIndex, MonoChunk> _monoChunks
            = new Dictionary<KeyIndex, MonoChunk>();

        private Stack<MonoChunk> _taskJob 
            = new Stack<MonoChunk>();

        private Camera _camera;

        private DistanceComparer _distanceComparer;
        
        private void Start()
        {
            _camera = FindObjectOfType<Camera>();
            _distanceComparer = new DistanceComparer(_camera.transform.position);
        }

        public void AddChunk(KeyIndex keyIndex, MonoChunk monoChunk)
        {
            if (!_monoChunks.ContainsKey(keyIndex))
                _monoChunks.Add(keyIndex, monoChunk);
        }

        private bool _inWork = false;
        
        private void FixedUpdate()
        {
            if (_inWork == false)
            {
                StartCoroutine(UpdateBuilders());
                _inWork = true;
            }
        }
        
        public class DistanceComparer : IComparer<MonoChunk>
        {
            private Vector3 _target;
 
            public DistanceComparer(Vector3 distanceToTarget)
            {
                UpdateTarget(distanceToTarget);
            }

            public void UpdateTarget(Vector3 position)
            {
                _target = position;
            }

            public int Compare(MonoChunk a, MonoChunk b)
            {
                var targetPosition = _target;
                return Vector3.Distance(a.GetPosition(), targetPosition).CompareTo(Vector3.Distance(b.GetPosition(), targetPosition));
            }
        }

        private List<MonoChunk> _chunksPooling 
            = new List<MonoChunk>();

        private MonoChunk[] GetRenderDistance(Vector3 cameraPosition)
        {
            _chunksPooling.Clear();
            
            int half = _renderDistance / 2;

            KeyIndex cameraIndex 
                = KeyIndex.ToChunkCoordinates((int)cameraPosition.x, (int)cameraPosition.z);
            
            for (int x = -half; x < half; x++)
            {
                for (int z = -half; z < half; z++)
                {
                    var key = new KeyIndex(cameraIndex.X + x, cameraIndex.Z + z);
                    if (_monoChunks.ContainsKey(key))
                    {
                        var chunk = _monoChunks[key];
                        _chunksPooling.Add(chunk);
                    }
                }
            }

            return _chunksPooling.ToArray();
        }


        private IEnumerator ClearQuote()
        {
            MonoChunk[] array = null;
            
            yield return MonoThreading.Instance.ThreadAsync(() =>
            {
                array = _monoChunks.Values.ToArray();
            });

            foreach (var chunk in array)
            {
                if (chunk.IsDead())
                {
                    _monoChunks.Remove(chunk.KeyIndex);
                    Destroy(chunk.gameObject);
                    yield return null;
                }
            }
        }

        private IEnumerator ThreadingQuote()
        {
            var cameraPosition = _camera.transform.position;
            
            _distanceComparer.UpdateTarget(cameraPosition);
            
            bool inWork = true;
            
            ThreadPool.QueueUserWorkItem((c) =>
            {
                var array = GetRenderDistance(cameraPosition);
                
                int count = array.Length;

                Array.Sort(array,0,count, _distanceComparer);
                
                int index = 0;
                
                for (int i = 0; i < count; i++)
                {
                    var chunk = array[i];

                    if (index < _jobQuote)
                    {
                        if (chunk.IsUpdated() == false)
                        {
                            _taskJob.Push(chunk);
                            index += 1;
                        }
                    }
                }

                inWork = false;
            });

            while (inWork == true)
            {
                yield return null;
            }
        }

        private IEnumerator ApplyVisibleMeshes()
        {
            bool inTask = false;
            var cameraPosition = _camera.transform.position;
            
            MonoChunk[] array = null;
            
            ThreadPool.QueueUserWorkItem((c) =>
            {
                array = GetRenderDistance(cameraPosition);
                inTask = true;
            });

            while (inTask == false)
            {
                yield return null;
            }

            int count = array.Length;

            for (int i = 0; i < count; i++)
            {
                var chunk = array[i];
                
                if (chunk.IsUpdated() == true && chunk.HasApply() == false)
                {
                    chunk.ClearLayer();
                    chunk.ApplyLayer();
                    
                    yield return null;
                }
                
                yield return null;
            }
        }

        private int workerIndex = 0;
        
        private IEnumerator UpdateBuilders()
        {
            workerIndex += 1;

            if (workerIndex % 2 == 0)
            {

                yield return ClearQuote();
                yield return ThreadingQuote();
                yield return StartThreading();
            }
            else
            {
                yield return ApplyVisibleMeshes();
            }

            _inWork = false;
        }

        private IEnumerator StartThreading()
        {
            if (_taskJob.Count != 0)
            {
                while (_taskJob.Count > 0)
                {
                    var chunk = _taskJob.Pop();

                    bool inWork = false;

                    ThreadPool.QueueUserWorkItem((c) =>
                    {
                        chunk.UpdateLayer();
                        inWork = true;
                    });

                    while (inWork == false)
                    {
                        yield return null;
                    }
                }
            }
        }
    }
}