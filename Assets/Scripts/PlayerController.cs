using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody rb;
    private Animator animator;

    private float moveHorizontal;
    private float moveVertical;
    private float moveSpeed = 4f;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");

        RotateCharacterToMouse();

        // Movement animations

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
        }
        if (moveVertical != 0 || moveHorizontal != 0)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
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
}
