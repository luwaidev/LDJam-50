using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class ChargeEnemy : MonoBehaviour
{
    public enum State
    {
        Follow,
        Following,
        Charge
    }
    public State state;
    public Vector2 target;
    public Rigidbody2D rb;
    public int health;

    [Header("Movement Settings")]
    public float speed;
    public float turnSpeed;
    public float angle;

    [Header("Following")]
    public Transform minionAhead;


    [Header("Effects")]
    public GameObject hitParticle;
    public MMFeedbacks hitFeedback;

    ///////// Unity Functions //////////
    // Start is called before the first frame update
    void Start()
    {
        NextState();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerDirection = PlayerController.player.transform.position - transform.position;

        angle = Mathf.LerpAngle(angle, Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg, turnSpeed);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    ////////// State //////////
    void NextState()
    {
        string methodName = state.ToString() + "State";

        // Get method
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);

        StartCoroutine((IEnumerator)info.Invoke(this, null)); // Call the next state
    }


    IEnumerator FollowState()
    {
        while (state == State.Follow)
        {

            // Move to position 
            Vector2 direction = PlayerController.player.transform.position - transform.position;
            rb.velocity = direction.normalized * chargeSpeed;

            Vector2 pp = PlayerController.player.transform.position;
            if (pp.y < transform.position.y && Vector2.Distance(pp, transform.position) < 1f)
            {
                state = State.Charge;
            }
            yield return null;
        }

        NextState();
    }

    public float chargeSpeed;
    IEnumerator ChargeState()

    {
        Vector2 direction = PlayerController.player.transform.position - transform.position;
        rb.velocity = direction.normalized * chargeSpeed;
        while (transform.position.y > -5.5)
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bullet")
        {
            if (hitFeedback == null) hitFeedback = GameObject.Find("Boss Hit").GetComponent<MMFeedbacks>();
            hitFeedback.PlayFeedbacks();
            Instantiate(hitParticle, other.transform.position, other.transform.rotation);
            Destroy(other.gameObject);
            GameManager.instance.damageDone++;
            health--;
            if (health <= 0)
            {
                Destroy(gameObject);
            }

        }

    }
}
