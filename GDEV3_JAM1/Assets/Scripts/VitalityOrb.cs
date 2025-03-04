using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class VitalityOrb : MonoBehaviour
{
    public float storedVitality = 20;
    private float depletionRate;
    public bool siphonable;

    public GameObject expireParticles;

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
            //orb disappear sfx
            Destroy(gameObject);
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

        //orb siphoning sfx and particles
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(2f);
        siphonable = true;
    }
}
