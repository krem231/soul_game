using UnityEngine;
using UnityEngine.UI;
public class Stamina : MonoBehaviour
{
    public Slider slider;
    public float autoFillTime = 2f;
    public int maxStamina;
    public float currentStamina;
    private float regenDelay = 1.5f;
    private float timeSinceStaminaDrop = 0f;
    private bool staminaJustDropped = false;

    void Start()
    {
        currentStamina = maxStamina;
        slider.maxValue = maxStamina;
        slider.value = currentStamina;
    }
    private void Update()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        float previousStamina = currentStamina;

        // Hành động làm giảm stamina (sẽ thêm trong tưong lai)
        if (PlayerController.sprint_check == true)
        {
            if (currentStamina > 0)
            {
                currentStamina -= playerController.decreaseAmount * Time.deltaTime;
                if (currentStamina <= 0)
                {
                    currentStamina = 0;
                    PlayerController.sprint_check = false;
                }
            }
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                PlayerController.sprint_check = false;
                PlayerController.walk_check = true;   
            }
        }

        if (currentStamina < previousStamina)
        {
            staminaJustDropped = true;
            timeSinceStaminaDrop = 0f;
        }

        if (staminaJustDropped)
        {
            timeSinceStaminaDrop += Time.deltaTime;
            if (timeSinceStaminaDrop >= regenDelay)
            {
                staminaJustDropped = false;
            }
        }

        if (!staminaJustDropped && currentStamina < maxStamina)
        {
            currentStamina += (maxStamina / autoFillTime) * Time.deltaTime;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }

        slider.value = currentStamina; 
    }
}