using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    private bool isNight;
    
    [SerializeField] private float fadeDuration = 2f;

    private List<AudioSource> activeMusicSources = new List<AudioSource>();
    private Coroutine currentCrossfade;
    
    [SerializeField] private AudioMixer masterMixer;

    private Dictionary<AudioClip, MixerGroups> sounds = new Dictionary<AudioClip, MixerGroups>();
    private List<AudioSource> uiSources = new List<AudioSource>();
    private List<AudioSource> sfxSources = new List<AudioSource>();
    private List<AudioSource> ambianceSources = new List<AudioSource>();
    private List<AudioSource> musicSources = new List<AudioSource>();

    [Header("Sounds")] 
    [SerializeField] private AudioClip _uiClick;
    [SerializeField] private AudioClip _uiHover;
    [SerializeField] private AudioClip _menuAmbiance;
    [SerializeField] private AudioClip _missionAmbiance;
    [FormerlySerializedAs("completedJingle")] [SerializeField] private AudioClip _completedJingle;
    [FormerlySerializedAs("failJingle")] [SerializeField] private AudioClip _failJingle;
    [SerializeField] private AudioClip _transitionOn;
    [SerializeField] private AudioClip _transitionOff;
    [SerializeField] private AudioClip _meatGrinder;
    [SerializeField] private AudioClip _saw;
    [SerializeField] private AudioClip _liquid;
    [SerializeField] private AudioClip[] _pointTick;
    [SerializeField] private AudioClip[] _enemyDeath;
    [SerializeField] private AudioClip[] _unitDeath;
    [SerializeField] private AudioClip[] _liberatedDeath;

    public AudioClip UIClick => _uiClick;
    public AudioClip UIHover => _uiHover;
    public AudioClip MenuAmbiance => _menuAmbiance;
    public AudioClip MissionAmbiance => _missionAmbiance;
    public AudioClip CompletedJingle => _completedJingle;
    public AudioClip FailJingle => _failJingle;
    public AudioClip TransitionOn => _transitionOn;
    public AudioClip TransitionOff => _transitionOff;
    public AudioClip MeatGrinder => _meatGrinder;
    public AudioClip Saw => _saw;
    public AudioClip Liquid => _liquid;
    public AudioClip[] PointTick => _pointTick;
    public AudioClip[] EnemyDeath => _enemyDeath;
    public AudioClip[] UnitDeath => _unitDeath;
    public AudioClip[] LiberatedDeath => _liberatedDeath;


    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        PlayAmbiance(_menuAmbiance, 1f);
    }

    public void Play(AudioClip _clip, MixerGroups mixerGroup, Vector2 pitchRange = default, float volume = 1f, Vector3? _location = null, float _spatial = -1f, int _priority = 128, bool _loop = false) {
        AudioClip audioClip = _clip;
        if (audioClip == null) return;

        AudioSource source = GetAvailableAudioSource(mixerGroup);
        if (_location.HasValue) {
            source.gameObject.transform.position = _location.Value;
        }

        source.priority = _priority;
        source.loop = _loop;
        source.volume = volume;
        source.clip = audioClip;
        
        float _spatialBlend = _spatial < 0f ? (_location.HasValue ? 1f : 0f) : _spatial;
        source.spatialBlend = _spatialBlend;
        source.pitch = pitchRange == default ? 1f : Random.Range(pitchRange.x, pitchRange.y);
        source.Play();
    }
    
    public void Play(AudioClip[] _clip, MixerGroups mixerGroup, Vector2 pitchRange = default, float volume = 1f, Vector3? _location = null, int _priority = 128) {
        AudioClip audioClip = _clip[Random.Range(0, _clip.Length)];
        Play(audioClip, mixerGroup, pitchRange, volume, _location);
    }

    public AudioSource PlayAmbiance(AudioClip _clip, float fadeDuration, float volume = 1f, float pitch = 1f, bool solo = true) {
        AudioClip audioClip = _clip;
        if (audioClip == null) return null;

        AudioSource source = GetAvailableAudioSource(MixerGroups.Ambiance);
        source.volume = 0;
        source.pitch = pitch;
        source.clip = audioClip;
        source.loop = true;
        source.Play();

        StartCoroutine(FadeAudioSource(source, volume, fadeDuration));

        if (solo) {
            foreach (AudioSource src in ambianceSources) {
                if (src != source) {
                    StartCoroutine(FadeAudioSource(src, 0f, fadeDuration));
                }
            }
        }

        return source;
    }
    
    public void FadeOutAllMusic(float fadeOutDuration) {
        //Debug.Log($"Fading out all music");
        foreach (var source in musicSources) {
            StartCoroutine(FadeAudioSource(source, 0f, fadeOutDuration, true));
        }
    }

    private IEnumerator FadeAudioSource(AudioSource source, float targetVolume, float duration, bool stopAfterFade = false) {
        if (source == null) yield break;

        float time = 0;
        float startVolume = source.volume;

        while (time < duration) {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return new WaitForEndOfFrame();
        }

        source.volume = targetVolume;
        if (stopAfterFade && targetVolume == 0) {
            source.Stop();
            source.clip = null;
            musicSources.Remove(source);
        }
    }

    public void StopAmbiance(AudioClip _clip, float fadeOutDuration) {
        AudioClip audioClip = _clip;
        if (audioClip == null) return;

        AudioSource source = GetSpecificAudioSource(audioClip, MixerGroups.Ambiance);
        if (source == null) return;

        StartCoroutine(FadeAudioSource(source, 0f, fadeOutDuration));
    }

    private IEnumerator CrossfadeMusic(AudioSource newMusicSource) {
        float time = 0;

        while (time < fadeDuration) {
            time += Time.deltaTime;
            float ratio = time / fadeDuration;
            newMusicSource.volume = Mathf.Clamp01(Mathf.Lerp(0f, 0.5f, ratio));
            //Debug.Log($"Fading in {newMusicSource.clip} | {newMusicSource.volume}");
            foreach (var source in activeMusicSources) {
                if (source != newMusicSource) {
                    //Debug.Log($"Fading out {source.clip} | {source.volume}");
                    source.volume = Mathf.Clamp01(Mathf.Lerp(source.volume, 0f, ratio));
                }
            }
            yield return new WaitForEndOfFrame();
        }

        FinishCurrentFade(newMusicSource);
    }

    public void SetMusic(AudioClip _clip) {
        //Debug.Log($"Setting music to {name}");
        AudioClip nextClip = _clip;
        if (nextClip == null) return;

        AudioSource newMusicSource = GetAvailableAudioSource(MixerGroups.Music);
        newMusicSource.volume = 0f;
        newMusicSource.clip = nextClip;
        newMusicSource.Play();

        if (currentCrossfade != null) {
            StopCoroutine(currentCrossfade);
            currentCrossfade = null;
        }
        
        activeMusicSources.Add(newMusicSource);
        currentCrossfade = StartCoroutine(CrossfadeMusic(newMusicSource));

    }

    private void FinishCurrentFade(AudioSource newMusicSource) {
        for (int i = activeMusicSources.Count - 1; i >= 0; i--) {
            var source = activeMusicSources[i];
            if (source != newMusicSource && source.volume <= 0) {
                //Debug.Log($"Removing {source.clip}");
                source.clip = null;
                activeMusicSources.RemoveAt(i);
            }
        }
    }

    private List<AudioSource> GetMixerGroupSources(MixerGroups mixerGroup) {
        return mixerGroup switch {
            MixerGroups.SFX => sfxSources,
            MixerGroups.Ambiance => ambianceSources,
            MixerGroups.UI => uiSources,
            MixerGroups.Music => musicSources,
            _ => new List<AudioSource>()
        };
    }

    private AudioSource GetAvailableAudioSource(MixerGroups mixerGroup) {
        List<AudioSource> currentAudioSources = GetMixerGroupSources(mixerGroup);
        foreach (AudioSource source in currentAudioSources) {
            if (!source.clip || !source.isPlaying || source.volume == 0f) return source;
        }

        GameObject _go = new GameObject(mixerGroup+ " Audio");
        _go.transform.SetParent(transform);
        AudioSource newSource = _go.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        AudioMixerGroup refMixerGroup = GetMixerGroup(mixerGroup);
        if (refMixerGroup != null) {
            newSource.outputAudioMixerGroup = refMixerGroup;
        }

        currentAudioSources.Add(newSource);
        return newSource;
    }

    private AudioSource GetSpecificAudioSource(AudioClip clip, MixerGroups mixerGroup) {
        List<AudioSource> currentAudioSources = GetMixerGroupSources(mixerGroup);
        foreach (AudioSource source in currentAudioSources) {
            if (source.clip == clip) return source;
        }

        return null;
    }
    
    public AudioMixerGroup GetMixerGroup(MixerGroups _mixer) {
        AudioMixerGroup[] _mixerGroups = masterMixer.FindMatchingGroups(_mixer.ToString());
        if (_mixerGroups.Length > 0) {
            return _mixerGroups[0];
        } else {
            Debug.LogError($"No mixer group found for {_mixer.ToString()}");
            return null;
        }
    }

    public void UpdateVolume(string _reference, string _value) {
        if (!masterMixer) {
            return;
        }

        if (Settings.Instance.SetSetting(_reference, _value) && float.TryParse(_value, out float _result)) {
            if(masterMixer) masterMixer.SetFloat(_reference, Mathf.Log10((_result-1)/100f) * 20);
        } else {
            Debug.LogError($"Unable to set volume for {_reference}. Setting not found or unable to parse value.");
        }
    }

    public void ClearGameSounds() {
        foreach (AudioSource _as in sfxSources) {
            Destroy(_as.gameObject);
        }
    }


    private void OnDestroy() {
        StopAllCoroutines();
    }
}

public enum MixerGroups {
    UI,
    SFX,
    Music,
    Ambiance
}
