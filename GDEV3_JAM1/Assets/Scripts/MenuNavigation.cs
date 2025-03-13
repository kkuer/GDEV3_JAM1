using UnityEngine;

public class MenuNavigation : MonoBehaviour
{
    public GameObject helpBG;
    public void clickEnter()
    {
        helpBG.SetActive(true);
    }

    public void clickExit()
    {
        helpBG.SetActive(false);
    }
}
