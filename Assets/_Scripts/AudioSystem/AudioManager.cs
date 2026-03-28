using PoolSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utility;
namespace AudioSystem
{
    public class AudioManager : Singleton<AudioManager>
    {
        protected override bool _isDonDestroyOnLoad => true;
        public AudioLibrary AudioLibrary => _library;
        [SerializeField, Required, LabelText("播放管理")]
        private AudioLibrary _library;
        [SerializeField, LabelText("通道映射")]
        private AudioMixerRouter _router;
        [SerializeField, LabelText("AudioSource预制体")]
        private GameObject _prefab;
        private int _nextToken = 1;
        private readonly Dictionary<string, float> _lastPlayTime = new();
        private readonly Dictionary<string, int> _playingCount = new();
        private readonly Dictionary<int, PooledAudioSource> _active = new();
        public AudioMixerRouter Router => _router;
        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="id"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public AudioHandle Play(string id, Action<PooledAudioSource> onFinished = null) => PlayInternal(id, Vector3.zero, onFinished == null ? OnFinished : onFinished, null, force3D: false, followInUpdate: false);
        /// <summary>
        /// 播放3D音频
        /// </summary>
        /// <param name="id"></param>
        /// <param name="worldPos"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public AudioHandle Play3D(string id, Vector3 worldPos, Action<PooledAudioSource> onFinished = null) => PlayInternal(id, worldPos, onFinished == null ? OnFinished : onFinished, null, force3D: true, followInUpdate: false);
        /// <summary>
        /// 播放音频，同时发生器跟随对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="target"></param>
        /// <param name="onFinished"></param>
        /// <param name="followInUpdate"></param>
        /// <returns></returns>
        public AudioHandle PlayFollow(string id, Transform target, Action<PooledAudioSource> onFinished = null, bool followInUpdate = true) => PlayInternal(id, target ? target.position : Vector3.zero, onFinished == null ? OnFinished : onFinished, target, force3D: true, followInUpdate: followInUpdate);
        /// <summary>
        /// 停止音频
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="invokeEvent"></param>
        public void Stop(AudioHandle handle, bool invokeEvent = true)
        {
            if (handle.IsValid)
                handle.pooled.Stop(handle.token, invokeEvent);
        }
        /// <summary>
        /// 暂停音频
        /// </summary>
        /// <param name="handle"></param>
        public void Pause(AudioHandle handle)
        {
            if (handle.IsValid)
                handle.pooled.Pause(handle.token);
        }
        /// <summary>
        /// 恢复音频
        /// </summary>
        /// <param name="handle"></param>
        public void Resume(AudioHandle handle)
        {
            if (handle.IsValid)
                handle.pooled.Resume(handle.token);
        }
        public void PauseAll()
        {
            foreach (var kv in _active)
                kv.Value.Pause(kv.Key);
        }
        public void ResumeAll()
        {
            foreach (var kv in _active)
                kv.Value.Resume(kv.Key);
        }
        private AudioHandle PlayInternal(string id, Vector3 pos, Action<PooledAudioSource> onFinished, Transform follow, bool force3D, bool followInUpdate)
        {
            if (_library == null || string.IsNullOrEmpty(id) || !_library.TryGet(id, out var audioEvent) || audioEvent == null)
            {
                Debug.LogWarning($"AudioManager PlayInternal Warning:\n" +
                    $"Library Is Null:{_library == null}\n" +
                    $"ID Is Null Or Empty:{string.IsNullOrEmpty(id)}");
                return default;
            }
            if (audioEvent.coolTime > 0f)
            {
                float now = Time.unscaledTime;
                if (_lastPlayTime.TryGetValue(id, out float last) && now - last < audioEvent.coolTime)
                    return default;
            }
            if (audioEvent.maxConcurrent >= 0)
            {
                _playingCount.TryGetValue(id, out int count);
                if (count >= audioEvent.maxConcurrent)
                    return default;
            }
            if (audioEvent.clips == null || audioEvent.clips.Count == 0)
            {
                Debug.LogWarning("AudioManager PlayInternal Warning:Clips Of AudioEvent Is Null");
                return default;
            }
            GameObject go = GameObjectPoolCenter.Instance.GetInstance(_prefab, pos, Quaternion.identity, parent: null,
               onBeforSetActive: obj =>
               {
                   // 确保有 PooledAudioSource/AudioSource
                   var pooled = obj.GetComponent<PooledAudioSource>();
                   if (pooled == null) pooled = obj.AddComponent<PooledAudioSource>();

                   // 把 AudioEvent 应用到 AudioSource（见下面函数，字段逐个对应）
                   AudioMixerGroup outGroup = null;
                   if (audioEvent.autoAudioBus)
                   {
                       if (_router != null)
                       {
                           _router.TryGetGroup(audioEvent.bus, out outGroup);
                       }
                   }
                   else
                       outGroup = audioEvent.mixerGroup;
                   pooled.Source.outputAudioMixerGroup = outGroup;
                   // 如果不需要 update 跟随，可以直接挂 parent 跟随
                   if (follow != null && !followInUpdate)
                   {
                       obj.transform.SetParent(follow, true);
                       obj.transform.localPosition = Vector3.zero;
                   }
               });
            var pooledSrc = go.GetComponent<PooledAudioSource>();
            if (pooledSrc == null)
            {
                GameObjectPoolCenter.Instance.Release(go);
                return default;
            }
            int token = _nextToken++;
            _active[token] = pooledSrc;
            _playingCount.TryGetValue(id, out int cur);
            _playingCount[id] = cur + 1;
            pooledSrc.Init(audioEvent);
            pooledSrc.Play(token, onFinished, follow, followInUpdate);
            _lastPlayTime[id] = Time.unscaledTime;
            return new AudioHandle() { id = audioEvent.id, token = token, pooled = pooledSrc };
        }
        private void OnFinished(PooledAudioSource pooled)
        {
            if (pooled == null) return;

            // 移除 active token
            _active.Remove(pooled.Token);

            // 并发计数 -1
            string id = pooled.EventID;
            if (!string.IsNullOrEmpty(id) && _playingCount.TryGetValue(id, out int count))
            {
                count--;
                if (count <= 0) _playingCount.Remove(id);
                else _playingCount[id] = count;
            }

            // 清理状态
            pooled.ResetState();

            // 归还池
            GameObjectPoolCenter.Instance.Release(pooled.gameObject);
        }
    }
}
