using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class startScene : MonoBehaviour
{
    public GameObject settingsPanel;

    public Button soundsOnOff;
    public static int isSoundOn;

    public Button vibrationsOnOff;
    public static int isVibrationOn;

    private void Start()
    {
        settingsPanel.SetActive(false);
        isSoundOn = PlayerPrefs.GetInt("sound");
        isVibrationOn = PlayerPrefs.GetInt("vibration");

        if (isSoundOn == 0)
        {
            soundsOnOff.GetComponentInChildren<TextMeshProUGUI>().text = "Sounds : Off";
        }

        else if (isSoundOn == 1)
        {
            soundsOnOff.GetComponentInChildren<TextMeshProUGUI>().text = "Sounds : On";
        }

        if (isVibrationOn == 0)
        {
            vibrationsOnOff.GetComponentInChildren<TextMeshProUGUI>().text = "Vibrations : Off";
        }

        else if (isVibrationOn == 1)
        {
            vibrationsOnOff.GetComponentInChildren<TextMeshProUGUI>().text = "Vibrations : On";
        }
    }

    public void StartButton()
    {
        SceneManager.LoadScene("levelScene");
    }

    public void SettingsButton()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseBtn()
    {
        settingsPanel.SetActive(false);
    }

    public void SoundsBtn()
    {
        if (isSoundOn == 0)
        {
            isSoundOn = 1;
            soundsOnOff.GetComponentInChildren<TextMeshProUGUI>().text = "Sounds : On";
            PlayerPrefs.SetInt("sound", isSoundOn);
        }

        else if (isSoundOn == 1)
        {
            isSoundOn = 0;
            soundsOnOff.GetComponentInChildren<TextMeshProUGUI>().text = "Sounds : Off";
            PlayerPrefs.SetInt("sound", isSoundOn);
        }
    }

    public void VibrationsBtn()
    {
        if (isVibrationOn == 0)
        {
            isVibrationOn = 1;
            vibrationsOnOff.GetComponentInChildren<TextMeshProUGUI>().text = "Vibrations : On";
            PlayerPrefs.SetInt("vibration", isVibrationOn);
        }

        else if (isVibrationOn == 1)
        {
            isVibrationOn = 0;
            vibrationsOnOff.GetComponentInChildren<TextMeshProUGUI>().text = "Vibrations : Off";
            PlayerPrefs.SetInt("vibration", isVibrationOn);
        }
    }
}
