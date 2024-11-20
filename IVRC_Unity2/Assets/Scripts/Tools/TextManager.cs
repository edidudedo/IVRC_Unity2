using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro namespace

public class TextManager : MonoBehaviour
{
    public ContentScriptData contentScriptData; // Reference to the ScriptableObject storing SceneContent
    public TextMeshProUGUI panelText; // Reference to TextMeshProUGUI component in the Canvas
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
        // Set up the text
        panelText.text = fullText;
        panelText.maxVisibleCharacters = 0; // Start with no characters visible

        // Play the audio (if available)
        if (audioClip != null && audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        // Calculate the total time it will take for the text to finish displaying
        float textDuration = fullText.Length * letterDelay;

        // Determine the longest duration (either the text or the audio clip)
        float maxDuration = Mathf.Max(textDuration, audioClip != null ? audioClip.length : 0f);

        // Animate the text letter by letter
        for (int i = 0; i <= fullText.Length; i++)
        {
            panelText.maxVisibleCharacters = i; // Reveal the next character
            yield return new WaitForSeconds(letterDelay); // Wait before revealing the next character
        }

        // Wait until the longer of text or audio has finished
        yield return new WaitForSeconds(maxDuration - textDuration);
        yield return new WaitForSeconds(0.5f);
    }
}
