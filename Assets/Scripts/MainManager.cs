using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text PlayerBestScore;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        var (score, name) = LoadBestScore();
        ShowBestScore(score, name);
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
        UpdateBestScore(m_Points);
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    public void UpdateBestScore(int score)
    {
        var (bestScore, bestPlayer) = LoadBestScore();
        if (score > bestScore)
        {
            bestScore = score;
            bestPlayer = ScoreManager.instance.playerName;
            ScoreManager.instance.score = bestScore;

            ShowBestScore(bestScore, bestPlayer);
            SaveBestScore();
        }
    }

    [System.Serializable]
    class PlayerScore
    {
        public string playerName;
        public int score;
    }

    public void SaveBestScore()
    {
        var data = new PlayerScore();
        data.playerName = ScoreManager.instance.playerName;
        data.score = ScoreManager.instance.score;

        var json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + $"/{data.playerName}.json", json);
    }
    public (int, string) LoadBestScore()
    {
        var path = Application.persistentDataPath + $"/{ScoreManager.instance.playerName}.json";

        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<PlayerScore>(json);

            PlayerBestScore.text= "Best score: " + data.playerName + ": " + data.score;

            return (data.score, data.playerName);
        }
        else
        {
            return (0, "New Player");
        }
    }


    private void ShowBestScore(int score, string name)
    {
        PlayerBestScore.text = "Best score: " + name + ": " + score;
    }
}
