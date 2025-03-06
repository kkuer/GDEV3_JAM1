using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
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
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }
}