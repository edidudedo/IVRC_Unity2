using System.Collections.Generic;
using UnityEngine;

// Define the structure for storing scene-specific text data
[System.Serializable]
public class SceneText
{
    public string sceneName; // The name of the scene
    [TextArea(3, 10)] // Optional: Makes it easier to edit multi-line text in the Inspector
    public string text; // The text associated with this scene
}

// Create a ScriptableObject to store the texts for multiple scenes
[CreateAssetMenu(fileName = "NewTextScript", menuName = "ScriptableObjects/TextScript")]
public class TextScriptData : ScriptableObject
{
    public List<SceneText> sceneTexts; // List of texts for different scenes

    // Get the text by scene name
    public string GetTextByScene(string sceneName)
    {
        foreach (SceneText sceneText in sceneTexts)
        {
            if (sceneText.sceneName == sceneName)
            {
                return sceneText.text;
            }
        }
        return "Text not found for scene: " + sceneName; // Return a default message if not found
    }
}
