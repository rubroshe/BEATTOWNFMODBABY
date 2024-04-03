using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap

    // This method now takes a cell position instead of directly taking a world position
    public void MoveToCell(Vector3Int cellPosition)
    {
        Vector3 cellCenterPosition = tilemap.GetCellCenterWorld(cellPosition); // Get the world position of the cell center
        transform.position = cellCenterPosition; // Move the player to the cell center
    }
}
