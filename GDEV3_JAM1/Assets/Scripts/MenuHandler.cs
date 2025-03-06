using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public static MenuHandler _instance { get; private set; }

    public string initials = "";

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void saveInitials(string name)
    {
        initials = name;
    }

    public string getInitials()
    {
        return initials;
    }
}
