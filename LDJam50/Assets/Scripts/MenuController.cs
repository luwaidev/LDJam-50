using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public Transform[] dialogue;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowMenu()
    {
        GetComponent<Animator>().SetTrigger("Out");
    }
    public void OnInfinite()
    {
        // Debug.Log("Loading Scene");
        // DialogueManager.i.StartText(dialogue);
        // GetComponent<Animator>().SetTrigger("Out");
        GameManager.instance.LoadWithDelay("Infinite", 0.8f);
        anim.Play("Mouth Open");

    }

    public void OnTimed()
    {
        // Debug.Log("Loading Scene");
        // DialogueManager.i.StartText(dialogue);
        // GetComponent<Animator>().SetTrigger("Out");
        GameManager.instance.LoadWithDelay("Game", 0.8f);
        anim.Play("Mouth Open");

    }

    public void ReturnToMainMenu()
    {
        GameManager.instance.Load("Main Menu");
    }
}
