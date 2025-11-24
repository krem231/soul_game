using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public float speed;
    public float sprint_speed;
    public Rigidbody2D rb;
    private Vector2 moveDirection;
    public static bool sprint_check;
    public static bool walk_check;
    public float decreaseAmount;
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    void Update()
    {
        InputMovement();
    }
    void FixedUpdate()
    {
        Move();
        Sprint();
    }
    void InputMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY);
    }
    void Move()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }
    void Sprint()
    {
        Stamina stamina = FindObjectOfType<Stamina>();
        bool isMoving = Mathf.Abs(moveDirection.x) > 0f || Mathf.Abs(moveDirection.y) > 0f;
        if (Input.GetKey(KeyCode.LeftShift) && isMoving && stamina != null && stamina.currentStamina > 0)
        {
            sprint_check = true;
            walk_check = false;
            speed = sprint_speed;
        }
        else
        {
            speed = 5f;
            sprint_check = false;
            walk_check = true;
        }
    }
    void Doughing()
    {

    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Player Died!");
        // Thêm logic chết ở đây (ví dụ: reload scene, hiển thị menu game over, v.v.)
    }
}