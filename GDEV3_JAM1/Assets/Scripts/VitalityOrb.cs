using System.Collections;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class VitalityOrb : MonoBehaviour
{
    public float storedVitality = 20;
    private float depletionRate;
    public bool siphonable;
    public bool siphoning;

    public GameObject expireParticles;

    private ParticleSystem siphoningParticles;
    private PlayerController player;

    public enum orbType
    {
        Low,
        Medium,
        High
    }
    public orbType orbOutput;

    private void Start()
    {
        siphonable = false;
        StartCoroutine(Lifetime());

        siphoningParticles = GetComponent<ParticleSystem>();

        player = PlayerController._playerInstance;
        CapsuleCollider playerCol = player.GetComponentInParent<CapsuleCollider>();
        if (playerCol != null)
        {
            siphoningParticles.trigger.AddCollider(playerCol);
        }
    }

    private void FixedUpdate()
    {
        if (siphonable)
        {
            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, Vector3.zero, Time.deltaTime * 0.25f);
        }
        if (gameObject.transform.localScale.z <= 0.1f)
        {
            Instantiate(expireParticles, gameObject.transform.position, Quaternion.identity);
            SoundManager._instance.POPTWINKLE();
            Destroy(gameObject);
        }

        if (player.isSiphoning && siphonable)
        {
            if (siphoningParticles != null && !siphoningParticles.isPlaying)
            {
                siphoningParticles.Play();
            }
        }
        else if (!player.isSiphoning)
        {
            if (siphoningParticles != null && siphoningParticles.isPlaying)
            {
                siphoningParticles.Stop();
            }
        }
    }

    public void Deplete()
    {
        if (orbOutput == orbType.Low)
        {
            depletionRate = 20f;
        }
        else if (orbOutput == orbType.Medium)
        {
            depletionRate = 10f;
        }
        else if (orbOutput == orbType.High)
        {
            depletionRate = 5f;
        }

        storedVitality -= Time.deltaTime * depletionRate;
        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, Vector3.zero, Time.deltaTime * (depletionRate/10));

        //orb siphoning sfx
    }

    IEnumerator Lifetime()
    {
        StartCoroutine(SoundManager._instance.SIPHONSPAWN());
        SoundManager._instance.SIPHONSPAWN();
        yield return new WaitForSeconds(2f);
        Instantiate(expireParticles, gameObject.transform.position, Quaternion.identity);
        siphonable = true;
    }
}