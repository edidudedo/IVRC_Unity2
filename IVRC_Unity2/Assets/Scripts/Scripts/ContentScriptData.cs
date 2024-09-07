using System.Collections.Generic;
using UnityEngine;

// Define the structure for storing scene-specific content (text and audio)
[System.Serializable]
public class SceneContentList
{
    public string sceneName; // The name of the scene
    public List<SceneContent> contents; // List of content (text and audio) associated with this scene
}

// Create a ScriptableObject to store the texts and audio for multiple scenes
[CreateAssetMenu(fileName = "NewContentScript", menuName = "ScriptableObjects/ContentScript")]
public class ContentScriptData : ScriptableObject
{
    public List<SceneContentList> sceneContents; // List of scene-specific content

    // Get all content (text and audio) by scene name
    public List<SceneContent> GetContentsByScene(string sceneName)
    {
        foreach (SceneContentList sceneContent in sceneContents)
        {
            if (sceneContent.sceneName == sceneName)
            {
                return sceneContent.contents; // Return all content for the scene
            }
        }
        return new List<SceneContent>(); // Return an empty list if the scene is not found
    }
}
