using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    public float vitality = 100;
    public bool isSiphoning;

    private GameObject player;
    private Rigidbody rb;

    public Camera cam;

    [SerializeField] private float vitalityDecreaseRate;
    [SerializeField] private float vitalityIncreaseRate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isSiphoning = false;
        player = this.gameObject;
        rb = player.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //update vitality
        if (isSiphoning && vitality <= 100f)
        {
            vitality += Time.deltaTime * vitalityIncreaseRate;
        }
        else if (!isSiphoning && vitality >= 0f)
        {
            vitality -= Time.deltaTime * vitalityDecreaseRate;
        }
    }

    void Update()
    {
        //player aim
        if (player)
        {
            trackMouse();
        }

        //vitality inputs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSiphoning = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isSiphoning = false;
        }
    }

    public void trackMouse()
    {
        Plane mousePlane = new Plane(Vector3.up, transform.position);
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);

        bool didHit = mousePlane.Raycast(camRay, out float distance);

        if (didHit)
        {
            Vector3 hitPos = camRay.GetPoint(distance);
            player.transform.LookAt(hitPos, Vector3.up);
        }
    }
}
