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
    public AudioSource growl;
    public AudioSource attack;

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

    [Header("Enemies")]
    public Transform[] spawnPositions;
    public GameObject minion;
    public GameObject chargeEnemy;

    [Header("Time Settings")]
    private float maxTime;
    public float time;
    public float actualTime;
    public float damageMultiplier;

    public float attackMultiplier;
    public bool hit;
    public Transform timeIndicator;
    public Transform timeBar;

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
        GameManager.instance.maxTime = time;
        time = 0;
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        if (!hit)
        {
            time++;
        }
        else if (state == State.Charge)
        {
            time += attackMultiplier;
        }
        else
        {
            time += damageMultiplier;
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


        ProgressBar();

        if (time > maxTime)
        {
            GameManager.instance.Load("End");
        }

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bullet")
        {
            GameManager.instance.damageDone++;
            damage++;
            hit = true;
            hitFeedback.PlayFeedbacks();
            Instantiate(hitParticle, other.transform.position, other.transform.rotation);
            Destroy(other.gameObject);
            if (damage % 5 == 0)
            {
                int spawnPoint = Random.Range(0, 1);
                Instantiate(chargeEnemy, spawnPositions[spawnPoint].position, Quaternion.identity);
            }
            else if (damage % 4 == 0)
            {
                int spawnPoint = Random.Range(0, 1);
                Instantiate(minion, spawnPositions[spawnPoint].position, Quaternion.identity);
            }

        }

    }

    public void ProgressBar()
    {
        timeIndicator.position = new Vector2(time / maxTime * 11 - 5.5f, 4.6f);
        timeBar.localScale = new Vector2(time / maxTime, 0.1f);
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
            if (damage >= 40)
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
        growl.Play();
        float time = 0;
        while (time < chargeWaitTime)
        {
            yield return null;

            target.x = PlayerController.player.transform.position.x;

            // Set y offset
            target.y = 5f;

            // Move to position 
            transform.position = Vector2.MoveTowards(transform.position, target, chargeWaitSpeed);

            time += Time.deltaTime;
        }

        attack.Play();
        // Set y position
        target.y = chargeYLimit - 1;
        while (transform.position.y > chargeYLimit)
        {
            // Move to position 
            transform.position = Vector2.Lerp(transform.position, target, chargeSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(rechargeSpeed);

        // Set new x offset
        target.x = Random.Range(xOffset.x, xOffset.y) * -Mathf.Sign(target.x);

        // Set new y offset
        target.y = Random.Range(yOffset.x, yOffset.y);

        while (transform.position.y < target.y - 1.5f)
        {
            // Move to position 
            transform.position = Vector2.Lerp(transform.position, target, chargeReturnSpeed);
            yield return null;
        }
        damage = 0;
        state = State.Idle;
        NextState();
    }

    [Header("Spawning Settings")]
    public int minSpawns;
    public int maxSpawns;
    public float spawnTime;
    IEnumerator SpawnWaves()
    {
        yield return null;

        for (int i = 0; i < Random.Range(minSpawns, maxSpawns); i++)
        {
            int spawnPoint = Random.Range(1, 2);
            if (i % 3 == 0)
            {
                Instantiate(chargeEnemy, spawnPositions[spawnPoint - 1].position, Quaternion.identity);
            }

            Instantiate(minion, spawnPositions[spawnPoint - 1].position, Quaternion.identity);
            yield return new WaitForSeconds(spawnTime);
        }
    }
}
