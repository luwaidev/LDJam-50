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

    public Vector2 maxPosition;
    // Movement Settings
    [SerializeField] private float speed;
    private Vector2 input;
    public Vector2 mouseDirection;

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
        StartCoroutine(Shoot());
    }


    public void OnLook()
    {
        var mouse = Mouse.current; // Get mouse
        Vector2 mousePositionToPlayer = ((Vector2)Camera.main.ScreenToWorldPoint(mouse.position.ReadValue()) - (Vector2)transform.position); // Get mouse positions

        // mouseAngle = Mathf.Atan2(mousePositionToPlayer.x, mousePositionToPlayer.y);
        mouseDirection = mousePositionToPlayer.normalized;

        // Set position
        shootPosition.localPosition = mouseDirection;
        shootPosition.eulerAngles = new Vector3(0, 0, Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg);
    }

    public void OnDash()
    {

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
        rb.velocity = input * speed;
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

    //////////////// Functions ///////////////
}
