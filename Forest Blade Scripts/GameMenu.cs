using UnityEngine;
using UnityEditor;

public class GameMenu : MonoBehaviour
{
    public void QuitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
