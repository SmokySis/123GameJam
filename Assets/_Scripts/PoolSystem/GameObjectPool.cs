using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
namespace PoolSystem
{
    public class GameObjectPool : IReference<GameObjectPool>
    {
        private int _poolID;
        private GameObject _prefab;
        private LinkedList<GameObject> _availableInstances;
        private GameObject _root;

        public void Init(GameObject prefab, int poolID, GameObject root, int initialCapacity = 8)
        {
            this._poolID = poolID;
            this._prefab = prefab;
            this._root = root;

            _availableInstances = new();
            Expand(initialCapacity);
        }
        private void Expand(int initialCapacity = 1)
        {
            for (int i = 0; i < initialCapacity; i++)
            {
                GameObject tempGO = GameObject.Instantiate(_prefab, _root.transform);
                tempGO.AddComponent<BelongToPoolIDMarker>().PoolID = _poolID;
                tempGO.SetActive(false);
                _availableInstances.AddLast(tempGO);
            }
        }
        public GameObject GetGameObject(Vector3 pos, Quaternion quaternion, Transform parent = null, Action<GameObject> onBeforeSetActive = null)
        {
            if (_availableInstances.Count == 0)
                Expand();
            LinkedListNode<GameObject> tempNode = _availableInstances.Last;
            _availableInstances.RemoveLast();
            GameObject tempGo = tempNode.Value;
            tempGo.transform.position = pos;
            tempGo.transform.rotation = quaternion;
            if (parent)
                tempGo.transform.SetParent(parent);
            onBeforeSetActive?.Invoke(tempGo);
            tempGo.SetActive(true);
            return tempGo;
        }
        public void ReleaseInstance(GameObject instance, Action<GameObject> onAfterSetActive = null)
        {
            instance.SetActive(false);
            instance.transform.SetParent(_root.transform);
            onAfterSetActive?.Invoke(instance);
            _availableInstances.AddLast(instance);
        }
        #region IReference
        public uint ReferenceType => ReferenceTypes.GAMEOBJECTPOOL;
        int IReference.IndexInReferencePool { get; set; }
        public void OnRecycle()
        {
            _availableInstances.Clear();
            _prefab = null;
        }
        public void Dispose()
        {
            OnRecycle();
            _availableInstances = null;
        }
        public IReference GetNewInstance() => new GameObjectPool() { _availableInstances = new(), _prefab = null, _root = null };
        #endregion
    }
}
