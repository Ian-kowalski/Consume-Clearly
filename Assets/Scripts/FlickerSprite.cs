using UnityEngine;

public class FlickerSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public float minAlpha = 0.3f;
    public float maxAlpha = 1f;
    public float flickerSpeed = 0.25f;
    
    private float timer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= flickerSpeed)
        {
            Color color = spriteRenderer.color;
            color.a = Random.Range(minAlpha, maxAlpha);
            spriteRenderer.color = color;

            timer = 0f;
        }
    }
}
