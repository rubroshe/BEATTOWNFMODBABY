using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AiMovement : MonoBehaviour
{
    public Tilemap tilemap;
    public float moveSpeed = 3.0f; // Speed of the movement

    private void Awake()
    {
        if (!tilemap)
        {
            Debug.LogError("Tilemap reference not set in AiMovement script.");
        }
    }

    // This method is called to move the AI towards the player
    public void MoveTowardsPlayer(GameObject player)
    {
        if (!player)
        {
            Debug.LogError("Player reference not set for AiMovement.");
            return;
        }

        Vector3Int aiCell = tilemap.WorldToCell(transform.position);
        Vector3Int playerCell = tilemap.WorldToCell(player.transform.position);

        // Calculate direction to player
        Vector3Int directionToPlayer = new Vector3Int(
            Mathf.Clamp(playerCell.x - aiCell.x, -1, 1),
            Mathf.Clamp(playerCell.y - aiCell.y, -1, 1),
            0
        );

        FMOD.Studio.EventInstance RingStep;
        RingStep = FMODUnity.RuntimeManager.CreateInstance("event:/Ring_Sounds/RingStep");
        RingStep.start();




        // Calculate the number of moves needed
        int movesNeeded = Mathf.Max(Mathf.Abs(playerCell.x - aiCell.x), Mathf.Abs(playerCell.y - aiCell.y));
        int movesToMake = Mathf.Min(movesNeeded, 1); // Move only up to 2 cells

        // If AI is already adjacent, no need to move
        if (movesNeeded > 1)
        {
            Vector3Int targetCell = aiCell + directionToPlayer * movesToMake;
            if (!GridManager.Instance.IsCellOccupied(targetCell))
            {
                StartCoroutine(SmoothMoveToCell(targetCell));
            }
            else
            {
                Debug.Log("Target cell is occupied, cannot move AI.");
            }
        }
    }

    private IEnumerator SmoothMoveToCell(Vector3Int targetCell)
    {
        Vector3 targetPosition = tilemap.GetCellCenterWorld(targetCell);
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float fracJourney = 0f;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f) // Ensure close enough
        {
            float distCovered = (Time.time - startTime) * moveSpeed;
            fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, fracJourney);
            yield return null;
        }

        transform.position = targetPosition; // Ensure the AI exactly reaches the target position

        // Optional: Update grid occupancy here if necessary
        GridManager.Instance.SetCellOccupied(tilemap.WorldToCell(startPosition), false);
        GridManager.Instance.SetCellOccupied(targetCell, true);
    }
}
