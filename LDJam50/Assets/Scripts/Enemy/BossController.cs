using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // Body movement settings
    public Vector2 offset;
    public Vector2 bspeed;
    public static Vector2 bodyOffset;
    public static Vector2 bodySpeed;

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

    ////////// Functions //////////
    private void Awake()
    {
        bodyOffset = offset;
        bodySpeed = bspeed;
        bodyAngleSpeed = angleSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        NextState();
    }

    // Update is called once per frame
    void Update()
    {

        // Rotate towards direction 
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, (target.x - transform.position.x) * angleMagnitude, angleSpeed));
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

        print(info);
        print(methodName);
        print(this);
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

        state = State.Idle;
        NextState();
    }
}
