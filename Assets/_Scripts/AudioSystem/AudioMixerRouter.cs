using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;
namespace AudioSystem
{
    [CreateAssetMenu(menuName = "AudioSystem/AudioMixerRouter", fileName = "AudioMixerRouter")]
    public class AudioMixerRouter : ScriptableObject
    {
        [SerializeField, LabelText("输出通道")]
        private AudioMixer _mixer;
        [SerializeField, LabelText("输出通道映射")]
        private List<BusGroupPair> _pairs = new();
        private Dictionary<AudioBus, BusGroupPair> _map;
        private readonly Dictionary<AudioBus, float> _muteRestoreDb = new();
        public AudioMixer Mixer => _mixer;
        private Dictionary<AudioBus, BusGroupPair> BuildMap()
        {
            if (_pairs == null)
            {
                Debug.LogError("AudioMixerRouter BuildMap Error:Pairs Is Null");
                return null;
            }
            Dictionary<AudioBus, BusGroupPair> dict = new();
            foreach (BusGroupPair busGroupPair in _pairs)
            {
                if (dict.ContainsKey(busGroupPair.bus))
                {
                    Debug.LogWarning("AudioMixerRouter BuildMap Error:There Are Same Bus");
                    continue;
                }
                if (!dict.TryAdd(busGroupPair.bus, busGroupPair))
                {
                    Debug.LogWarning("AudioMixerRouter BuildMap Error:Cant Add To Map");
                    continue;
                }
            }
            return dict;
        }
        private void OnEnable() => _map ??= BuildMap();
#if UNITY_EDITOR
        private void OnValidate()
        {
            _map = null;
        }
#endif
        public bool TryGetGroup(AudioBus bus, out AudioMixerGroup audioMixerGroup)
        {
            _map ??= BuildMap();
            if (_map != null && _map.TryGetValue(bus, out var pair) && pair.mixerGroup != null)
            {
                audioMixerGroup = pair.mixerGroup;
                return true;
            }
            audioMixerGroup = null;
            return false;
        }
        public bool TrySetBusVolume(AudioBus bus, float linear01)
        {
            _map ??= BuildMap();
            if (_map != null && _map.TryGetValue(bus, out var pair) && !string.IsNullOrEmpty(pair.volumeParam))
            {
                float db = LinearToDecibel01(linear01);
                Debug.Log("1113");
                return _mixer.SetFloat(pair.volumeParam, db);
            }
            return false;
        }
        public bool TryGetBusVolume(AudioBus bus, out float linear01)
        {
            _map ??= BuildMap();
            if (_map != null && _map.TryGetValue(bus, out var pair) && !string.IsNullOrEmpty(pair.volumeParam) && _mixer.GetFloat(pair.volumeParam, out float db))
            {
                linear01 = DecibelToLinear01(db);
                return true;
            }
            linear01 = 1f;
            return false;
        }
        public bool TrySetBusMute(AudioBus bus, bool mute)
        {
            _map ??= BuildMap();
            if (_map == null || !_map.TryGetValue(bus, out var pair) || string.IsNullOrEmpty(pair.volumeParam))
                return false;

            if (!_mixer.GetFloat(pair.volumeParam, out var curDb))
                return false;

            if (mute)
            {
                if (!_muteRestoreDb.ContainsKey(bus))
                    _muteRestoreDb[bus] = curDb;

                return _mixer.SetFloat(pair.volumeParam, -80f);
            }
            else
            {
                float restoreDb = _muteRestoreDb.TryGetValue(bus, out var db) ? db : 0f; // 默认恢复到 0dB 或你想要的值
                _muteRestoreDb.Remove(bus);
                return _mixer.SetFloat(pair.volumeParam, restoreDb);
            }
        }
        /// <summary>
        /// Unity Mixer 的音量通常用 dB 表示：0dB = 原始音量；负值衰减；-80dB≈静音
        /// </summary>
        public static float LinearToDecibel01(float linear01)
        {
            linear01 = Mathf.Clamp01(linear01);
            // 0 不能 log，给一个很小的值当静音下限
            if (linear01 <= 0.0001f) return -80f;
            return Mathf.Log10(linear01) * 20f; // 20*log10(linear)
        }
        public static float DecibelToLinear01(float db) => Mathf.Pow(10f, db / 20f);
    }
}
