using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleNPC : MonoBehaviour
{
    public float interactionRange = 2f;   // Distance player must be in to interact
    public Transform player;              // Assign player in Inspector

    private bool isPlayerNear = false;

    void Update()
    {
        if (player == null) return;

        // Check distance to player
        float dist = Vector2.Distance(transform.position, player.position);
        isPlayerNear = dist <= interactionRange;

        // If player presses F while near NPC
        if (isPlayerNear && Keyboard.current.fKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void Interact()
    {
        Debug.Log("NPC interaction triggered!");
        // Here you can call dialogue system, open shop, etc.
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize interaction range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
