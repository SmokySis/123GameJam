using System.Collections.Generic;
using UnityEngine;
using Utility;
using System;
namespace PoolSystem
{
    public class GameObjectPoolCenter : Singleton<GameObjectPoolCenter>
    {
        protected override bool _isDonDestroyOnLoad => true;

        private int _poolCount = 0;
        private List<GameObjectPool> _pools = new();
        private Dictionary<int, int> _prefabID_PoolIDMap = new();
        /// <summary>
        /// 获取池化对象
        /// </summary>
        /// <param name="prefab">
        /// 池化对象的预制体
        /// </param>
        /// <param name="worldPosition">
        /// 生成对象的世界坐标
        /// </param>
        /// <param name="quaternion">
        /// 生成对象的四元数
        /// </param>
        /// <param name="parent">
        /// 生成对象的父物体(只有在第一次取用的时候作为父物体，如果在后续取用需要切换父物体需要设置changeParent=true)
        /// </param>
        /// <param name="onBeforSetActive">
        /// 在激活池化物体之前的事件
        /// </param>
        /// <param name="changeParent">
        /// 是否改变父物体
        /// </param>
        /// <returns>
        /// 被池化的物体
        /// </returns>
        public GameObject GetInstance(GameObject prefab, Vector3 worldPosition, Quaternion quaternion, Transform parent = null, Action<GameObject> onBeforSetActive = null, bool changeParent = false)
        {
            int id = prefab.GetInstanceID();
            GameObjectPool pool;
            if (!_prefabID_PoolIDMap.ContainsKey(id))
            {
                pool = new();
                GameObject newRoot = new GameObject($"GameObjectPoolRoot_{_poolCount}_{prefab.name}");
                newRoot.transform.SetParent(parent);
                pool.Init(prefab, _poolCount, newRoot);
                _pools.Add(pool);
                _prefabID_PoolIDMap.Add(id, _poolCount++);
            }
            else
                pool = _pools[_prefabID_PoolIDMap[id]];
            return pool.GetGameObject(worldPosition, quaternion, parent: changeParent ? parent : null, onBeforSetActive);
        }
        /// <summary>
        /// 回收池化对象
        /// </summary>
        /// <param name="gameObject">
        /// 被池化的对象(一定是之前从池子中取出来的对象才能归还)
        /// </param>
        public void Release(GameObject gameObject)
        {
            if (!gameObject)
            {
                Debug.LogError($"GameObjectPoolCenter Release Error:Cant Find GameObject");
                return;
            }
            if (!gameObject.TryGetComponent<BelongToPoolIDMarker>(out var marker))
            {
                Debug.LogError($"GameObjectPoolCenter Release Error:Cant Find Marker");
                return;
            }
            int id = marker.PoolID;
            if (id < 0 || id >= _poolCount)
            {
                Debug.LogError($"GameObjectPoolCenter Release Error:ID:{id} Is Out Of Range");
                return;
            }
            _pools[id].ReleaseInstance(gameObject);
        }
        public void OnDestroy()
        {
            foreach (var pool in _pools)
                pool?.Dispose();
            _pools.Clear();
            _prefabID_PoolIDMap.Clear();
            _pools = null;
            _prefabID_PoolIDMap = null;
        }
    }
}
