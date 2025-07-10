using UnityEngine;

public class Prism : MonoBehaviour
{
    public LineRenderer[] spectralLasers;
    public float dispersionAngle = 15f;
    public float rotationSpeed = 100f;
    public float moveSpeed = 5f;

    private bool isDragging;
    private Vector3 offset;

    void Update()
    {
        HandleRotation();
        HandleMovement();
        UpdateLaserColliders();
    }

    void HandleRotation()
    {
        // Вращение клавишами Q/E
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }
    }

    void HandleMovement()
    {
        // Перемещение правой кнопкой мыши
        if (Input.GetMouseButtonDown(1)) // Правая кнопка
        {
            offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(
                mousePos.x + offset.x,
                mousePos.y + offset.y,
                transform.position.z
            );
        }
    }

    public void ActivateSpectrum(Vector2 hitPoint, Vector2 hitNormal)
    {
        for (int i = 0; i < spectralLasers.Length; i++)
        {
            spectralLasers[i].gameObject.SetActive(true);
            spectralLasers[i].SetPosition(0, hitPoint);

            float angle = dispersionAngle * (i - spectralLasers.Length / 2);
            Vector2 dir = Quaternion.Euler(0, 0, angle) * hitNormal;
            spectralLasers[i].SetPosition(1, hitPoint + dir * 10f);
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
            if (!laser.gameObject.activeSelf) continue;

            var collider = laser.GetComponent<BoxCollider2D>();
            if (collider == null) continue;

            Vector3 start = laser.GetPosition(0);
            Vector3 end = laser.GetPosition(1);

            collider.size = new Vector2(Vector3.Distance(start, end), 0.1f);
            collider.transform.position = (start + end) / 2f;
            collider.transform.right = (end - start).normalized;
        }
    }
}