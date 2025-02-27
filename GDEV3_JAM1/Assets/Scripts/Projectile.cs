using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private float bulletSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //move bullet
    }
}
