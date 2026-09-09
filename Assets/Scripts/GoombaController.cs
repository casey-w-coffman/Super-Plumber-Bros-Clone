using UnityEngine;

public class GoombaController : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float flipDuration = 0.25f;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    int lastFlipTime;
    int direction = -1; // -1 = left, 1 = right

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance == null) return;

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        //update the 50 if fixedupdate ticks changed
        int flipIntervalTicks = (int)(flipDuration * 50f);
        if (GameManager.Instance.TimeTicks - lastFlipTime >= flipIntervalTicks)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            lastFlipTime = GameManager.Instance.TimeTicks;
        }
    }

    //flip direction upon collision with Wall tag
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
            direction *= -1;
    }
}