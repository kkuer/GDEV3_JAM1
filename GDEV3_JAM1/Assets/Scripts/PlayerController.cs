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
            Vector3 point = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            float t = cam.transform.position.y / (cam.transform.position.y - point.y);
            Vector3 finalPoint = new Vector3(t * (point.x - cam.transform.position.x) + cam.transform.position.x, 1, t * (point.z - cam.transform.position.z) + cam.transform.position.z);
            player.transform.LookAt(finalPoint, Vector3.up);
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
}
