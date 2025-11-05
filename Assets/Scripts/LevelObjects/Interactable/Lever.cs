using System.Collections;
using UnityEngine;

namespace LevelObjects.Interactable
{
    public class Lever : Interactable
    {
        [Header("Linked Objects")]
        public Interactable[] linkedObjects; // Objects this lever controls (trapdoors, lifts, etc.)

        [Header("Lever Handle Settings")] public Transform handle; // The moving part of the lever
        public float flipAngle = 45f; // How far the handle rotates
        public float flipSpeed = 5f; // How fast it moves

        private bool isOn = false;
        private bool isFlipping = false;
        private Quaternion startRotation;
        private Quaternion targetRotation;

        void Start()
        {
            if (handle == null)
            {
                Debug.LogWarning("Lever handle not assigned! Please assign it in the Inspector.");
                handle = transform; // fallback to this object if not assigned
            }

            startRotation = handle.localRotation;
            targetRotation = Quaternion.Euler(handle.localEulerAngles + new Vector3(0, 0, flipAngle));

            // Validate linked objects
            foreach (var obj in linkedObjects)
            {
                if (obj != null && !obj.RequiresLever)
                {
                    Debug.LogWarning(
                        $"Object {obj.name} is linked to a lever but doesn't require one. Set RequiresLever to true in the inspector.");
                }
            }
        }

        public override void Interact()
        {
            if (isFlipping) return;

            isOn = !isOn;
            StartCoroutine(FlipHandle());

            foreach (Interactable obj in linkedObjects)
            {
                if (obj != null)
                    obj.Interact();
            }
        }

        private IEnumerator FlipHandle()
        {
            isFlipping = true;

            Quaternion target = isOn ? targetRotation : startRotation;

            while (Quaternion.Angle(handle.localRotation, target) > 0.1f)
            {
                handle.localRotation = Quaternion.Lerp(handle.localRotation, target, Time.deltaTime * flipSpeed);
                yield return null;
            }

            handle.localRotation = target;
            isFlipping = false;
        }
    }
}