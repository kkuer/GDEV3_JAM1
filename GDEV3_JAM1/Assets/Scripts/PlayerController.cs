using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System;
using UnityEditor.Experimental.GraphView;

public class PlayerController : MonoBehaviour
{
    public static PlayerController _playerInstance {  get; private set; }

    public bool playerWeak;
    public bool playerBuffed;

    public float vitality = 100;
    public bool isSiphoning;
    public bool isAttacking;

    public float siphonRadius;

    public GameObject playerMesh;
    private Rigidbody rb;

    public Camera cam;

    [SerializeField] private float defaultVitalityDecreaseRate;
    [SerializeField] private float defaultVitalityIncreaseRate;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float buffedMoveSpeed;
    [SerializeField] private float weakMoveSpeed;
    [SerializeField] private float meleeDuration;
    [SerializeField] private float rangeDuration;

    [SerializeField] private float swingSpeed;

    [SerializeField] private Quaternion bladeTargetRotation;

    public GameObject VFX_SLASH;
    public GameObject bladeHolstered;
    public GameObject bladePrefab;
    public Transform bladePivotPoint;

    public GameObject projectilePrefab;
    public GameObject currentBlade;

    public Slider vitalitySlider;

    public Volume globalVolume;
    public List<VolumeProfile> volumeProfiles = new List<VolumeProfile>();
    //gameplay  [0]
    //weak      [1]
    //buffed    [2]

    void Start()
    {
        //set Player instance
        if (_playerInstance == null)
        {
            _playerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //set startup variables
        isSiphoning = false;
        playerWeak = false;
        isAttacking = false;

        //get components
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //update UI
        vitalitySlider.value = vitality / 100;

        //update vitality
        if (isSiphoning && vitality <= 100f)
        {
            vitality += Time.deltaTime * defaultVitalityIncreaseRate;
        }
        else if (!isSiphoning && vitality >= 0f)
        {
            vitality -= Time.deltaTime * defaultVitalityDecreaseRate;
        }

        if (vitality <= 0f)
        {
            playerWeak = true;
            UpdateVolume(volumeProfiles[1]);
        }
        else if (vitality > 0f && vitality >= 33.4f)
        {
            playerWeak = false;
            playerBuffed = false;
            UpdateVolume(volumeProfiles[0]);
        }
        else if (vitality > 0f && vitality <= 33.3f)
        {
            playerBuffed = true;
        }

        if (playerWeak)
        {
            moveSpeed = weakMoveSpeed;
        }
        else if (!playerWeak && !playerBuffed)
        {
            moveSpeed = 500f;
        }
        else if (!playerWeak && playerBuffed)
        {
            moveSpeed = buffedMoveSpeed;
        }

        //blade swing
        if (currentBlade && isAttacking)
        {
            currentBlade.transform.localRotation = Quaternion.Lerp(currentBlade.transform.localRotation, bladeTargetRotation, Time.deltaTime * swingSpeed);
        }

        //input
        Vector3 inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        rb.linearVelocity = inputMovement * Time.deltaTime * moveSpeed;
        //transform.Translate(inputMovement * Time.deltaTime * moveSpeed, Space.World);
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
            StartCoroutine(meleeAttack());
        }
        if (Input.GetMouseButtonDown(1) && !isSiphoning)
        {
            ShakeBehaviour._instance.shakeCam(2f, 0.1f);
            StartCoroutine(rangedAttack());
        }

        //vitality inputs
        if (Input.GetKey(KeyCode.Space))
        {
            //check orb distance
            //isSiphoning = true;
            siphonByDistance();
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

    IEnumerator meleeAttack()
    {
        //state logic
        isAttacking = true;

        //visuals

        GameObject slash = Instantiate(VFX_SLASH, bladePivotPoint.position, bladePivotPoint.rotation);
        GameObject blade = Instantiate(bladePrefab, Vector3.zero, Quaternion.identity, bladePivotPoint);
        blade.transform.localPosition = Vector3.zero;
        blade.transform.localRotation = Quaternion.identity;
        bladeTargetRotation = Quaternion.Euler(0f, 120f, 0f);
        currentBlade = blade;
        bladeHolstered.SetActive(false);

        //attack logic


        //wait and reset
        yield return new WaitForSeconds(meleeDuration);

        Destroy(slash);
        currentBlade = null;
        Destroy(blade);
        bladeHolstered.SetActive(true);

        isAttacking = false;
    }

    IEnumerator rangedAttack()
    {
        //state logic
        isAttacking = true;

        //visuals
        //partiles enable here

        //handle attack logic
        GameObject projectile = Instantiate(projectilePrefab, bladePivotPoint.position, playerMesh.transform.localRotation);
        Destroy(projectile, 10f);

        //wait and reset
        yield return new WaitForSeconds(rangeDuration);


        //partiles disable here
        isAttacking = false;
    }

    private void UpdateVolume(VolumeProfile profile)
    {
        globalVolume.profile = profile;
    }

    private void siphonByDistance()
    {
        Collider[] nearbyOrbs = Physics.OverlapSphere(gameObject.transform.position, siphonRadius);

        bool foundSiphonableOrb = false;

        foreach (var collider in nearbyOrbs)
        {
            VitalityOrb orb = collider.GetComponentInParent<VitalityOrb>();

            if (orb != null && orb.siphonable)
            {
                foundSiphonableOrb = true;
                orb.Deplete();
            }
        }
        if (foundSiphonableOrb)
        {
            isSiphoning = true;
        }
        else
        {
            isSiphoning = false;
        }
    }
}