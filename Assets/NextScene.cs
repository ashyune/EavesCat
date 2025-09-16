using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class AnyKeyToLoad_NewInput : MonoBehaviour
{
    public string nextSceneName = "Room1";
    bool loading = false;

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        // keyboard key OR any real input device button pressed this frame
        if (!loading && (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame
                         || Pointer.current != null && Pointer.current.press.isPressed))
        {
            loading = true;
            SceneManager.LoadScene(nextSceneName);
        }
#else
        // fallback if new input system not enabled
        if (!loading && Input.anyKeyDown)
        {
            loading = true;
            SceneManager.LoadScene(nextSceneName);
        }
#endif
    }
}
