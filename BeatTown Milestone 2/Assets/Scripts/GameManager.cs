using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    private GameObject currentPlayer, targetPlayer;
    public Button moveButton;
    public Button punchButton;
    public Button endTurnButton;
    public Tilemap tilemap;

    private bool isMoveModeActive = false;
    private bool isPunchModeActive = false;

    void Start()
    {
        currentPlayer = player1;
        SetupButtonListeners();
    }

    void SetupButtonListeners()
    {
        moveButton.onClick.AddListener(() => { isMoveModeActive = true; isPunchModeActive = false; });
        punchButton.onClick.AddListener(() => { isPunchModeActive = true; isMoveModeActive = false; });
        endTurnButton.onClick.AddListener(OnEndTurnClicked);
    }

    void OnEndTurnClicked()
    {
        EndTurn();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

            if (isMoveModeActive)
            {
                currentPlayer.GetComponent<PlayerMovement>().MoveToCell(cellPosition);
                isMoveModeActive = false; // Exit move mode
            }
            if (isPunchModeActive && Input.GetMouseButtonDown(0)) // Left mouse click
            {
                AttemptPunch(Input.mousePosition);
            }
        }
    }

    private void AttemptPunch(Vector3 mousePosition)
    {
        // Convert mouse position to world position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 rayPosition = new Vector2(worldPosition.x, worldPosition.y);

        // Perform the raycast
        RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.zero);
        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;
            // Check if the clicked object is a player and not the current player
            if ((clickedObject == player1 || clickedObject == player2) && clickedObject != currentPlayer)
            {
                currentPlayer.GetComponent<PlayerPunch>().TryPunch(clickedObject);
                isPunchModeActive = false; // Exit punch mode
                Debug.Log("Punch attempted on " + clickedObject.name);
            }
        }
    }


    void EndTurn()
    {
        currentPlayer = (currentPlayer == player1) ? player2 : player1;
        // Reset modes for the new turn
        isMoveModeActive = false;
        isPunchModeActive = false;
    }
}
