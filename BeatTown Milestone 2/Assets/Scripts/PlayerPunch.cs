using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPunch : MonoBehaviour
{
    public Tilemap tilemap;
    public Animator animator;  // Ensure this is linked in the Inspector
    public float knockbackSpeed = 2.0f; // Speed of the knockback

    public void TryPunch(GameObject target)
    {
        Vector3Int attackerPosition = tilemap.WorldToCell(transform.position);
        Vector3Int targetPosition = tilemap.WorldToCell(target.transform.position);

        if (Mathf.Abs(attackerPosition.x - targetPosition.x) <= 1 && Mathf.Abs(attackerPosition.y - targetPosition.y) <= 1)
        {
            AiPunch targetAiPunch = target.GetComponent<AiPunch>();
            if (targetAiPunch != null)
            {
                //targetAiPunch.health -= 3;
                Debug.Log($"Punched AI {target.name}. Remaining Health: {targetAiPunch.health}");

                ObjectivesManager.Instance.IncrementPunchCount();

                animator.Play("PunchAnimation");

                // Wait for the animation to finish before executing knockback
                StartCoroutine(KnockbackAfterAnimation(target, attackerPosition, targetPosition));
            }
            else
            {
                Debug.Log("No AiPunch component found on the target.");
            }
        }
        else
        {
            Debug.Log("Target is not adjacent. Punch failed.");
        }
    }

    private IEnumerator KnockbackAfterAnimation(GameObject target, Vector3Int attackerPosition, Vector3Int targetPosition)
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Vector3Int moveDirection = targetPosition - attackerPosition;
        Vector3Int newTargetPosition = targetPosition + moveDirection;
        Vector3 newWorldPosition = tilemap.GetCellCenterWorld(newTargetPosition);

        if (!GridManager.Instance.IsCellOccupied(newTargetPosition))
        {
            StartCoroutine(SmoothMove(target, newWorldPosition, newTargetPosition));
        }
        else
        {
            Debug.Log("Knockback failed, cell is occupied.");
        }
    }

    private IEnumerator SmoothMove(GameObject target, Vector3 newWorldPosition, Vector3Int newCellPosition)
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
        target.transform.position = newWorldPosition;

        GridManager.Instance.SetCellOccupied(tilemap.WorldToCell(startPosition), false);
        GridManager.Instance.SetCellOccupied(newCellPosition, true);
    }
}
