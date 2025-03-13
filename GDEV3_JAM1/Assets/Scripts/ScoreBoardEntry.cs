using UnityEngine;
using TMPro;

public class ScoreBoardEntry : MonoBehaviour
{
    public TextMeshProUGUI NameLabel;
    public TextMeshProUGUI ScoreLabel;

    public void Setup(HighScoreHandler.NameAndScore s, int rank)
    {
        NameLabel.text = s.Name;
        ScoreLabel.text = s.Score.ToString("D6");
    }
}
