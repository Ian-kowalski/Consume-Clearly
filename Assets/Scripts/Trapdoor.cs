using UnityEngine;
using System.Collections;

public class Trapdoor : MonoBehaviour
{
    public bool isOpen = false;
    public float openAngle = 90f;
    public float rotationSpeed = 90f; // degrees per second

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine rotationCoroutine;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, openAngle));
    }

    public void ToggleTrapdoor()
    {
        isOpen = !isOpen;

        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        rotationCoroutine = StartCoroutine(RotateTrapdoor(isOpen ? openRotation : closedRotation));
    }

    IEnumerator RotateTrapdoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRotation; // Snap to final position
    }
}