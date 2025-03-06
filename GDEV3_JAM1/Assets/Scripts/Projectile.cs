using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private float bulletSpeed;

    public float baseDamage;
    public float weakDamage;
    public float buffedDamage;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //move bullet
    }
}