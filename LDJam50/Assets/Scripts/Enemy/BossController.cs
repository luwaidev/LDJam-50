using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class BossController : MonoBehaviour
{
    // Body movement settings
    public Vector2 offset;
    public Vector2 bspeed;
    public static Vector2 bodyOffset;
    public static Vector2 bodySpeed;

    public static BossController instance;

    [Header("Effects")]
    public GameObject hitParticle;
    public MMFeedbacks hitFeedback;

    [Header("Health")]
    public int maxHealth;
    public static int health;

    public enum State
    {
        Idle,
        Charge
    }

    [Header("General")]
    public State state;
    public Vector2 target;
    public float angleMagnitude;
    public float angleSpeed;
    public static float bodyAngleSpeed;
    public int damage;

    [Header("Time Settings")]
    private float maxTime;
    public float time;
    public float actualTime;
    public float damageMultiplier;

    public float attackMultiplier;
    public bool hit;

    [Header("Evacuation Stats")]
    public float baseEvacuations;

    ////////// Functions //////////
    private void Awake()
    {
        bodyOffset = offset;
        bodySpeed = bspeed;
        bodyAngleSpeed = angleSpeed;
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        NextState();
        maxTime = time;
    }

    IEnumerator Timer()
    {
        if (!hit)
        {
            time--;
        }
        else if (state == State.Charge)
        {
            time -= attackMultiplier;
        }
        else
        {
            time -= damageMultiplier;
        }
        actualTime++;
        yield return new WaitForSeconds(1);
        hit = false;
        StartCoroutine(Timer());
    }

    // Update is called once per frame
    void Update()
    {

        // Rotate towards direction 
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, (target.x - transform.position.x) * angleMagnitude, angleSpeed));

        if (!hit)
        {
            time -= Time.deltaTime;
        }
        else
        {
            time -= Time.deltaTime * damageMultiplier;
        }


    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bullet")
        {
            damage++;
            hit = true;
            hitFeedback.PlayFeedbacks();
            Instantiate(hitParticle, other.transform.position, other.transform.rotation);
            Destroy(other.gameObject);
        }

    }

    public void ProgressBar()
    {

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


    [Header("Idle State")]
    public Vector2 yOffset;
    public Vector2 xOffset;
    public float idleSpeed;
    public Vector2 delayTime;
    IEnumerator IdleState()
    {
        while (state == State.Idle)
        {
            if (Vector2.Distance(transform.position, target) < 0.1f)
            {
                target = transform.position;
                yield return new WaitForSeconds(Random.Range(delayTime.x, delayTime.y));
                // Set new x offset
                target.x = Random.Range(xOffset.x, xOffset.y) * -Mathf.Sign(target.x);

                // Set new y offset
                target.y = Random.Range(yOffset.x, yOffset.y);

            }

            // Move to position 
            transform.position = Vector2.MoveTowards(transform.position, target, idleSpeed);
            yield return null;

            // check if should attack
            if (damage >= 35)
            {
                state = State.Charge;
            }
        }


        NextState();
    }

    [Header("Charge attack settings")]
    public float chargeWaitTime;
    public float chargeWaitSpeed;
    public float chargeSpeed;
    public float chargeReturnSpeed;

    public float chargeYLimit;
    public float rechargeSpeed;
    IEnumerator ChargeState()
    {
        float time = 0;
        while (time < chargeWaitTime)
        {
            yield return null;

            target.x = PlayerController.player.transform.position.x;

            // Set y offset
            target.y = Random.Range(yOffset.x, yOffset.y);

            // Move to position 
            transform.position = Vector2.MoveTowards(transform.position, target, chargeWaitSpeed);

            time += Time.deltaTime;
        }

        // Set y position
        target.y = chargeYLimit - 1;
        while (transform.position.y > chargeYLimit)
        {
            // Move to position 
            transform.position = Vector2.Lerp(transform.position, target, chargeSpeed);
        }

        yield return new WaitForSeconds(rechargeSpeed);

        // Set new x offset
        target.x = Random.Range(xOffset.x, xOffset.y) * -Mathf.Sign(target.x);

        // Set new y offset
        target.y = Random.Range(yOffset.x, yOffset.y);

        while (transform.position.y < target.y)
        {
            // Move to position 
            transform.position = Vector2.Lerp(transform.position, target, chargeReturnSpeed);
        }
        damage = 0;
        state = State.Idle;
        NextState();
    }
}
