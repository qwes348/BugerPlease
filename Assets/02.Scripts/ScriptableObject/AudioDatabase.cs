using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Scriptable Objects/AudioDatabase")]
public class AudioDatabase : ScriptableObject
{
    [System.Serializable]
    public class AudioEntry
    {
        public string addressableKey; // 어드레서블 키
        public AudioCategory category;
        [NaughtyAttributes.ShowIf("category", AudioCategory.BGM)]
        [AllowNesting]
        public Define.Bgm bgmType = Define.Bgm.None;
        [NaughtyAttributes.ShowIf("category", AudioCategory.SFX)]
        [AllowNesting]
        public Define.Sfx sfxType = Define.Sfx.None;
    }

    public enum AudioCategory { BGM, SFX };
    
    public List<AudioEntry> audioEntries = new List<AudioEntry>();

    public string GetAddressableKey(Define.Bgm bgmType)
    {
        var entry = audioEntries.Find(e => e.category == AudioCategory.BGM && e.bgmType == bgmType);
        return entry?.addressableKey;
    }
    
    public string GetAddressableKey(Define.Sfx sfxType)
    {
        var entry = audioEntries.Find(e => e.category == AudioCategory.SFX && e.sfxType == sfxType);
        return entry?.addressableKey;
    }
}
