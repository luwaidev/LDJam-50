using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public Animator transition;

    [Header("Scene Loading Settings")]
    [SerializeField] float sceneTransitionTime;
    public bool loaded;
    public bool loadingScene;
    public AudioSource menuSound;
    public AudioSource gameSound;

    [Header("Score")]
    public float maxTime;
    public float time;
    public int damageDone;

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("GameController") != gameObject) Destroy(gameObject);

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
    }
    public void Load(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }



    public void LoadWithDelay(string sceneName, float delayTime)
    {
        StartCoroutine(Delay(sceneName, delayTime));
    }

    IEnumerator Delay(string sceneName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(LoadScene(sceneName));
    }
    public IEnumerator LoadScene(string sceneName)
    {
        Debug.Log("Loading Scene");

        if (SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "Infinite")
        {
            time = BossController.instance.actualTime;
        }
        bool inStoryMode = SceneManager.GetActiveScene().name == "Game";

        if (loadingScene) yield break;
        loadingScene = true;

        // sound.Play();
        transition.SetTrigger("Transition"); // Start transitioning scene out
        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition

        // Start loading scene
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        load.allowSceneActivation = false;
        while (!load.isDone)
        {
            if (load.progress >= 0.9f)
            {
                load.allowSceneActivation = true;
            }

            yield return null;
        }
        load.allowSceneActivation = true;



        transition.SetTrigger("Transition"); // Start transitioning scene back

        yield return new WaitForEndOfFrame();
        if (sceneName == "Menu")
        {
            menuSound.Play();
            gameSound.Stop();
        }
        else if (sceneName == "Game" || sceneName == "Infinite")
        {
            menuSound.Stop();
            gameSound.Play();
        }
        else if (sceneName == "End")
        {

            menuSound.Play();
            gameSound.Stop();
            GameObject.Find("time").GetComponent<TMP_Text>().text = "Time - " + time;
            GameObject.Find("score").GetComponent<TMP_Text>().text = "Score - " + (time * damageDone / 2);
            if (inStoryMode)
            {
                // GameObject.Find("survived").GetComponent<TMP_Text>().text = "People Escaped - " + (time * damageDone / 2);
            }
        }
        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition
        loadingScene = false;

        yield return new WaitForSeconds(1);
        instance = this;

        loaded = true;
    }

    private void Update()
    {

        // if (Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
    }


}
