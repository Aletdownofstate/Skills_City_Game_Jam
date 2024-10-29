using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody rb;
    private Animator animator;
    private PlayerHealth playerHealth;
    private Ammo ammo;

    [SerializeField] private AudioSource gunshotSound, reloadSound;    
    [SerializeField] private Light muzzleFlash;
    [SerializeField] private Slider staminaBar;

    private bool canControl = true;

    private float moveHorizontal;
    private float moveVertical;
    private float moveSpeed = 4f;

    private bool isIdle;
    private bool isRunning = false;
    private bool isAiming = false;

    public LayerMask enemyLayer;
    private bool canShoot = true;
    public float shootRange = 100f;

    private int staminaMax = 100;
    private int stamina;
    private bool canDrainStamina = true;
    private bool canRefillStamina = false;
    private bool isStaminaDelayComplete = false;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponentInChildren<Animator>();
        playerHealth = gameObject.GetComponent<PlayerHealth>();
        ammo = gameObject.GetComponent<Ammo>();
        mainCamera = Camera.main;

        stamina = staminaMax;

        isIdle = true;
        muzzleFlash.enabled = false;
    }

    private void Update()
    {
        if (playerHealth.isDead)
        {
            canControl = false;
        }
        else
        {
            canControl = true;
        }

        if (canControl)
        {
            // Inputs
            // Movement

            moveHorizontal = Input.GetAxisRaw("Horizontal");
            moveVertical = Input.GetAxisRaw("Vertical");

            // Running

            if (Input.GetKey(KeyCode.LeftShift) && stamina != 0)
            {
                isRunning = true;
            }
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                isRunning = false;
                moveSpeed = 4f;                
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isStaminaDelayComplete = false;
                StartCoroutine(StaminaRefillDelay());
            }

            // Aiming

            if (Input.GetMouseButton(1) && !isRunning)
            {
                isAiming = true;
            }
            if (!Input.GetMouseButton(1) || isRunning)
            {
                isAiming = false;
            }

            // Shooting

            if (ammo.currentClipSize == 0 || ammo.currentAmmo == 0)
            {
                canShoot = false;
            }

            if (Input.GetMouseButton(0) && isAiming && canShoot)
            {
                Shoot();
                StartCoroutine(MuzzleFlashDelay());
                gunshotSound.Play();
                StartCoroutine(ShootDelay());
            }

            // Reloading

            if (Input.GetKeyDown(KeyCode.R))
            {
                reloadSound.Play();
                StartCoroutine(ReloadDelay());
                int firedAmmo = ammo.maxClipSize - ammo.currentClipSize;
                ammo.currentAmmo -= firedAmmo;
                ammo.currentClipSize = ammo.maxClipSize;
            }

            RotateCharacterToMouse();
        }

        #region Animations

        // Idle

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            isIdle = true;
        }

        // Walking

        if (moveVertical != 0 || moveHorizontal != 0)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
            isIdle = false;
        }

        // Running

        if (isRunning && !isIdle && stamina != 0)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);
            isIdle = false;

            moveSpeed = 10f;
        }
        else if (!isRunning)
        {
            animator.SetBool("isRunning", false);
        }

        // Aiming

        if (isAiming && isIdle)
        {
            animator.SetBool("isIdleAiming", true);
        }
        else
        {
            animator.SetBool("isIdleAiming", false);
        }

        if (isAiming && !isIdle && !isRunning)
        {
            animator.SetBool("isMoveAiming", true);
        }
        else
        {
            animator.SetBool("isMoveAiming", false);
        }

        // Death

        if (playerHealth.isDead)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdleAiming", false);
            animator.SetBool("isMoveAiming", false);

            animator.SetBool("isDead", true);
        }

        #endregion

        #region Stamina

        staminaBar.value = stamina;

        if (stamina == 0)
        {            
            moveSpeed = 4f;
            isRunning = false;
        }

        if (isRunning && !isIdle && canDrainStamina)
        {
            StartCoroutine(DrainStamina());            
        }

        if (isStaminaDelayComplete)
        {
            StartCoroutine(RefillStamina());
        }

        #endregion
    }

    private void FixedUpdate()
    {
        Vector3 movement = (transform.right * moveHorizontal + transform.forward * moveVertical).normalized * moveSpeed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    private void RotateCharacterToMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        float hitDist = 0.0f;

        if (playerPlane.Raycast(ray, out hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            targetPoint.y = transform.position.y;

            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, shootRange, enemyLayer))
        {
            EnemyAI enemy = hit.collider.GetComponentInChildren<EnemyAI>();
            enemy.TakeDamage(100);
        }

        ammo.currentClipSize--;

        if (!EnemyAI.isStalking)
        {
            EnemyAI.isStalking = true;
        }
    }

    private IEnumerator ShootDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.25f);
        canShoot = true;
    }

    private IEnumerator MuzzleFlashDelay()
    {
        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.enabled = false;
    }
    
    private IEnumerator DrainStamina()
    {
        stamina--;

        if (stamina <= 0)
        {
            stamina = 0;
        }

        canDrainStamina = false;
        yield return new WaitForSeconds(0.035f);
        canDrainStamina = true;
    }

    private IEnumerator RefillStamina()
    {
        stamina++;

        if (stamina >= staminaMax)
        {
            stamina = staminaMax;
            isStaminaDelayComplete = false;
        }
        canRefillStamina = false;
        yield return new WaitForSeconds(0.75f);
        canRefillStamina = true;
    }

    private IEnumerator StaminaRefillDelay()
    {
        isStaminaDelayComplete = false;
        yield return new WaitForSeconds(3.0f);
        isStaminaDelayComplete = true;
    }

    private IEnumerator ReloadDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(1.5f);
        canShoot = true;
    }
}