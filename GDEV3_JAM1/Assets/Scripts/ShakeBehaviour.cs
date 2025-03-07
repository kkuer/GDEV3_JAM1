using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.UI;

public class ShakeBehaviour : MonoBehaviour
{
    public static ShakeBehaviour _instance { get; private set; }

    private CinemachineCamera cam;
    private float shakeTimer;

    public GameObject flashBasic;
    public GameObject flashCenter;
    public GameObject flashCorners;

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
        cam = GetComponent<CinemachineCamera>();
    }

    public void shakeCam(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin perlin = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.AmplitudeGain = intensity;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                //timer over
                CinemachineBasicMultiChannelPerlin perlin = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();

                perlin.AmplitudeGain = 0f;
            }
        }
    }

    public IEnumerator screenFlash(Color color)
    {
        flashCenter.GetComponent<Image>().color = color;
        flashCorners.GetComponent<Image>().color = color;

        flashCenter.SetActive(true);

        yield return new WaitForSeconds(0.05f);

        flashCorners.SetActive(true);
        flashCenter.SetActive(false);

        yield return new WaitForSeconds(0.05f);

        flashCorners.SetActive(false);
    }

    public IEnumerator quickFlash(Color color)
    {
        flashBasic.GetComponent<Image>().color = color;

        flashBasic.SetActive(true);

        yield return new WaitForSeconds(0.02f);

        flashBasic.SetActive(false);
    }
}
