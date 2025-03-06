using UnityEngine;

public class UpdateScores : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject scoreEntryPrefab;
    public Transform scoreEntryParent;
    public bool scoreSubmitted;

    private void Start()
    {
        gameManager = GameManager._gmInstance;
        scoreSubmitted = false;
    }

    private void Update()
    {
        if (gameManager.gameActive == false && scoreSubmitted == false)
        {
            if (HighScoreHandler._instance.GetMyRank(gameManager.finalScore) >= 0)
            {
                SubmitScore();
            }
        }
    }

    private void SubmitScore()
    {
        scoreSubmitted = true;

        string playerName = MenuHandler._instance.initials;

        HighScoreHandler.NameAndScore updatedScore = new HighScoreHandler.NameAndScore
        {
            Score = gameManager.finalScore,
            Name = playerName
        };

        HighScoreHandler._instance.AcceptNewScore(updatedScore);
        HighScoreHandler._instance.SaveScores();
        UpdateScoreboard();
    }
    private void UpdateScoreboard()
    {
        foreach (Transform child in scoreEntryParent)
        {
            Destroy(child.gameObject);
        }

        int scoreCount = HighScoreHandler._instance.GetScoreCount();
        for (int i = 0; i < scoreCount; i++)
        {
            HighScoreHandler.NameAndScore score = HighScoreHandler._instance.GetScoreAt(i);
            GameObject entry = Instantiate(scoreEntryPrefab, scoreEntryParent);
            entry.GetComponent<ScoreBoardEntry>().Setup(score, i + 1);
        }
    }
}
