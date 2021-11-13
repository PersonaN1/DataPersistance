using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    public Text PlayerBestScoreText;
    public InputField PlayerName;
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
		Application.Quit();
#endif
    }

    void Update()
    {
        PlayerName.onEndEdit.AddListener(delegate { SubmitName(PlayerName.text);  });
    }

    void SubmitName(string text)
    {
        ScoreManager.instance.playerName = text;
    }
}
