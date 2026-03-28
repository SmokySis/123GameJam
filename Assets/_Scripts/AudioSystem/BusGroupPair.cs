using System;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

namespace AudioSystem
{
    [Serializable]
    public struct BusGroupPair
    {
        [LabelText("逻辑通道")]
        public AudioBus bus;
        [LabelText("输出通道")]
        public AudioMixerGroup mixerGroup;
        [LabelText("音量变量名")]
        public string volumeParam;
    }
}
