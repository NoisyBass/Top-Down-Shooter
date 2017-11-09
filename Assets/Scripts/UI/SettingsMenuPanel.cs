﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuPanel : MonoBehaviour {

    [SerializeField]
    private Slider volumeSlider;

	public void CloseSettings()
    {
        GameManager.Instance.CloseSettings();
    }

    public void ControllerSettingsChange(bool value)
    {
        GameManager.Instance.ControllerSettingsChange(value);
    }

    public void VolumeSettingsChange()
    {
        GameManager.Instance.VolumeSettingsChange(volumeSlider.value);
    }
}
