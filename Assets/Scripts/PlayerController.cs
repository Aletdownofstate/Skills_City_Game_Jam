using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody rb;
    private Animator animator;

    [SerializeField] private AudioSource gunshotSound;
    [SerializeField] private Light muzzleFlash;

    private float moveHorizontal;
    private float moveVertical;
    private float moveSpeed = 4f;

    private bool isIdle;
    private bool isRunning = false;
    private bool isAiming = false;

    public LayerMask enemyLayer;
    private bool canShoot = true;
    public float shootRange = 100f;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponentInChildren<Animator>();
        mainCamera = Camera.main;

        isIdle = true;
        muzzleFlash.enabled = false;
    }

    private void Update()
    {
        // Inputs
        // Movement

        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");
        
        // Running

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = false;
            moveSpeed = 4f;
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

        if (Input.GetMouseButton(0) && isAiming && canShoot)
        {
            Shoot();
            StartCoroutine(MuzzleFlashDelay());
            gunshotSound.Play();
            StartCoroutine(ShootDelay());
        }

        RotateCharacterToMouse();       

        // Movement animations
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

        if (isRunning && rb.velocity.x >= 0.01f || rb.velocity.y >= 0.01f)
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

}
