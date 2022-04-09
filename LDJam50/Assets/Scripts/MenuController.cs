using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public Transform[] dialogue;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlay()
    {
        // Debug.Log("Loading Scene");
        DialogueManager.i.StartText(dialogue);
        GetComponent<Animator>().SetTrigger("Out");

    }

    public void ReturnToMainMenu()
    {
        GameManager.instance.Load("Main Menu");
    }
}
