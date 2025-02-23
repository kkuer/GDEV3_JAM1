using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public bool playerWeak;

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

    [SerializeField] private float swingSpeed;

    [SerializeField] private Quaternion bladeStartRotation;
    [SerializeField] private Quaternion bladeTargetRotation;

    public GameObject VFX_SLASH;

    public GameObject bladePrefab;
    public Transform bladePivotPoint;


    public Volume globalVolume;
    public List<VolumeProfile> volumeProfiles = new List<VolumeProfile>();
    //gameplay  [0]
    //weak      [1]

    void Start()
    {
        //set startup variables
        isSiphoning = false;
        playerWeak = false;
        isAttacking = false;

        //get components
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

        if(vitality <= 0f)
        {
            playerWeak = true;
            UpdateVolume(volumeProfiles[1]);
        }
        else if (vitality > 0)
        {
            playerWeak = false;
            UpdateVolume(volumeProfiles[0]);
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
            TrackMouse();
        }

        //attack inputs
        if (Input.GetMouseButtonDown(0) && !isAttacking && !isSiphoning)
        {
            ShakeBehaviour._instance.shakeCam(2f, 0.15f);
            StartCoroutine(MeleeCooldown());

        }
        if (Input.GetMouseButtonDown(1) && !isSiphoning)
        {
            ShakeBehaviour._instance.shakeCam(2f, 0.1f);
            StartCoroutine(RangeCooldown());
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

    public void TrackMouse()
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

    IEnumerator MeleeCooldown()
    {
        //state logic
        isAttacking = true;

        //visuals
        GameObject slash = Instantiate(VFX_SLASH, bladePivotPoint.position, bladePivotPoint.rotation);
        GameObject blade = Instantiate(bladePrefab, bladePivotPoint.position, Quaternion.Euler(0f, bladePivotPoint.rotation.y, 0f), bladePivotPoint);
        blade.transform.rotation = Quaternion.Lerp(bladeStartRotation, bladeTargetRotation, Time.deltaTime * swingSpeed);

        //attack logic

        //wait and reset
        yield return new WaitForSeconds(meleeDuration);

        Destroy(slash);
        Destroy(blade);

        isAttacking = false;
    }

    IEnumerator RangeCooldown()
    {
        //state logic
        isAttacking = true;

        //visuals
        //partiles enable here

        //handle attack logic

        //wait and reset
        yield return new WaitForSeconds(rangeDuration);


        //partiles disable here
        isAttacking = false;
    }

    private void UpdateVolume(VolumeProfile profile)
    {
        globalVolume.profile = profile;
    }
}
