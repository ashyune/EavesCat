using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    private float horizontalMovement;
    private bool isFacingRight = false; // Cat faces left by default
    private Animator anim;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Interaction / Dialogue")]
    public GameObject chatBox;        // UI Panel
    public TMP_Text dialogueText;     // Text element inside panel
    public TMP_Text nameText;         // Optional: NPC name
    private bool isNearNPC = false;
    private bool isChatActive = false;

    [Header("NPC Interaction")]
    public NPCFollowInteraction npc;  // Reference to NPC script

    private NPCDialogue currentNPCDialogue;
    private int currentLine = 0;


    private void Start()
    {
        anim = GetComponent<Animator>();

        // Flip sprite to face left at start if needed
        if (!isFacingRight)
        {
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
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
        if (npc == null)
        {
            npc = (NPCFollowInteraction)FindFirstObjectByType(typeof(NPCFollowInteraction));
        }
    }

    // --- MOVEMENT ---
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);

        GroundCheck();
        Flip();

        // Animation
        if (anim != null)
            anim.SetBool("IsMove", horizontalMovement != 0);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        horizontalMovement = input.x;
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
        }

    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {

            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
            }

            else if (context.canceled)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
            }
        }
    }

    private void Flip()
    {
        if ((isFacingRight && horizontalMovement > 0) || (!isFacingRight && horizontalMovement < 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
            if(npc != null && npc.IsFollowing())
            {
                npc.StartFollowing(isFacingRight ? 5f : -5f);
            }
        }
    }

    // --- INTERACTION ---
    private void Update()
    {
        if (isNearNPC)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                // Dialogue
                if (!isChatActive)
                {
                    StartDialogue();
                }
                else
                {
                    NextLine();
                }
            }
            else if (Keyboard.current.eKey.wasPressedThisFrame) {
                // Make NPC follow behind player
                if (npc != null && !npc.IsFollowing())
                {
                    npc.StartFollowing(isFacingRight ? -5f : 5f);
                } else if (npc != null && npc.IsFollowing())
                {
                    npc.StopFollowing();
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC") || other.CompareTag("Detective"))
        {
            isNearNPC = true;
            currentNPCDialogue = other.GetComponent<NPCDialogue>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC") || other.CompareTag("Detective"))
        {
            isNearNPC = false;
            EndDialogue();
            currentNPCDialogue = null;
        }
    }

    private void StartDialogue()
    {
        if (currentNPCDialogue == null) return;

        chatBox.SetActive(true);
        nameText.text = currentNPCDialogue.dialogueLines[0].speakerName;
        currentLine = 0;
        dialogueText.text = currentNPCDialogue.dialogueLines[currentLine].line;
        isChatActive = true;
    }

    private void NextLine()
    {
        currentLine++;
        if (currentNPCDialogue != null && currentLine < currentNPCDialogue.dialogueLines.Length)
        {
            dialogueText.text = currentNPCDialogue.dialogueLines[currentLine].line;
        }
        else
        {
            EndDialogue();
        }
    }


    private void EndDialogue()
    {
        chatBox.SetActive(false);
        isChatActive = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}
