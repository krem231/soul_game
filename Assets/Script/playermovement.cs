using UnityEngine;
using UnityEngine.InputSystem;
public class playermovement : MonoBehaviour
{
    public float speed = 20;
    public Rigidbody2D rb;
    public Vector2 movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = 0, y = 0;

        if (Keyboard.current.aKey.isPressed) x = -1;
        if (Keyboard.current.dKey.isPressed) x = 1;
        if (Keyboard.current.wKey.isPressed) y = 1;
        if (Keyboard.current.sKey.isPressed) y = -1;

        movement = new Vector2(x, y).normalized;

    }
    void FixedUpdate()
    {
        // Di chuyển nhân vật
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
