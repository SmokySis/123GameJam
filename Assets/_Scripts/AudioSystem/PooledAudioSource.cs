using System;
using System.Collections;
using UnityEngine;
namespace AudioSystem
{
    [DisallowMultipleComponent, RequireComponent(typeof(AudioSource))]
    public class PooledAudioSource : MonoBehaviour
    {
        public AudioSource Source { get; private set; }
        public string EventID { get; private set; }
        public int Token { get; private set; }
        private Action<PooledAudioSource> _onFinished;
        private Coroutine _finishedCoroutine;
        private Transform _followTarget;
        private bool _followInUpdate;
        private AudioEvent _audioEvent;
        private bool _paused;
        private void Awake()
        {
            if (!TryGetComponent(out AudioSource source))
                Debug.LogError("PooledAudioSource Awake Error:Cant Find AudioSource");
            Source ??= source;
        }
        public void Init(AudioEvent audioEvent)
        {
            if (Source == null)
                Source = gameObject.AddComponent<AudioSource>();
            if (Source == null)
            {
                Debug.LogError("PooledAudioSource Init Error:Cant Find AudioSource");
                return;
            }
            ResetState();
            _audioEvent = audioEvent;
            EventID = audioEvent.id;
            if (audioEvent.clips == null)
            {
                Debug.LogWarning("PooledAudioSource Init Warning:Clips Of AudioEvent Is Null");
                return;
            }
            else
            {
                switch (audioEvent.playType)
                {
                    case ClipPlayType.随机:
                        Source.clip = audioEvent.PickClip(); break;
                    case ClipPlayType.只播放第一首:
                        Source.clip = audioEvent.clips[0]; break;
                    case ClipPlayType.从第一首开始连续播放:
                        Source.clip = audioEvent.clips[0]; break;
                }
            }
            Source.volume = Mathf.Clamp01(audioEvent.volume + UnityEngine.Random.Range(-audioEvent.volumeRandom, audioEvent.volumeRandom));
            Source.pitch = Mathf.Clamp(audioEvent.pitch + UnityEngine.Random.Range(-audioEvent.pitchRandom, audioEvent.pitchRandom), -3f, 3f);
            Source.loop = audioEvent.loop;
            if (audioEvent.is3D)
            {
                Source.spatialBlend = Mathf.Clamp01(audioEvent.spatialBlend);
                Source.minDistance = audioEvent.minDistance;
                Source.maxDistance = audioEvent.maxDistance;
                Source.rolloffMode = audioEvent.rolloffMode;
            }
            else
                Source.spatialBlend = 0;
        }
        public void Play(int token, Action<PooledAudioSource> onFinished, Transform followTarget = null, bool followInUpdate = false)
        {
            if (!Source)
            {
                Debug.LogError("PooledAudioSource Play Error:Cant Find AudioSource");
                return;
            }
            Token = token;
            _onFinished = onFinished;
            _followTarget = followTarget;
            _followInUpdate = followInUpdate;
            if (_finishedCoroutine != null)
            {
                StopCoroutine(_finishedCoroutine);
                _finishedCoroutine = null;
            }
            Source.Play();
            if (!Source.loop)
            {
                if (_audioEvent.playType != ClipPlayType.从第一首开始连续播放)
                    _finishedCoroutine = StartCoroutine(CoWaitFinish(token, true));
                else
                    _finishedCoroutine = StartCoroutine(CoContinueWaitFinish(token));
            }
        }
        private IEnumerator CoContinueWaitFinish(int token)
        {
            for (int i = 0; i < _audioEvent.clips.Count && token == Token; i++)
            {
                Source.clip = _audioEvent.clips[i];
                Source.Play();
                yield return StartCoroutine(CoWaitFinish(token, i == _audioEvent.clips.Count - 1));
            }
        }
        private IEnumerator CoWaitFinish(int token, bool isInvoke)
        {
            if (!Source)
            {
                Debug.LogError("PooledAudioSource CoWaitFinish Error:Cant Find AudioSource");
                yield break;
            }
            while (Token == token)
            {
                if (_paused) { yield return null; continue; }
                if (!Source || !Source.isPlaying) break;
                yield return null;
            }
            if (Token != token) yield break;
            if (isInvoke)
                _onFinished?.Invoke(this);
        }
        public void Pause(int token)
        {
            if (token != Token || _paused) return;
            _paused = true;
            Source?.Pause();
        }
        public void Resume(int token)
        {
            if (Token != token || !_paused) return;
            _paused = false;
            Source?.UnPause();
        }
        public void Stop(int token, bool invokeFinishedEvent)
        {
            if (Token != token) return;
            StopInternal(invokeFinishedEvent);
        }
        public void StopInternal(bool invokeFinished)
        {
            if (_finishedCoroutine != null)
            {
                StopCoroutine(_finishedCoroutine);
                _finishedCoroutine = null;
            }
            _paused = false;
            Source?.Stop();
            _followInUpdate = false;
            _followTarget = null;
            if (invokeFinished)
                _onFinished?.Invoke(this);
        }
        public void ResetState()
        {
            Source.Stop();
            EventID = null;
            Source.clip = null;
            Source.outputAudioMixerGroup = null;
            Source.loop = false;
            Source.volume = 1f;
            Source.pitch = 1f;

            Source.spatialBlend = 0f;
            Source.minDistance = 1f;
            Source.maxDistance = 25f;
            Source.rolloffMode = AudioRolloffMode.Logarithmic;

            Token = -1;
            _onFinished = null;
            _followTarget = null;
            _followInUpdate = false;
            _audioEvent = null;
            _paused = false;
            transform.SetParent(null);
        }
        private void LateUpdate()
        {
            if (!_followInUpdate || _followTarget == null) return;
            transform.position = _followTarget.position;
        }
    }
}

