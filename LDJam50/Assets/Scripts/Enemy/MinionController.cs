using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class MinionController : MonoBehaviour
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

    [Header("Movement Settings")]
    public float speed;
    public float turnSpeed;
    public float angle;


    [Header("Effects")]
    public GameObject hitParticle;
    public MMFeedbacks hitFeedback;

    [Header("Following")]
    public Transform minionAhead;

    ///////// Unity Functions //////////
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerPosition = PlayerController.player.transform.position;
        Vector2 playerDirection = playerPosition - (Vector2)transform.position;

        angle = Mathf.LerpAngle(angle, Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg, turnSpeed);
        transform.eulerAngles = new Vector3(0, 0, angle);

        rb.velocity = ((playerPosition + Vector2.down * 5) - (Vector2)transform.position).normalized * speed;

        if (transform.position.y < -5.7f)
        {
            Destroy(gameObject);
        }
    }

    ////////// State //////////

    // void NextState()
    // {
    //     string methodName = state.ToString() + "State";

    //     // Get method
    //     System.Reflection.MethodInfo info =
    //         GetType().GetMethod(methodName,
    //                             System.Reflection.BindingFlags.NonPublic |
    //                             System.Reflection.BindingFlags.Instance);

    //     print(info);
    //     print(methodName);
    //     print(this);
    //     StartCoroutine((IEnumerator)info.Invoke(this, null)); // Call the next state
    // }


    // [Header("Idle State")]
    // public Vector2 yOffset;
    // public Vector2 xOffset;
    // public float idleSpeed;
    // public Vector2 delayTime;
    // IEnumerator IdleState()
    // {
    //     while (state == State.Follow)
    //     {
    //         if (Vector2.Distance(transform.position, target) < 0.5f)
    //         {
    //             // Set new x offset
    //             target.x = Random.Range(xOffset.x, xOffset.y) * -Mathf.Sign(target.x);

    //             // Set new y offset
    //             target.y = Random.Range(yOffset.x, yOffset.y);

    //             yield return new WaitForSeconds(Random.Range(delayTime.x, delayTime.y));
    //         }

    //         // Move to position 
    //         transform.position = Vector2.MoveTowards(transform.position, target, idleSpeed);
    //         yield return null;
    //     }

    //     NextState();
    // }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bullet")
        {
            if (hitFeedback == null) hitFeedback = GameObject.Find("Boss Hit").GetComponent<MMFeedbacks>();
            hitFeedback.PlayFeedbacks();
            Instantiate(hitParticle, other.transform.position, other.transform.rotation);
            Destroy(other.gameObject);
            Destroy(gameObject);

        }

    }
}
