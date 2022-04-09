using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPointController : MonoBehaviour
{
    public Animator[] anim;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnHealthChange()
    {
        for (int i = 5; i >= 1; i--)
        {
            if (PlayerController.player.health <= i)
            {
                anim[i - 1].SetBool("Dead", true);
            }
        }


    }
}
