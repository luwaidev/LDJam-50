using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
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
        GameManager.instance.LoadWithDelay("Game", 1);

    }

    public void ReturnToMainMenu()
    {
        GameManager.instance.Load("Main Menu");
    }
}
