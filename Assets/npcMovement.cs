using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCFollowInteraction : MonoBehaviour
{
    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Follow Settings")]
    public Transform player;          // Reference to the player
    public float offsetX = -5f;       // Distance behind the player
    public float moveSpeed = 5f;      // How fast the NPC moves

    private bool shouldFollow = false;
    private string currentScene;

    private void Awake()
    {
        // Keep NPC alive between scenes
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("Detective");
        if (npcs.Length > 1)
        {
            Destroy(gameObject); // Destroy duplicates
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            if (shouldFollow)
            {
                transform.position = new Vector3(player.position.x + offsetX, transform.position.y, transform.position.z);
            }
        }
    }




    private void Update()
    {
        if (!gameObject.activeSelf) return;

        if (shouldFollow && player != null)
        {
            Vector3 targetPos = new Vector3(player.position.x + offsetX, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            FlipTowardsPlayer();
        }

    }

    public bool IsFollowing() => shouldFollow;

    // Call this from PlayerMovement when interacting
    public void StartFollowing(float newOffsetX)
    {
        shouldFollow = true;
        offsetX = newOffsetX;
        currentScene = SceneManager.GetActiveScene().name;
    }

    // Optional: stop following
    public void StopFollowing()
    {
        shouldFollow = false;
        currentScene = SceneManager.GetActiveScene().name;
    }

    private void FlipTowardsPlayer()
    {
        if (player == null) return;

        if (player.position.x < transform.position.x && transform.localScale.x > 0)
        {
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
        else if (player.position.x > transform.position.x && transform.localScale.x < 0)
        {
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPos != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        }
    }
}
