using UnityEngine;

public static class NPCStateManager
{
    // Is the NPC currently following the player?
    public static bool isFollowing = false;

    // Last saved position (world space)
    public static Vector3 lastPosition = Vector3.zero;

    // The scene name where the NPC was last placed
    public static string lastScene = "";
}
