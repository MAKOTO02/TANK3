using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [System.Serializable]
    public class SoundData
    {
        public string name;
        public AudioClip audioClip;
    }

    [SerializeField]
    private List<SoundData> soundDatas = new();

    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] AudioMixerGroup targetMixerGroup;

    //AudioSource（スピーカー）を同時に鳴らしたい音の数だけ用意
    static private AudioSource[] audioSourceList = new AudioSource[20];

    //別名(name)をキーとした管理用Dictionary
    static private Dictionary<string, SoundData> soundDictionary = new Dictionary<string, SoundData>();

    protected override void Awake()
    {
        base.Awake();
        //auidioSourceList配列の数だけAudioSourceを自分自身に生成して配列に格納
        for (var i = 0; i < audioSourceList.Length; ++i)
        {
            audioSourceList[i] = gameObject.AddComponent<AudioSource>();
            audioSourceList[i].outputAudioMixerGroup = targetMixerGroup;
        }

        soundDictionary.Clear();
        //soundDictionaryにセット
        foreach (var soundData in soundDatas)
        {
            soundDictionary.Add(soundData.name, soundData);
        }
    }

    //未使用のAudioSourceの取得 全て使用中の場合はnullを返却
    static private AudioSource GetUnusedAudioSource()
    {
        for (var i = 0; i < audioSourceList.Length; ++i)
        {
            if (audioSourceList[i].isPlaying == false) return audioSourceList[i];
        }

        return null; //未使用のAudioSourceは見つかりませんでした
    }

    //指定されたAudioClipを未使用のAudioSourceで再生
    static public void Play(AudioClip clip)
    {
        var audioSource = GetUnusedAudioSource();
        if (audioSource == null) return; //再生できませんでした
        audioSource.clip = clip;
        audioSource.Play();
    }

    //指定された別名で登録されたAudioClipを再生
    static public void Play(string name)
    {
        if (soundDictionary.TryGetValue(name, out var soundData)) //管理用Dictionary から、別名で探索
        {
            Play(soundData.audioClip); //見つかったら、再生
        }
        else
        {
            Debug.LogWarning($"その別名は登録されていません:{name}");
        }
    }
}