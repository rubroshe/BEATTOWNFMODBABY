using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPunch : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap for calculating positions
    public int health = 10; // Health of the player

    // Call this method to attempt a punch on a target player
    public void TryPunch(GameObject target)
    {
        Vector3Int attackerPosition = tilemap.WorldToCell(transform.position);
        Vector3Int targetPosition = tilemap.WorldToCell(target.transform.position);

        // Check if the target is adjacent to the attacker
        if (Mathf.Abs(attackerPosition.x - targetPosition.x) <= 1 && Mathf.Abs(attackerPosition.y - targetPosition.y) <= 1)
        {
            // Target is adjacent, perform the punch
            target.GetComponent<PlayerPunch>().health -= 3; // Deduct health
            Debug.Log($"Punched {target.name}. Remaining Health: {target.GetComponent<PlayerPunch>().health}");

            // Move the target away from the attacker
            Vector3Int moveDirection = targetPosition - attackerPosition;
            Vector3Int newTargetPosition = targetPosition + moveDirection;
            target.transform.position = tilemap.GetCellCenterWorld(newTargetPosition);
        }
        else
        {
            Debug.Log("Target is not adjacent. Punch failed.");
        }
    }
}
