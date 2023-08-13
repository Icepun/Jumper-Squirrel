using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class levelScene : MonoBehaviour
{
    public void LevelButton(Button levelBtn)
    {
        int levelNum = int.Parse(levelBtn.GetComponentInChildren<TextMeshProUGUI>().text);
        LoadLevel(levelNum);
    }

    public void LoadLevel(int levelNumber)
    {
        string sceneName = "level" + levelNumber;
        SceneManager.LoadScene(sceneName);
    }
}
