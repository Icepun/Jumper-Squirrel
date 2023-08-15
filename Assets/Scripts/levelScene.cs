using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class levelScene : MonoBehaviour
{
    public Button[] levelButtons;
    public AudioSource music;

    private void Start()
    {
        if (startScene.isSoundOn == 0)
        {
            music.mute = true;
        }

        else if (startScene.isSoundOn == 1)
        {
            music.mute = false;
        }

        int lastCompletedLevel = PlayerPrefs.GetInt("LastCompletedLevel", 0);

        // En son geçilen seviyeden bir fazla butonu etkinleþtir
        for (int i = 0; i < lastCompletedLevel + 1 && i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = true;
        }
    }

    public void LevelButton(Button levelBtn)
    {
        int levelNum;
        if (int.TryParse(levelBtn.GetComponentInChildren<TextMeshProUGUI>().text, out levelNum))
        {
            LoadLevel(levelNum);
        }
    }

    public void LoadLevel(int levelNumber)
    {
        string sceneName = "level" + levelNumber;
        SceneManager.LoadScene(sceneName);
    }

    public void CloseButton()
    {
        SceneManager.LoadScene(0);
    }
}
