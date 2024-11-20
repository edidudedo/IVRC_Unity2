using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro namespace

public class TextManager : MonoBehaviour
{
    public ContentScriptData contentScriptData; // Reference to the ScriptableObject storing SceneContent
    public AudioSource audioSource; // Reference to AudioSource to play the audio
    public float letterDelay = 0.05f; // Time between each letter appearing
    public float textDelay = 0.5f; // Delay between texts once both text and audio are done

    private Coroutine textAnimationCoroutine; // To store the active coroutine

    void Start()
    {
        audioSource.volume = 0.7f;
        // Example: Fetch and animate content (text + audio) for Scene 1 when the game starts
        DisplayContentForScene("0");
    }

    // Method to get and animate the content (text + audio) for any scene
    public void DisplayContentForScene(string sceneName)
    {
        // Get the list of contents (text + audio) for the scene
        List<SceneContent> sceneContents = contentScriptData.GetContentsByScene(sceneName);

        // If there's an active animation coroutine, stop it before starting a new one
        if (textAnimationCoroutine != null)
        {
            StopCoroutine(textAnimationCoroutine);
        }

        // Start the coroutine to display each text and play audio one after the other
        textAnimationCoroutine = StartCoroutine(DisplayMultipleContents(sceneContents));
    }

    // Coroutine to animate multiple contents (text + audio) one after the other
    IEnumerator DisplayMultipleContents(List<SceneContent> contents)
    {
        foreach (SceneContent content in contents)
        {
            // Start both text animation and audio at the same time
            yield return StartCoroutine(AnimateTextAndPlayAudio(content.text, content.audio));

            // Add a 0.05-second delay after both text and audio finish
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Coroutine to animate the text letter by letter and play the audio
    IEnumerator AnimateTextAndPlayAudio(string fullText, AudioClip audioClip)
    {

        // Play the audio (if available)
        if (audioClip != null && audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        // Determine the longest duration (either the text or the audio clip)
        float maxDuration = audioClip != null ? audioClip.length : 0f;


        // Wait until the longer of text or audio has finished
        yield return new WaitForSeconds(maxDuration);
        yield return new WaitForSeconds(0.5f);
    }
}
