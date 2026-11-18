using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private List<AudioSource> _effectSources = new();

    [SerializeField] private AudioSource _sourceMusic;
    [SerializeField] private AudioSource _sourceEffects;

    [SerializeField] private AudioClip _loveColoredMasterSpark;
    public AudioClip LoveColoredMasterSpark { get => _loveColoredMasterSpark; }

    [SerializeField] private AudioClip _playerShoot;
    public AudioClip PlayerShoot { get => _playerShoot; }
    [SerializeField] private AudioClip _playerDead;
    public AudioClip PlayerDead { get => _playerDead; }

    [SerializeField] private AudioClip _enemyHit;
    public AudioClip EnemyHit { get => _enemyHit; }

    [SerializeField] private AudioClip _enemyDead;
    public AudioClip EnemyDead { get => _enemyDead; }

    void Awake()
    {
        Instance = this;

        _effectSources.Add(_sourceEffects);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        _effectSources.Clear();
    }

    void Start()
    {
        _sourceMusic.clip = LoveColoredMasterSpark;
        _sourceMusic.Play();
    }

    public void PlayEffect(AudioClip clip, float volume = 0.15f)
    {
        foreach (var source in _effectSources)
        {
            if (!source.isPlaying)
            {
                source.volume = volume;
                source.PlayOneShot(clip);
                return;
            }
        }

        AudioSource temp = gameObject.AddComponent<AudioSource>();
        temp.volume = volume;
        temp.PlayOneShot(clip);

        _effectSources.Add(temp);
    }
}
