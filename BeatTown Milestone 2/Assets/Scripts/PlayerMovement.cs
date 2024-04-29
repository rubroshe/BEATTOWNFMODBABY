using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Tilemap tilemap;
    public float moveSpeed = 3.0f; // Speed of the movement

    public void MoveToCell(Vector3Int cellPosition)
    {
        Vector3 targetPosition = tilemap.GetCellCenterWorld(cellPosition);
        StartCoroutine(SmoothMoveToCell(cellPosition)); // Call the correctly named coroutine

        FMOD.Studio.EventInstance RingStep;
        RingStep = FMODUnity.RuntimeManager.CreateInstance("event:/Ring_Sounds/RingStep");
        RingStep.start();




    }

    public IEnumerator SmoothMoveToCell(Vector3Int targetCell)
    {
        if (!GridManager.Instance.IsCellOccupied(targetCell))
        {
            Vector3 targetPosition = tilemap.GetCellCenterWorld(targetCell);
            float startTime = Time.time;
            Vector3 startPosition = transform.position;
            float journeyLength = Vector3.Distance(startPosition, targetPosition);
            float fracJourney = 0f;

            while (fracJourney < 1f)
            {
                float distCovered = (Time.time - startTime) * moveSpeed;
                fracJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(startPosition, targetPosition, fracJourney);
                yield return null;
            }

            // Update the grid occupancy
            Vector3Int oldCell = tilemap.WorldToCell(startPosition);
            GridManager.Instance.SetCellOccupied(oldCell, false);
            GridManager.Instance.SetCellOccupied(targetCell, true);
        }
        else
        {
            Debug.Log("Move failed, cell is occupied.");
        }
    }
}
