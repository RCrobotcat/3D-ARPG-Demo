using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource SfxAudio;
    public AudioSource footStepSource;
    public AudioClip PlayerWalk_solid;
    public AudioClip PlayerWalk_soft;
    public AudioClip PlayerRun_solid;
    public AudioClip PlayerRun_soft;
    public AudioClip PlayerSlash;
    public AudioClip PlayerSlash_target;

    protected override void Awake()
    {
        base.Awake();
    }

    public void PlaySfx(AudioClip clip)
    {
        SfxAudio.PlayOneShot(clip);
    }

    public void PlayFootStep(AudioClip clip)
    {
        footStepSource.PlayOneShot(clip);
    }

    public void RandomPlaySfx(AudioClip clip)
    {
        int R = Random.Range(1, 11);
        if (R > 5)
        {
            SfxAudio.PlayOneShot(clip);
        }
    }

    public void DoubleRandomPlaySfx(AudioClip clip, AudioClip clip02)
    {
        int R = Random.Range(1, 11);
        if (R > 5)
        {
            SfxAudio.PlayOneShot(clip);
        }
        else
        {
            SfxAudio.PlayOneShot(clip02);
        }
    }
}