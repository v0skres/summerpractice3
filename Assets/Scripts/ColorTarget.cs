using UnityEngine;

public class ColorTarget : MonoBehaviour
{
    public Color requiredColor;
    public ParticleSystem successEffect;
    public GameObject SuccessScreen;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Объект {other.name} вошёл в триггер {gameObject.name}");

        if (other.CompareTag("SpectralLaser"))
        {
            Debug.Log("Обнаружен SpectralLaser!");

            LineRenderer laser = other.GetComponent<LineRenderer>();
            if (laser == null)
            {
                Debug.LogError("Нет компонента LineRenderer!");
                return;
            }

            if (laser.material == null)
            {
                Debug.LogError("Нет материала у LineRenderer!");
                return;
            }

            Debug.Log($"Цвет луча: {ColorUtility.ToHtmlStringRGB(laser.material.color)}, " +
                     $"Требуется: {ColorUtility.ToHtmlStringRGB(requiredColor)}");

            if (ColorsMatch(laser.material.color, requiredColor))
            {
                Debug.Log("Цвет совпал! Активация...");
                SuccessScreen.SetActive(true);
            }
        }
    }

    bool ColorsMatch(Color a, Color b, float tolerance = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}