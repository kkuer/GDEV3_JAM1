using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System;
public enum state
{
    Normal,
    Weakened,
    Buffed
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController _playerInstance {  get; private set; }

    public bool playerWeak;
    public bool playerBuffed;

    public float vitality = 100;
    public bool isSiphoning;
    public bool isAttacking;

    public bool canUseQ;
    public bool canUseE;

    public float siphonRadius;
    public float aoeRadius;

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
    public GameObject VFX_AOE;

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

    public state playerState;

    public int nearbyOrbsAmount;

    public Image vitalityBarImage;
    public Color normalVitalityColor;
    public Color adrenalineVitalityColor;

    public GameObject bloodParticles;
    public GameObject shootParticles;

    private void Awake()
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
    }

    void Start()
    {
        //set startup variables
        playerState = state.Normal;

        //get components
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //blade swing
        if (currentBlade && isAttacking)
        {
            currentBlade.transform.localRotation = Quaternion.Lerp(currentBlade.transform.localRotation, bladeTargetRotation, Time.deltaTime * swingSpeed);
        }

        //input
        Vector3 inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        rb.linearVelocity = inputMovement * Time.fixedDeltaTime * moveSpeed;
    }

    void Update()
    {
        updateVitality();
        managePlayerState();

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
        if (Input.GetMouseButtonDown(1) && !isAttacking && !isSiphoning)
        {
            ShakeBehaviour._instance.shakeCam(2f, 0.1f);
            StartCoroutine(rangedAttack());
        }

        //vitality inputs
        if (Input.GetKey(KeyCode.Space))
        {
            siphonByDistance();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isSiphoning = false;
        }

        //ability inputs
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //q
            if (canUseQ)
            {
                ShakeBehaviour._instance.shakeCam(2f, 0.1f);
                GameManager._gmInstance.qCooldownTimer = 20;

                //add set vitality amount
                vitality += 50f;
                StartCoroutine(ShakeBehaviour._instance.quickFlash(Color.green));

                //sounds
                SoundManager._instance.HEALABIL();
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //e
            if (canUseE && !isSiphoning)
            {
                ShakeBehaviour._instance.shakeCam(2f, 0.15f);
                GameManager._gmInstance.eCooldownTimer = 25;

                //aoe logic
                aoeAttack();
                GameObject aoeBurst = Instantiate(VFX_AOE, bladePivotPoint.position, bladePivotPoint.rotation);
                StartCoroutine(ShakeBehaviour._instance.quickFlash(Color.white));
                Destroy(aoeBurst, 2f);

                //sounds
                SoundManager._instance.AOE();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
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

        //visuals and audio
        StartCoroutine(ShakeBehaviour._instance.quickFlash(Color.white));
        SoundManager._instance.SWING();

        GameObject slash = Instantiate(VFX_SLASH, bladePivotPoint.position, bladePivotPoint.rotation);
        GameObject blade = Instantiate(bladePrefab, Vector3.zero, Quaternion.identity, bladePivotPoint);
        blade.transform.localPosition = Vector3.zero;
        blade.transform.localRotation = Quaternion.identity;
        bladeTargetRotation = Quaternion.Euler(0f, 120f, 0f);
        currentBlade = blade;
        bladeHolstered.SetActive(false);

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

        //visuals and audio
        GameObject projParticles = Instantiate(shootParticles, bladePivotPoint.position, playerMesh.transform.rotation);
        SoundManager._instance.SHOOT();

        //handle attack logic
        GameObject projectile = Instantiate(projectilePrefab, bladePivotPoint.position, playerMesh.transform.rotation);
        Destroy(projectile, 5f);

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

        int orbsInRange = 0;

        foreach (var collider in nearbyOrbs)
        {
            VitalityOrb orb = collider.GetComponentInParent<VitalityOrb>();

            if (orb != null && orb.siphonable)
            {
                foundSiphonableOrb = true;
                orbsInRange++;
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

        nearbyOrbsAmount = orbsInRange;
    }

    private void aoeAttack()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(gameObject.transform.position, aoeRadius);

        HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();

        foreach (var collider in nearbyEnemies)
        {
            Enemy enemy = collider.GetComponentInParent<Enemy>();

            if (enemy != null && !damagedEnemies.Contains(enemy))
            {
                if (playerState == state.Normal || playerState == state.Weakened)
                {
                    enemy.takeDamage(80f);
                }
                else if (playerState == state.Buffed)
                {
                    enemy.takeDamage(150f);
                }
                GameObject blood = Instantiate(bloodParticles, enemy.gameObject.transform.position, gameObject.transform.rotation);
                damagedEnemies.Add(enemy);
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null && enemy.canDealDamage && enemy.canDealDamage)
        {
            ShakeBehaviour._instance.screenFlash(Color.red);

            StartCoroutine(enemy.dealDamage());
            GameObject blood = Instantiate(bloodParticles, other.ClosestPoint(transform.position), gameObject.transform.rotation);
            ShakeBehaviour._instance.shakeCam(2f, 0.2f);
            StartCoroutine(ShakeBehaviour._instance.screenFlash(Color.red));
            vitality -= enemy.damage;
        }
    }

    public void updateVitality()
    {
        //update UI
        vitalitySlider.value = vitality / 100;

        //update vitality
        if (isSiphoning && vitality <= 100f)
        {
            vitality += Time.deltaTime * (nearbyOrbsAmount * defaultVitalityIncreaseRate);
        }
        else if (!isSiphoning && vitality >= 0f)
        {
            vitality -= Time.deltaTime * defaultVitalityDecreaseRate;
            if (vitality >= 100f)
            {
                vitality = 100f;
            }
        }

        if (vitality <= 0f)
        {
            playerState = state.Weakened;
            vitality = 0f;
            UpdateVolume(volumeProfiles[1]);
        }
        else if (vitality > 0f && vitality >= 39.3f)
        {
            playerState = state.Normal;
            UpdateVolume(volumeProfiles[0]);
            vitalityBarImage.color = normalVitalityColor;
        }
        else if (vitality > 0f && vitality <= 39.3f)
        {
            playerState = state.Buffed;
            UpdateVolume(volumeProfiles[2]);
            vitalityBarImage.color = adrenalineVitalityColor;
        }
        else if (vitality >= 100f)
        {
            vitality = 100f;
        }
    }

    public void managePlayerState()
    {
        if (playerState == state.Weakened)
        {
            moveSpeed = weakMoveSpeed;
        }
        else if (playerState == state.Normal)
        {
            moveSpeed = 500f;
        }
        else if (playerState == state.Buffed)
        {
            moveSpeed = buffedMoveSpeed;
        }
    }
}