using UnityEngine;

namespace LevelObjects.Interactable
{
    public class CogwheelRotation : MonoBehaviour
    {
        [Header("Rotation Settings")]
        public float rotationSpeed = 100f;   // Degrees per second
        public bool isRotating = true;       // Toggle rotation on/off
        public bool reverseDirection = false; // Reverse rotation direction

        void Update()
        {
            if (isRotating)
            {
                // Determine direction
                float direction = reverseDirection ? -1f : 1f;

                // Apply rotation (around Z axis for 2D cogwheel)
                transform.Rotate(0f, 0f, direction * rotationSpeed * Time.deltaTime);
            }
        }
    }
}
