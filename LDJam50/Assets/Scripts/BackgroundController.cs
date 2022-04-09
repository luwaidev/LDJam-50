using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public BossController bc;
    public float speed;
    public Transform[] background;

    private void Start()
    {
        if (BossController.instance != null) bc = BossController.instance;
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < background.Length; i++)
        {
            if (bc != null)
            {
                if (bc.hit) background[i].position = new Vector2(0, background[i].position.y + speed * bc.damageMultiplier * Time.deltaTime);
                else background[i].position = new Vector2(0, background[i].position.y + speed * Time.deltaTime);

                if (background[i].position.y >= 12)
                {
                    background[i].position = new Vector2(0, -12);
                }
            }

            else
            {
                background[i].position = new Vector2(0, background[i].position.y + speed * Time.deltaTime);

                if (background[i].position.y >= 12)
                {
                    background[i].position = new Vector2(0, -12);
                }
            }

        }
    }
}
