using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerHealth playerHealth;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Always face the camera
        transform.LookAt(transform.position + cameraTransform.forward);

        // Update health text
        if (playerHealth != null)
        {
            int current = Mathf.RoundToInt(playerHealth.CurrentHealth);
            int max = Mathf.RoundToInt(playerHealth.HealthPercentage * 100f);
            healthText.text = $"{current} / 100";
        }
    }
}