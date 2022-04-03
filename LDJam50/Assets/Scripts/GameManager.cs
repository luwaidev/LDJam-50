using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public void LoadNext()
    {
        string sceneName = (Int32.Parse(SceneManager.GetActiveScene().name) + 1).ToString();
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

        if (sceneName == "Menu")
        {
            menuSound.Play();
            gameSound.Stop();
        }
        else if (sceneName == "Game")
        {
            menuSound.Stop();
            gameSound.Play();
        }

        transition.SetTrigger("Transition"); // Start transitioning scene back

        yield return new WaitForEndOfFrame();
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
