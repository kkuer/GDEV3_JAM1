using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager _instance {  get; private set; }

    public AudioSource SFX_SWING;
    public AudioSource SFX_HIT;
    public AudioSource SFX_SWINGHIT;
    public AudioSource SFX_SIPHON;
    public AudioSource SFX_SHOOT;
    public AudioSource SFX_AOE;
    public AudioSource SFX_ABILREADY;
    public AudioSource SFX_POP;
    public AudioSource SFX_WOOSH;
    public AudioSource SFX_POPTWINKLE;

    private void Awake()
    {
        //set sound manager instance
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SWINGHIT()
    {
        SFX_SWINGHIT.pitch = Random.Range(0.90f, 1.10f);
        SFX_SWINGHIT.Play();
    }

    public void HIT()
    {
        SFX_HIT.pitch = Random.Range(0.90f, 1.10f);
        SFX_HIT.Play();
    }

    public void SWING()
    {
        SFX_SWING.pitch = Random.Range(0.80f, 1.10f);
        SFX_SWING.Play();
    }

    public void SHOOT()
    {
        SFX_SHOOT.pitch = Random.Range(0.80f, 1.20f);
        SFX_SHOOT.Play();
    }

    public void AOE()
    {
        SFX_AOE.pitch = 1f;
        SFX_AOE.Play();
    }

    public void HEALABIL()
    {
        SFX_ABILREADY.pitch = 1f;
        SFX_ABILREADY.Play();
    }

    public IEnumerator SIPHONSPAWN()
    {
        SFX_WOOSH.pitch = Random.Range(0.90f, 1.10f);
        SFX_WOOSH.Play();

        yield return new WaitForSeconds(2);

        SFX_POP.pitch = Random.Range(0.80f, 1.20f);
        SFX_POP.Play();
    }

    public void POPTWINKLE()
    {
        SFX_POPTWINKLE.pitch = Random.Range(0.80f, 1.20f);
        SFX_POPTWINKLE.Play();
    }
}
