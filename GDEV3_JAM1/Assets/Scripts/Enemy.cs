using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health;

    public float damage;
    [SerializeField] private float damageCooldown;

    private float movespeed;
    [SerializeField] private int scoreToAdd;

    public bool canDealDamage;

    public GameManager gameManager;
    public PlayerController player; 

    public GameObject essenceParticles;
    public GameObject vitalityOrb;

    public Slider healthSlider;

    public enum enemyType
    {
        Standard,
        Heavy,
        Light
    }
    public enemyType type;

    void Start()
    {
        player = PlayerController._playerInstance;
        canDealDamage = true;

        //find game manager instance
        if (GameManager._gmInstance != null)
        {
            gameManager = GameManager._gmInstance;
        }

        //set stats
        if (type == enemyType.Standard)
        {
            health = 50;
            healthSlider.maxValue = health / 100;
            scoreToAdd = 25;
        }
        else if (type == enemyType.Heavy)
        {
            health = 100;
            healthSlider.maxValue = health / 100;
            scoreToAdd = 50;
        }
        else if (type == enemyType.Light)
        {
            health = 25;
            healthSlider.maxValue = health / 100;
            scoreToAdd = 15;
        }
    }

    private void Update()
    {
        //update healthbar
        if (health > 0)
        {
            healthSlider.value = health / 100;
        }
        
        //navmesh here
    }

    public float takeDamage(float damageToTake)
    {
        health -= damageToTake;

        //damage particles


        if (health < 0f)
        {
            health = 0f;
            //death particles


            //essence particles
            Instantiate(essenceParticles, gameObject.transform.position, Quaternion.identity);

            //spawn vitality orb
            Instantiate(vitalityOrb, gameObject.transform.position, Quaternion.identity);

            //add score
            gameManager.addScore(scoreToAdd);

            //delete enemy
            Destroy(gameObject);
        }
        return health;
    }

    private void OnTriggerEnter(Collider other)
    {
        Blade blade = other.GetComponentInParent<Blade>();
        Projectile projectile = other.GetComponentInParent<Projectile>();
        if (blade != null)
        {
            if (blade.bladeHits.Count == 0)
            {
                blade.bladeHits.Add(gameObject);

                if (player.playerBuffed)
                {
                    takeDamage(blade.buffedDamage);
                }
                else if (player.playerWeak)
                {
                    takeDamage(blade.weakDamage);
                }
                else if (!player.playerBuffed && !player.playerWeak)
                {
                    takeDamage(blade.baseDamage);
                }
                gameManager.addScore(25);
            }
            else if (blade.bladeHits.Count >= 1)
            {
                foreach (GameObject hit in blade.bladeHits)
                {
                    if (hit != gameObject)
                    {
                        if (player.playerBuffed)
                        {
                            takeDamage(blade.buffedDamage);
                        }
                        else if (player.playerWeak)
                        {
                            takeDamage(blade.weakDamage);
                        }
                        else if (!player.playerBuffed && !player.playerWeak)
                        {
                            takeDamage(blade.baseDamage);
                        }
                        gameManager.addScore(25);
                    }
                }
            }
        }
        else if (projectile != null)
        {
            if (player.playerBuffed)
            {
                takeDamage(projectile.buffedDamage);
            }
            else if (player.playerWeak)
            {
                takeDamage(projectile.weakDamage);
            }
            else if (!player.playerBuffed && !player.playerWeak)
            {
                takeDamage(projectile.baseDamage);
            }
            gameManager.addScore(5);
            Destroy(projectile.gameObject);
        }
    }

    public IEnumerator dealDamage()
    {
        canDealDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDealDamage = true;
    }
}