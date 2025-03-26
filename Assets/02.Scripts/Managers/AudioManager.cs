using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AudioManager
{
    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioDatabase audioDb;

    public void Init()
    {
        bgmSource.volume = Managers.SaveLoad.localSaveData.BGMVolume;
        bgmSource.loop = true;
        bgmSource.playOnAwake = true;
        
        sfxSource.volume = Managers.SaveLoad.localSaveData.SFXVolume;
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        
        audioDb = Resources.Load<AudioDatabase>("Data/AudioDatabase");
    }

    public void SetAudioSource(AudioSource bgm, AudioSource sfx)
    {
        bgmSource = bgm;
        sfxSource = sfx;
    }

    public async UniTask PlayBgm(Define.Bgm bgm)
    {
        string address = audioDb.GetAddressableKey(bgm);
        if (address == null)
        {
            Debug.LogError($"BGM 없음 : {bgm.ToString()}");
            return;
        }
        var clip = await Managers.Resource.LoadAsset<AudioClip>(address);

        if (bgmSource.isPlaying)
        {
            await bgmSource.DOFade(0f, 0.5f);
            bgmSource.clip = clip;
            bgmSource.Play();
            await bgmSource.DOFade(Managers.SaveLoad.localSaveData.BGMVolume, 0.5f);
        }
        else
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    public async UniTask PlaySfx(Define.Sfx sfx)
    {
        string address = audioDb.GetAddressableKey(sfx);
        if (address == null)
        {
            Debug.LogError($"SFX 없음 : {sfx.ToString()}");
            return;
        }
        var clip = await Managers.Resource.LoadAsset<AudioClip>(address);
        
        sfxSource.PlayOneShot(clip);
    }

    public void SetBgmPitch(int pitchLevel)
    {
        bgmSource.pitch = Define.BGMPitch[pitchLevel];
    }

    public void SetBgmVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
