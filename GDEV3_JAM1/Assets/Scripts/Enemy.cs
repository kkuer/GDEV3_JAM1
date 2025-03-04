using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health;
    private float damage;
    private float movespeed;

    private float canTakeDamage;

    public GameManager gameManager;

    public GameObject essenceParticles;
    public GameObject vitalityOrb;

    public enum enemyType
    {
        Standard,
        Heavy,
        Light
    }
    public enemyType type;

    void Start()
    {
        //find game manager instance
        if (GameManager._gmInstance != null)
        {
            gameManager = GameManager._gmInstance;
        }

        //set stats
        if(type == enemyType.Standard)
        {
            health = 50;
        }
        else if (type == enemyType.Heavy)
        {
            health = 100;
        }
        else if (type == enemyType.Light)
        {
            health = 25;
        }
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
                takeDamage(blade.baseDamage);
                gameManager.addScore(5f);
            }
            else if (blade.bladeHits.Count >= 1)
            {
                foreach (GameObject hit in blade.bladeHits)
                {
                    if (hit != gameObject)
                    {
                        takeDamage(blade.baseDamage);
                        gameManager.addScore(5f);
                    }
                }
            }
            
        }
        else if (projectile != null)
        {
            takeDamage(5f);
            gameManager.addScore(1f);
            Destroy(projectile.gameObject);
        }
    }
}
