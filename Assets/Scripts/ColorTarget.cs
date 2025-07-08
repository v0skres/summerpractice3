using UnityEngine;

public class ColorTarget : MonoBehaviour
{
    public Color requiredColor;
    public ParticleSystem successEffect;
    public GameObject SuccessScreen;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"������ {other.name} ����� � ������� {gameObject.name}");

        if (other.CompareTag("SpectralLaser"))
        {
            Debug.Log("��������� SpectralLaser!");

            LineRenderer laser = other.GetComponent<LineRenderer>();
            if (laser == null)
            {
                Debug.LogError("��� ���������� LineRenderer!");
                return;
            }

            if (laser.material == null)
            {
                Debug.LogError("��� ��������� � LineRenderer!");
                return;
            }

            Debug.Log($"���� ����: {ColorUtility.ToHtmlStringRGB(laser.material.color)}, " +
                     $"���������: {ColorUtility.ToHtmlStringRGB(requiredColor)}");

            if (ColorsMatch(laser.material.color, requiredColor))
            {
                Debug.Log("���� ������! ���������...");
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