using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;

namespace WibeSoft.Core.Managers
{
    public class PoolManager : SingletonBehaviour<PoolManager>
    {
        private Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
        private LogManager _logger => LogManager.Instance;

        public async UniTask Initialize()
        {
            _pools.Clear();
            _logger.LogInfo("PoolManager initialized", "PoolManager");
            await UniTask.CompletedTask;
        }

        public GameObject GetFromPool(string poolKey, GameObject prefab)
        {
            if (!_pools.ContainsKey(poolKey))
            {
                _pools[poolKey] = new Queue<GameObject>();
            }

            GameObject obj;
            if (_pools[poolKey].Count == 0)
            {
                obj = Instantiate(prefab);
                _logger.LogInfo($"Created new object for pool: {poolKey}", "PoolManager");
            }
            else
            {
                obj = _pools[poolKey].Dequeue();
                obj.SetActive(true);
                _logger.LogInfo($"Reused object from pool: {poolKey}", "PoolManager");
            }

            return obj;
        }

        public void ReturnToPool(string poolKey, GameObject obj)
        {
            if (!_pools.ContainsKey(poolKey))
            {
                _pools[poolKey] = new Queue<GameObject>();
            }

            obj.SetActive(false);
            _pools[poolKey].Enqueue(obj);
            _logger.LogInfo($"Returned object to pool: {poolKey}", "PoolManager");
        }
    }
} 