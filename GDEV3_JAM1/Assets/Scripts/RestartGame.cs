using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public void changeScene()
    {
        SceneManager.LoadScene(0);
    }
}
