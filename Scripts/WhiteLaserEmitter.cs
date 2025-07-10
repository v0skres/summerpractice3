using UnityEngine;

public class WhiteLaserEmitter : MonoBehaviour
{
    [Header("Настройки")]
    public LineRenderer whiteLaser; 
    public float maxDistance = 50f;
    public LayerMask prismLayer; 

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            transform.right,
            maxDistance,
            prismLayer);

        if (hit.collider != null && !hit.collider.CompareTag("SpectralLaser"))
        {
            whiteLaser.SetPosition(1, hit.point); 

            if (hit.collider.CompareTag("Prism"))
            {
                hit.collider.GetComponent<Prism>().ActivateSpectrum(hit.point, hit.normal);
            }
        }
        else
        {
            whiteLaser.SetPosition(1, transform.position + transform.right * maxDistance);
        }
    }
}