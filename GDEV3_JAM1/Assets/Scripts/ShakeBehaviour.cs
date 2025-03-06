using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class ShakeBehaviour : MonoBehaviour
{
    public static ShakeBehaviour _instance { get; private set; }

    private CinemachineCamera cam;
    private float shakeTimer;

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
        Debug.Log("Flashed");
        //set image color

        //turn on middle

        yield return new WaitForSeconds(0.05f);

        //turn on outside

        yield return new WaitForSeconds(0.05f);

        //delete middle

        yield return new WaitForSeconds(0.05f);

        //delete outside
    }

    public IEnumerator quickFlash(Color color)
    {
        Debug.Log("FlashedQuick");
        //set image color

        //turn on

        yield return new WaitForSeconds(0.05f);

        //turn off
    }
}
