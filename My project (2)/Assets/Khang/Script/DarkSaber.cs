using UnityEngine;

public class DarkSaber : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 3f; //tư hủy sau 3s
    public int damage = 10;

    private Rigidbody2D rb;
    private float direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    public void Initialize(float dir)
    {
        direction = dir;
        // áp dụng vận tốc theo hướng
        rb.linearVelocity = new Vector2(speed * -direction, 0f);

        //lật phi tiêu khi cần
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }    
    }
}



