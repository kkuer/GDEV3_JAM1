using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ChangeScene : MonoBehaviour
{
    public TMP_InputField initialsInput;

    public void changeScene()
    {
        MenuHandler._instance.saveInitials(initialsInput.text);
        Debug.Log(MenuHandler._instance.initials);
        SceneManager.LoadScene(1);
    }
}
