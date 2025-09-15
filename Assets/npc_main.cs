using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCFollowPlayer : MonoBehaviour
{
    public float followDistance = 2f;   // How far behind the player the NPC stays
    public float followSpeed = 3f;      // How fast the NPC moves toward player
    private Transform player;

    private void Awake()
    {
        // Keep this NPC alive between scenes
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        if (npcs.Length > 1)
        {
            Destroy(gameObject); // Destroy duplicates
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Subscribe to sceneLoaded so we can re-find the player
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find the player when a new scene loads
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;

            // Place NPC near player when scene starts
            transform.position = player.position + new Vector3(-followDistance, 0, 0);
        }
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 targetPos = player.position + new Vector3(-followDistance, 0, 0);

            // Smooth follow
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * followSpeed
            );
        }
    }
}
