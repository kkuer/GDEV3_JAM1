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
    private bool vitalityScoreAdjusted;

    public float gameTimer = 300;

    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public GameObject endScreen;

    public PlayerController player;

    private bool enemySpawnable;

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
        vitalityScoreAdjusted = false;

        //allow enemies to spawn
        enemySpawnable = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameActive)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        if (gameTimer > 0 && gameActive)
        {
            gameTimer -= Time.deltaTime;
            updateTimer(gameTimer);

            //spawn enemies
            if (enemySpawnable && gameActive)
            {
                StartCoroutine(spawnEnemyTest());
            }

            //add current vitality to score
            if (player.vitality >= 0 && !vitalityScoreAdjusted)
            {
                StartCoroutine(addVitalityScore());
            }
            else if (player.vitality <= 0 && !vitalityScoreAdjusted)
            {
                if (score > 0)
                {
                    StartCoroutine(removeVitalityScore());
                }
                else
                {
                    score = 0;
                }
            }
        }
        else
        {
            gameTimer = 0;
            finalScore = score;
            finalScoreText.text = finalScore.ToString("D8");
            endScreen.SetActive(true);
            gameActive = false;
        }
    }

    public float addScore(int scoreToAdd)
    {
        score += scoreToAdd;
        string formattedScore = score.ToString("D8");
        scoreText.text = $"{formattedScore}";
        return score;
    }

    public float removeScore(int scoreToRemove)
    {
        score -= scoreToRemove;
        string formattedScore = score.ToString("D8");
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
        vitalityScoreAdjusted = true;
        int vitalityScore = Mathf.RoundToInt(player.vitality / 10);
        addScore(vitalityScore);
        yield return new WaitForSeconds(1f);
        vitalityScoreAdjusted = false;
    }

    public IEnumerator removeVitalityScore()
    {
        vitalityScoreAdjusted = true;
        removeScore(1);
        yield return new WaitForSeconds(0.125f);
        vitalityScoreAdjusted = false;
    }

    public IEnumerator spawnEnemyTest()
    {
        enemySpawnable = false;

        GameObject randomEnemy = enemies[Random.Range(0, enemies.Count)];
        GameObject randomSpawn = spawnPositions[Random.Range(0, spawnPositions.Count)];

        GameObject newEnemy = Instantiate(randomEnemy, randomSpawn.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(3f);

        enemySpawnable = true;
    }
}