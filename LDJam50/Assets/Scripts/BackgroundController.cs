using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public float speed;
    public Transform[] background;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < background.Length; i++)
        {
            background[i].position = new Vector2(0, background[i].position.y + speed);

            if (background[i].position.y >= 12)
            {
                background[i].position = new Vector2(0, -12);
            }
        }
    }
}
