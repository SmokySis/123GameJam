using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace AudioSystem
{
    [CreateAssetMenu(menuName = "AudioSystem/AudioLibrary", fileName = "AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        [SerializeField, LabelText("音频事件")]
        private List<AudioEvent> _events = new();
        private Dictionary<string, AudioEvent> _map;
        public IReadOnlyList<AudioEvent> Events => _events;
        public bool TryGet(string id, out AudioEvent audioEvent)
        {
            if (string.IsNullOrEmpty(id))
            {
                audioEvent = null;
                return false;
            }
            _map ??= BuildMap();
            return _map.TryGetValue(id, out audioEvent);
        }
        private Dictionary<string, AudioEvent> BuildMap()
        {
            Dictionary<string, AudioEvent> dict = new();
            foreach (var audio in _events)
            {
                if (audio == null)
                {
                    Debug.LogWarning("AudioLibrary BuildMap Warning:There Is That Same AudioEvent Is Null");
                    continue;
                }
                if (string.IsNullOrEmpty(audio.id))
                {
                    Debug.LogWarning("AudioLibrary BuildMap Warning:Wrong ID");
                    continue;
                }
                if (dict.ContainsKey(audio.id))
                {
                    Debug.LogWarning("AudioLibrary BuildMap Warning:Same ID");
                    continue;
                }
                if (!dict.TryAdd(audio.id, audio))
                    Debug.LogWarning("AudioLibrary BuildMap Warning:Cant Add");
            }
            return dict;
        }
#if UNITY_EDITOR
        private void OnValidate() => _map = null;// 编辑器下每次改动都刷新字典，避免运行时取不到最新
#endif
    }
}
