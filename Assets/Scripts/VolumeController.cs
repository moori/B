using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class VolumeController : MonoBehaviour
{
    public AudioMixerGroup group;
    public Slider slider;
    public TextMeshProUGUI text;

    private void OnEnable()
    {
        slider.value = PlayerPrefs.GetFloat(group.name, group.name=="BGM" ? 0.75f : 1f);
        text.text = $"{ (int)(100 * slider.value)}%";
    }

    public void OnValueChange(float value)
    {
        PlayerPrefs.SetFloat(group.name, value);
        text.text = $"{ (int)(100 * slider.value)}%";
        group.audioMixer.SetFloat(group.name, VolumeController.LinearToDecibel(value));
    }
    public static float LinearToDecibel(float linear)
    {
        float dB;
        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;
        return dB;
    }
}
