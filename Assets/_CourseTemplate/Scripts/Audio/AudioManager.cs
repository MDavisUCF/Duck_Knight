
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource audioSource;
    public AudioSource musicSource;

    [Header("Combat Sounds")]
public AudioClip[] goblinDeathClips;
    public AudioClip shieldBlockClip;
    public AudioClip goblinAttackClip;
    public AudioClip fireballImpactClip;
    public AudioClip fireballLaunchClip;

    [Header("Egg Health Sounds")]
    public AudioClip eggCrackClip;
    public AudioClip eggBreakClip;

    [Header("Shield Sounds")]
    public AudioClip shieldActivateClip;
    public AudioClip shieldDeactivateClip;

    [Header("Round Sounds")]
    public AudioClip victoryClip;
    public AudioClip defeatClip;

    [Header("Interface Sounds")]
    public AudioClip inputModeClip;
    public AudioClip resetClip;

    [Header("Background Music")]
    public AudioClip backgroundMusic;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float soundVolume = 0.7f;

    [Range(0f, 1f)]
    public float musicVolume = 0.4f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void PlayShieldBlockSound()
    {
        PlaySound(shieldBlockClip);
    }

    public void PlayGoblinAttackSound()
    {
        PlaySound(goblinAttackClip);
    }

    public void PlayFireballImpactSound()
    {
        PlaySound(fireballImpactClip);
    }

    public void PlayEggCrackSound()
    {
        PlaySound(eggCrackClip);
    }

    public void PlayEggBreakSound()
    {
        PlaySound(eggBreakClip);
    }

    public void PlayShieldActivateSound()
    {
        PlaySound(shieldActivateClip);
    }

    public void PlayShieldDeactivateSound()
    {
        PlaySound(shieldDeactivateClip);
    }

    public void PlayVictorySound()
    {
        PlaySound(victoryClip);
    }

    public void PlayDefeatSound()
    {
        PlaySound(defeatClip);
    }

    public void PlayInputModeSound()
    {
        PlaySound(inputModeClip);
    }

    public void PlayResetSound()
    {
        PlaySound(resetClip);
    }

    // Compatibility with existing gameplay scripts.
    public void PlayScoreSound()
    {
        PlayGoblinDeathSound();
    }

    public void PlayDangerSound()
    {
        PlayFireballImpactSound();
    }

    public void PlayPropActivateSound()
    {
        PlayShieldActivateSound();
    }

    public void PlayPropDeactivateSound()
    {
        PlayShieldDeactivateSound();
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip, soundVolume);
    }
    public void PlayGoblinDeathSound()
{
    if (goblinDeathClips == null ||
        goblinDeathClips.Length == 0)
        return;

    AudioClip selectedClip = goblinDeathClips[
        Random.Range(0, goblinDeathClips.Length)
    ];

    PlaySound(selectedClip);
}

public void PlayFireballLaunchSound()
{
    PlaySound(fireballLaunchClip);
}
}
