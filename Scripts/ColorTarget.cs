using UnityEngine;

public class ColorTarget : MonoBehaviour
{
    public Color requiredColor;
    public ParticleSystem successEffect;
    public GameObject SuccessScreen;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpectralLaser"))
        {
            LineRenderer laser = other.GetComponent<LineRenderer>();
            if (laser == null)
            {
                return;
            }

            if (laser.material == null)
            {
                return;
            }


            if (ColorsMatch(laser.material.color, requiredColor))
            {
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