using UnityEngine;
using UnityEngine.UI;

public class PolarizationController : MonoBehaviour
{
    [Header("Scene References")]
    public Slider polarizer1Slider;
    public Slider polarizer2Slider;
    public Renderer screenRenderer;

    [Header("Polarizer GameObjects")]
    public GameObject polarizer1GO;
    public GameObject polarizer2GO;

    // --- ����� ���������� ��� ���������� ������������� ---
    private Material polarizer1Material;
    private Material polarizer2Material;

    [Header("Polarizer Rotation Axes")]
    public Vector3 polarizer1RotationAxis = Vector3.up;
    public Vector3 polarizer2RotationAxis = Vector3.up;

    [Header("Light Properties")]
    [Range(0f, 1f)]
    public float initialLightIntensity = 1.0f;
    public Color lightColor = Color.white; // ���� �����, ���� ����� ��� ������

    private float polarizer1Angle = 0f;
    private float polarizer2Angle = 75f;

    // ���������� ������������� ��� ������� ������ (�� �������� ��� ���������)
    private static readonly int GlobalPolarizedIntensityID = Shader.PropertyToID("_GlobalPolarizedIntensity");

    // ������������� �������� Opacity � ����� ����� ������� PolarizerGlassTransparencyShader
    private static readonly int PolarizerOpacityID = Shader.PropertyToID("_PolarizerOpacity");

    void Start()
    {
        // �������� ��������� � ���������� �������������
        if (polarizer1GO != null)
        {
            // �������� ��������. .material ������� ����� ���������, ���� ��� ����� ��������.
            // ���� �� ������ �������������� ������ ���� ���������, ����������� .material.
            polarizer1Material = polarizer1GO.GetComponent<Renderer>().material;
        }
        if (polarizer2GO != null)
        {
            polarizer2Material = polarizer2GO.GetComponent<Renderer>().material;
        }

        // ��������� ���������
        if (polarizer1Slider != null)
        {
            polarizer1Slider.minValue = 0;
            polarizer1Slider.maxValue = 180;
            polarizer1Slider.onValueChanged.AddListener(OnPolarizer1AngleChanged);
            polarizer1Slider.value = polarizer1Angle;
        }

        if (polarizer2Slider != null)
        {
            polarizer2Slider.minValue = 0;
            polarizer2Slider.maxValue = 180;
            polarizer2Slider.onValueChanged.AddListener(OnPolarizer2AngleChanged);
            polarizer2Slider.value = polarizer2Angle;
        }

        // ������������� ������� ��� ������
        UpdatePolarizationEffect();
    }

    // ���������� ���� ������� ������������
    public void OnPolarizer1AngleChanged(float value)
    {
        polarizer1Angle = value;
        if (polarizer1GO != null)
        {
            polarizer1GO.transform.localRotation = Quaternion.Euler(polarizer1RotationAxis * polarizer1Angle);
        }
        UpdatePolarizationEffect();
    }

    // ���������� ���� ������� ������������
    public void OnPolarizer2AngleChanged(float value)
    {
        polarizer2Angle = value;
        if (polarizer2GO != null)
        {
            polarizer2GO.transform.localRotation = Quaternion.Euler(polarizer2RotationAxis * polarizer2Angle);
        }
        UpdatePolarizationEffect();
    }

    // ���������� � ���������� ������� �����������
    void UpdatePolarizationEffect()
    {
        // --- ������ ������������� �����, ��������� �� ����� (��������� ��� ����) ---
        float currentIntensityForScreen = initialLightIntensity;
        currentIntensityForScreen *= 0.5f; // ����� ������� ������������

        float angleDifference = Mathf.Abs(polarizer1Angle - polarizer2Angle);
        if (angleDifference > 180) angleDifference = 360 - angleDifference;

        float cosSquared = Mathf.Cos(angleDifference * Mathf.Deg2Rad);
        cosSquared *= cosSquared;

        currentIntensityForScreen *= cosSquared; // ������������� ��� ������ �� ������ ������

        // ��������� ���������� �������� ������� ��� ������
        // ��� ��-�������� ����� ������ ������� ������.
        Shader.SetGlobalFloat(GlobalPolarizedIntensityID, currentIntensityForScreen);
        // Debug.Log($"P1 Angle: {polarizer1Angle}, P2 Angle: {polarizer2Angle}, Screen Intensity: {currentIntensityForScreen}");


        // --- ����� ���: ���������� ������������� ������������� ---
        // ����� ���� ��������� �������� (cosSquared = 1), ������������ ������ ���� �����������.
        // ����� ���� ��������� ����������� (cosSquared = 0), ������������ ������ ���� ������������� (�������).

        // �������� �������������� ��� �������������:
        // ���� cosSquared = 1 (���������), opacity = 0
        // ���� cosSquared = 0 (�����������), opacity = 1
        float polarizerDisplayOpacity = 1.0f - cosSquared;

        // ��������� ��� �������� � ���������� �������������
        if (polarizer1Material != null)
        {
            // ����� �� ����� ������, ��� ������ ����������� ������ ���������� ������.
            // ������, ����� ���� ������������, ��� ������������ �������� �������.
            polarizer1Material.SetFloat(PolarizerOpacityID, polarizerDisplayOpacity);
        }
        if (polarizer2Material != null)
        {
            polarizer2Material.SetFloat(PolarizerOpacityID, polarizerDisplayOpacity);
        }

        Debug.Log($"P1 Angle: {polarizer1Angle}, P2 Angle: {polarizer2Angle}, Screen Intensity: {currentIntensityForScreen}, Polarizer Visual Opacity: {polarizerDisplayOpacity}");
    }
}