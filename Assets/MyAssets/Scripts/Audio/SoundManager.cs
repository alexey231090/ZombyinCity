using UnityEngine;
using UniRx;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] FireAudio;
    public AudioSource[] ReloadAudio;
    public AudioSource SelectOther;
    public AudioSource UppAmmo;
    public AudioClip[] Steps;
    public AudioSource stepsSource;
    public AudioSource[] JumpSurce;
    public AudioSource aScreamFromBlow;
    public AudioSource noAmmoSound;
    public AudioSource onTarget;
    public AudioSource backGroundMusic;



    CompositeDisposable _disposable = new();


    public static readonly Subject<AudioSource[]> fireSound = new();
    public static readonly Subject<AudioSource[]> ReloadSound = new();

    private void Awake()
    {
        fireSound.OnNext(FireAudio);
        ReloadSound.OnNext(ReloadAudio);

    }
}
