using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float vitality = 100;
    public bool isSiphoning;
    public bool isAttacking;

    public GameObject playerMesh;
    private Rigidbody rb;

    public Camera cam;

    [SerializeField] private float defaultVitalityDecreaseRate;
    [SerializeField] private float defaultVitalityIncreaseRate;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float meleeDuration;
    [SerializeField] private float rangeDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isSiphoning = false;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //update vitality
        if (isSiphoning && vitality <= 100f)
        {
            vitality += Time.deltaTime * defaultVitalityIncreaseRate;
        }
        else if (!isSiphoning && vitality >= 0f)
        {
            vitality -= Time.deltaTime * defaultVitalityDecreaseRate;
        }

        //input
        Vector3 inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(inputMovement * Time.deltaTime * moveSpeed, Space.World);
    }

    void Update()
    {
        //player aim
        if (playerMesh)
        {
            trackMouse();
        }

        //attack inputs
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(meleeCooldown());
        }
        if (Input.GetMouseButtonDown(1) && !isAttacking)
        {
            StartCoroutine(rangeCooldown());
        }

        //vitality inputs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSiphoning = true;
            isAttacking = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isSiphoning = false;
            isAttacking = false;
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
            playerMesh.transform.LookAt(hitPos, Vector3.up);
        }
    }

    IEnumerator meleeCooldown()
    {
        isAttacking = true;

        //handle attack logic

        yield return new WaitForSeconds(meleeDuration);
        isAttacking = false;
    }

    IEnumerator rangeCooldown()
    {
        isAttacking = true;

        //handle attack logic

        yield return new WaitForSeconds(rangeDuration);
        isAttacking = false;
    }
}
