using UnityEngine;
using Unity.Cinemachine;

public class ShakeBehaviour : MonoBehaviour
{
    public static ShakeBehaviour _instance { get; private set; }

    private CinemachineCamera cam;
    private float shakeTimer;

    private void Awake()
    {
        _instance = this;
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
}
