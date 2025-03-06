using UnityEngine;
using Unity.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using TMPro;

public class GameManager : MonoBehaviour
{
    //set single static instance of GameManager
    public static GameManager _gmInstance { get; private set; }

    public int score = 0;
    public int finalScore;

    public List<GameObject> enemies;
    public List<GameObject> spawnPositions;

    public bool gameActive;
    private bool vitalityScoreAdded;

    public float gameTimer = 300;

    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;

    public PlayerController player;

    private void Awake()
    {
        //set GameManager instance
        if (_gmInstance == null)
        {
            _gmInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //start game
        gameActive = true;

        //get player instance
        player = PlayerController._playerInstance;

        //set score to 0
        score = 0;

        //allow vitality score adding
        vitalityScoreAdded = false;
    }

    private void Update()
    {
        if (gameTimer > 0 && gameActive)
        {
            gameTimer -= Time.deltaTime;
            updateTimer(gameTimer);

            //add current vitality to score
            if (player.vitality >= 0 && !vitalityScoreAdded)
            {
                StartCoroutine(addVitalityScore());
            }
            else if (player.vitality <= 0 && !vitalityScoreAdded)
            {
                if (score > 0)
                {
                    int vitalityScore = Mathf.FloorToInt(Time.deltaTime * 5);
                    removeScore(vitalityScore);
                }
                else
                {
                    score = 0;
                }
            }
        }
        else
        {
            Debug.Log("time up");
            gameTimer = 0;
            gameActive = false;
        }
    }

    private void FixedUpdate()
    {
        
    }

    public float addScore(int scoreToAdd)
    {
        score += scoreToAdd;
        string formattedScore = score.ToString("D6");
        scoreText.text = $"{formattedScore}";
        return score;
    }

    public float removeScore(int scoreToRemove)
    {
        score -= scoreToRemove;
        string formattedScore = score.ToString("D6");
        scoreText.text = $"{formattedScore}";
        return score;
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float mins = Mathf.FloorToInt(currentTime / 60);
        float secs = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:0} : {1:00}", mins, secs);
    }

    public IEnumerator addVitalityScore()
    {
        vitalityScoreAdded = true;
        int vitalityScore = Mathf.FloorToInt(player.vitality / 10);
        addScore(vitalityScore);
        yield return new WaitForSeconds(1f);
        vitalityScoreAdded = false;
    }
}