using UnityEngine;

public class Prism : MonoBehaviour
{
    public LineRenderer[] spectralLasers; 
    public float dispersionAngle = 15f; 

    public void ActivateSpectrum(Vector2 hitPoint, Vector2 hitNormal)
    {
        for (int i = 0; i < spectralLasers.Length; i++)
        {
            spectralLasers[i].gameObject.SetActive(true);
            spectralLasers[i].SetPosition(0, hitPoint);

            float angle = dispersionAngle * (i - spectralLasers.Length / 2);
            Vector2 dir = Quaternion.Euler(0, 0, angle) * hitNormal;
            Vector2 endPos = hitPoint + dir * 10f;
            spectralLasers[i].SetPosition(1, endPos);

            BoxCollider2D collider = spectralLasers[i].GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.size = new Vector2(Vector2.Distance(hitPoint, endPos), 0.1f);
                collider.transform.position = (hitPoint + endPos) / 2f;
                collider.transform.right = dir;
            }
        }
    }

    public void DeactivateSpectrum()
    {
        foreach (var laser in spectralLasers)
        {
            laser.gameObject.SetActive(false);
        }
    }

    void UpdateLaserColliders()
    {
        foreach (var laser in spectralLasers)
        {
            var collider = laser.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                Vector3 start = laser.GetPosition(0);
                Vector3 end = laser.GetPosition(1);

                collider.size = new Vector2(Vector3.Distance(start, end), 0.1f);
                collider.transform.position = (start + end) / 2f;
                collider.transform.right = (end - start).normalized;

                collider.usedByEffector = true;
            }
        }
    }
}