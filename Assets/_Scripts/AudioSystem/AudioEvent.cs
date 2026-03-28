using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
namespace AudioSystem
{
    [Serializable]
    public class AudioEvent
    {
        [BoxGroup("基础信息"), LabelText("ID")]
        public string id;
        [BoxGroup("基础信息"), LabelText("播放方式")]
        public ClipPlayType playType;
        [BoxGroup("基础信息"), LabelText("音频")]
        public List<AudioClip> clips = new();

        [BoxGroup("音频通道"), LabelText("自动")]
        public bool autoAudioBus = true;
        [BoxGroup("音频通道"), LabelText("输出通道"), ShowIf("@autoAudioBus")]
        public AudioBus bus = AudioBus.SFX;
        [BoxGroup("音频通道"), LabelText("输出通道"), ShowIf("@!autoAudioBus")]
        public AudioMixerGroup mixerGroup;

        [BoxGroup("基础播放参数"), LabelText("音量"), PropertyRange(0, 1)]
        public float volume = 1;
        [BoxGroup("基础播放参数"), LabelText("音调"), PropertyRange(-3, 3)]
        public float pitch = 1;
        [BoxGroup("基础播放参数"), LabelText("循环")]
        public bool loop = false;

        [FoldoutGroup("高级")]

        [BoxGroup("高级/随机扰动"), LabelText("音量扰动"), Tooltip("最终音量 = volume + Random(-volumeRandom, +volumeRandom)"),Min(0f)]
        public float volumeRandom = 0;
        [BoxGroup("高级/随机扰动"), LabelText("音调扰动"), Tooltip("最终音调 = pitch + Random(-pitchRandom, +pitchRandom)"), Min(0f)]
        public float pitchRandom = 0;
        [BoxGroup("高级/3D", showLabel: false), LabelText("3D")]
        public bool is3D = false;
        [BoxGroup("高级/3D", showLabel: false), ShowIf("@is3D"), LabelText("2D/3D 混合"), Tooltip("2D/3D 混合（0=纯2D，1=纯3D）"), PropertyRange(0, 1)]
        public float spatialBlend = 0;
        [BoxGroup("高级/3D", showLabel: false), ShowIf("@is3D"), LabelText("最小距离"), Tooltip("在这个距离内音量基本不衰减")]
        public float minDistance = 1;
        [BoxGroup("高级/3D", showLabel: false), ShowIf("@is3D"), LabelText("最大距离"), Tooltip("超过这个距离基本听不到"),
            InfoBox("最大值不能小于最小值",VisibleIf = "@maxDistance<minDistance",InfoMessageType =InfoMessageType.Error)]
        public float maxDistance = 25;
        [BoxGroup("高级/3D", showLabel: false), ShowIf("@is3D"), LabelText("距离衰减曲线")]
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        [BoxGroup("高级/限制"), LabelText("最大个数"), Tooltip("同一个 id 同时最多允许播放多少个，负数代表无限制")]
        public int maxConcurrent = -1;
        [BoxGroup("高级/限制"), LabelText("冷却时间"), Tooltip("同一个 id 的最短触发间隔（秒）")]
        public float coolTime = 0;
        /// <summary>
        /// 从歌单中随机获取
        /// </summary>
        /// <returns></returns>
        public AudioClip PickClip()
        {
            if (clips == null || clips.Count == 0) return null;
            return clips[UnityEngine.Random.Range(0, clips.Count)];
        }
    }
}
