using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AiPunch : MonoBehaviour
{
    public Tilemap tilemap;
    public int health = 10; // Health of the AI
    public float knockbackSpeed = 3.0f; // Speed at which the target is knocked back

    public void TryPunchIfInRange(GameObject target)
    {
        Vector3Int aiPosition = tilemap.WorldToCell(transform.position);
        Vector3Int targetPosition = tilemap.WorldToCell(target.transform.position);

        if (Mathf.Abs(aiPosition.x - targetPosition.x) <= 1 && Mathf.Abs(aiPosition.y - targetPosition.y) <= 1)
        {
            // Target is adjacent, perform the punch
            target.GetComponent<PlayerPunch>().health -= 3; // Deduct health from the player
            Debug.Log($"AI punched {target.name}. Remaining Health: {target.GetComponent<PlayerPunch>().health}");
            FMOD.Studio.EventInstance Player_Hurt;
            Player_Hurt = FMODUnity.RuntimeManager.CreateInstance("event:/Ring_Sounds/Player_Hurt");
            Player_Hurt.start();





            // Calculate new position for knockback
            Vector3Int moveDirection = targetPosition - aiPosition;
            Vector3Int newTargetPosition = targetPosition + moveDirection;
            Vector3 newWorldPosition = tilemap.GetCellCenterWorld(newTargetPosition);

            // Start coroutine to move the target smoothly
            StartCoroutine(SmoothMove(target, newWorldPosition, newTargetPosition));
        }
        else
        {
            Debug.Log("Target is not adjacent. Punch failed.");
        }
    }

    // Coroutine for smooth movement
    private IEnumerator SmoothMove(GameObject target, Vector3 newWorldPosition, Vector3Int newCellPosition)
    {
        if (!GridManager.Instance.IsCellOccupied(newCellPosition))
        {
            Vector3 startPosition = target.transform.position;
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(startPosition, newWorldPosition);

            while (Vector3.Distance(target.transform.position, newWorldPosition) > 0.01f)
            {
                float distCovered = (Time.time - startTime) * knockbackSpeed;
                float fracJourney = distCovered / journeyLength;
                target.transform.position = Vector3.Lerp(startPosition, newWorldPosition, fracJourney);
                yield return null;
            }
            target.transform.position = newWorldPosition; // Ensure target exactly reaches the new position

            // Update the grid occupancy
            GridManager.Instance.SetCellOccupied(tilemap.WorldToCell(startPosition), false);
            GridManager.Instance.SetCellOccupied(newCellPosition, true);
        }
        else
        {
            Debug.Log("Knockback failed, cell is occupied.");
        }
    }
}
