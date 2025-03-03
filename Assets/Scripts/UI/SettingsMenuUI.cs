using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;      // UIスライダーの参照
    [SerializeField] private string exposedParameter = "MasterVolume";
    private void Start()
    {
        audioMixer = SoundManager.Instance.audioMixer;
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        float currentVolume;
        if (audioMixer.GetFloat(exposedParameter, out currentVolume))
        {
            volumeSlider.value = currentVolume;
        }
    }

    public void OnVolumeChanged(float value)
    {
        // AudioMixerのパラメータを更新
        float dB = value > 0 ? Mathf.Log10(value) * 20 : -80;
        audioMixer.SetFloat(exposedParameter, dB);
        Debug.Log($"volume = {value}");
    }
}
