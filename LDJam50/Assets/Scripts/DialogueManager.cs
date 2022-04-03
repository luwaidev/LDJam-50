using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dialogue system using 
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager i;
    public GameObject textBox;
    public Transform[] textObjects;
    public int currentText;
    public int currentSubText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartText(Transform[] text)
    {
        textObjects = text;
    }

    public void OnTextEnd()
    {
        // Advance the subtext
        currentSubText++;

        // if at the end of current text, set to next text
        if (currentSubText >= textObjects[currentText].childCount)
        {
            // Hide old text
            textObjects[currentText].GetChild(currentSubText).gameObject.SetActive(false);

            // Update new position
            currentSubText = 0;
            currentText++;
        }

        // if at the end of all the text, end
        if (currentText >= textObjects.Length)
        {
            EndDialogue();
            return;
        }

        // Show next text
        textObjects[currentText].GetChild(currentSubText).gameObject.SetActive(true);
    }

    private void EndDialogue()
    {
        textBox.SetActive(false);
        currentText = 0;
        currentSubText = 0;
    }
}
