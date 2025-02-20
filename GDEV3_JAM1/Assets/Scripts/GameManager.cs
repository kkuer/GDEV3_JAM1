using UnityEngine;

public class GameManager : MonoBehaviour
{
    //set single static instance of GameManager
    public static GameManager _gmInstance { get; private set; }

    private void Start()
    {
        //set GameManager instance
        if (_gmInstance != null)
        {
            _gmInstance = this;
        }
    }
}
