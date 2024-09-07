using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneContent
{
    [TextArea(3, 10)]
    public string text; // The text associated with this scene
    public AudioClip audio; // The MP3 or audio file associated with this scene
}
