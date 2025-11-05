using UnityEngine;
using System.Collections;

namespace LevelObjects.Interactable
{
    public class LiftPlatform : Interactable
    {
        public Transform topPoint;
        public Transform bottomPoint;
        public float moveSpeed = 2f;
        public GameObject liftGround;

        private bool isAtTop = false;
        public bool isMoving = false;
        private GameObject currentPlayer;
        private Rigidbody2D playerRb;
        private Vector3 playerStartLocalPosition;

        private void Start()
        {
            if (liftGround == null)
            {
                liftGround = transform.Find("LiftGround")?.gameObject;
                if (liftGround == null)
                {
                    Debug.LogError("LiftGround not found! Please assign it in the inspector.");
                }
            }
        }

        public override void Interact()
        {
            if (!isMoving)
                StartCoroutine(MoveLift());
        }

        private IEnumerator MoveLift()
        {
            isMoving = true;
            Vector3 startPos = transform.position;
            Vector3 targetPos = isAtTop ? bottomPoint.position : topPoint.position;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * moveSpeed;
                Vector3 newPosition = Vector3.Lerp(startPos, targetPos, t);

                // Calculate movement delta
                Vector3 movement = newPosition - transform.position;

                // Move platform
                transform.position = newPosition;

                // Update player position directly if attached
                if (currentPlayer != null && playerRb != null)
                {
                    currentPlayer.transform.position += movement;
                }

                yield return null;
            }

            transform.position = targetPos;
            isAtTop = !isAtTop;
            isMoving = false;
        }

        public void OnPlayerEntered(GameObject player)
        {
            if (player.CompareTag("Player"))
            {
                currentPlayer = player;
                playerRb = player.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // Keep track of the player's relative position
                    playerStartLocalPosition = transform.InverseTransformPoint(player.transform.position);

                    // Make the player a child of the platform and disable physics
                    player.transform.SetParent(transform);
                    playerRb.simulated = false;

                    // Ensure player is at the correct position immediately
                    Vector3 worldPos = transform.TransformPoint(playerStartLocalPosition);
                    player.transform.position = worldPos;
                }
            }
        }
        
        public void OnPlayerExited(GameObject player)
        {
            if (player == currentPlayer)
            {
                if (playerRb != null)
                {
                    // Restore the player's parent to null
                    player.transform.SetParent(null);

                    // Re-enable rigidbody physics
                    playerRb.simulated = true;
                }

                currentPlayer = null;
                playerRb = null;
            }
        }

        private void OnDrawGizmos()
        {
            if (topPoint != null && bottomPoint != null)
            {
                // Draw path
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(topPoint.position, bottomPoint.position);

                float pointSize = 0.3f;

                // Top point
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(topPoint.position, pointSize);

                // Bottom point
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(bottomPoint.position, pointSize);

                // Current position
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, Vector3.one * pointSize);

                // Platform bounds
                if (liftGround != null)
                {
                    Gizmos.color = isMoving ? new Color(1f, 0.5f, 0f, 0.3f) : new Color(0f, 1f, 1f, 0.3f);
                    BoxCollider2D collider = liftGround.GetComponent<BoxCollider2D>();
                    if (collider != null)
                    {
                        Vector3 pos = liftGround.transform.position + (Vector3)collider.offset;
                        Vector3 size = new Vector3(
                            collider.size.x * liftGround.transform.lossyScale.x,
                            collider.size.y * liftGround.transform.lossyScale.y,
                            0.1f
                        );
                        Gizmos.DrawCube(pos, size);
                    }
                }
            }
        }
    }
}