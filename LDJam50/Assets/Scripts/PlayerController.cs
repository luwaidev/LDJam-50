using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController player;
    public Rigidbody2D rb;
    public Transform shootPosition;
    public GameObject bullet;
    [Header("Afterimage")]
    public Transform afterImage;
    public Vector2 afterImagePosition;
    public float afterImageSpeed;

    [Header("State")]
    public bool movementLocked;
    public bool invincible;

    [Header("Movement Settings")]
    // Movement Settings
    private float angle;
    public float turnSpeed;
    [SerializeField] private float speed;
    private Vector2 input;
    private Vector2 mouseDirection;
    public float mouseSpeed;

    [Header("Dash Settings")]
    public float dashSpeed;
    public float dashTime;

    [Header("Hit Settings")]
    public float knockbackSpeed;
    public float knockbackTime;

    // Combat settings
    public bool firing;
    [SerializeField] float fireSpeed;
    public int damage;

    //////////////// INPUT ///////////////
    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>(); // Get Vector2 input
        // direction = (Mathf.Abs(input.x) >= Mathf.Abs(input.y) ? "Side" : (input.y > 0 ? "Up" : "Down"));
    }

    // Called when fire button is pressed
    public void OnFire(InputValue value)
    {
        if (!movementLocked) StartCoroutine(Shoot());
    }


    public void OnLook()
    {
        var mouse = Mouse.current; // Get mouse
        Vector2 mousePositionToPlayer = ((Vector2)Camera.main.ScreenToWorldPoint(mouse.position.ReadValue()) - (Vector2)transform.position); // Get mouse positions

        angle = Mathf.LerpAngle(angle,
            Mathf.Atan2(mousePositionToPlayer.y, mousePositionToPlayer.x) * Mathf.Rad2Deg, turnSpeed);
        transform.eulerAngles = new Vector3(0, 0, angle);

        mouseDirection = mousePositionToPlayer.normalized;

    }

    public void OnDash()
    {
        if (!movementLocked) StartCoroutine(Dash());
    }

    //////////////// Unity Functions ///////////////

    private void Awake()
    {
        player = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!movementLocked)
        {
            // Setting movement velocity
            Vector2 directionBonus = input == Vector2.zero ? Vector2.zero : mouseDirection * mouseSpeed;
            rb.velocity = input * speed + directionBonus;
        }

        AfterImage();
    }

    //////////////// Coroutines ///////////////
    public IEnumerator Shoot()
    {
        if (firing) yield break;
        firing = true;
        Instantiate(bullet, shootPosition.transform.position, shootPosition.rotation);
        yield return new WaitForSeconds(fireSpeed);
        firing = false;
    }

    public IEnumerator Hit()
    {
        movementLocked = true;
        yield return new WaitForSeconds(knockbackTime);
        movementLocked = false;
    }

    public IEnumerator Dash()
    {
        rb.velocity = dashSpeed * mouseDirection;
        movementLocked = true;
        yield return new WaitForSeconds(dashTime);
        movementLocked = false;
        // Setting movement velocity
        Vector2 directionBonus = input == Vector2.zero ? Vector2.zero : mouseDirection * mouseSpeed;
        rb.velocity = input * speed + directionBonus;
    }

    //////////////// Functions ///////////////

    public void OnHit(Vector2 position)
    {
        rb.velocity = -(position - (Vector2)transform.position).normalized * knockbackSpeed;
    }

    private void AfterImage()
    {
        afterImage.position = Vector2.Lerp(afterImage.position, (Vector2)transform.position + afterImagePosition, afterImageSpeed);
        afterImage.rotation = Quaternion.Lerp(afterImage.rotation, transform.rotation, afterImageSpeed);
    }
}
