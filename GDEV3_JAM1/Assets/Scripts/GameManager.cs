using UnityEngine;

public class GameManager : MonoBehaviour
{
    //set single static instance of GameManager
    public static GameManager _gmInstance { get; private set; }

    public float score = 0;

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
        //set score to 0    
        score = 0;
    }

    public float addScore(float scoreToAdd)
    {
        score += scoreToAdd;
        return score;
    }

    public float removeScore(float scoreToRemove)
    {
        score -= scoreToRemove;
        return score;
    }
}
