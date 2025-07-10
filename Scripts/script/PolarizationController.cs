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

    // --- НОВЫЕ ПЕРЕМЕННЫЕ ДЛЯ МАТЕРИАЛОВ ПОЛЯРИЗАТОРОВ ---
    private Material polarizer1Material;
    private Material polarizer2Material;

    [Header("Polarizer Rotation Axes")]
    public Vector3 polarizer1RotationAxis = Vector3.up;
    public Vector3 polarizer2RotationAxis = Vector3.up;

    [Header("Light Properties")]
    [Range(0f, 1f)]
    public float initialLightIntensity = 1.0f;
    public Color lightColor = Color.white; // Цвет света, если нужен для экрана

    private float polarizer1Angle = 0f;
    private float polarizer2Angle = 75f;

    // Глобальный идентификатор для шейдера экрана (он остается без изменений)
    private static readonly int GlobalPolarizedIntensityID = Shader.PropertyToID("_GlobalPolarizedIntensity");

    // Идентификатор свойства Opacity в вашем новом шейдере PolarizerGlassTransparencyShader
    private static readonly int PolarizerOpacityID = Shader.PropertyToID("_PolarizerOpacity");

    void Start()
    {
        // Получаем материалы с рендереров поляризаторов
        if (polarizer1GO != null)
        {
            // Получаем материал. .material создает новый экземпляр, если это общий материал.
            // Если вы хотите модифицировать только этот экземпляр, используйте .material.
            polarizer1Material = polarizer1GO.GetComponent<Renderer>().material;
        }
        if (polarizer2GO != null)
        {
            polarizer2Material = polarizer2GO.GetComponent<Renderer>().material;
        }

        // Настройка слайдеров
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

        // Инициализация эффекта при старте
        UpdatePolarizationEffect();
    }

    // Обновление угла первого поляризатора
    public void OnPolarizer1AngleChanged(float value)
    {
        polarizer1Angle = value;
        if (polarizer1GO != null)
        {
            polarizer1GO.transform.localRotation = Quaternion.Euler(polarizer1RotationAxis * polarizer1Angle);
        }
        UpdatePolarizationEffect();
    }

    // Обновление угла второго поляризатора
    public void OnPolarizer2AngleChanged(float value)
    {
        polarizer2Angle = value;
        if (polarizer2GO != null)
        {
            polarizer2GO.transform.localRotation = Quaternion.Euler(polarizer2RotationAxis * polarizer2Angle);
        }
        UpdatePolarizationEffect();
    }

    // Вычисление и применение эффекта поляризации
    void UpdatePolarizationEffect()
    {
        // --- Расчет интенсивности света, падающего на ЭКРАН (оставляем как есть) ---
        float currentIntensityForScreen = initialLightIntensity;
        currentIntensityForScreen *= 0.5f; // После первого поляризатора

        float angleDifference = Mathf.Abs(polarizer1Angle - polarizer2Angle);
        if (angleDifference > 180) angleDifference = 360 - angleDifference;

        float cosSquared = Mathf.Cos(angleDifference * Mathf.Deg2Rad);
        cosSquared *= cosSquared;

        currentIntensityForScreen *= cosSquared; // Интенсивность для экрана по закону Малюса

        // Обновляем глобальные свойства шейдера для ЭКРАНА
        // Это по-прежнему будет менять яркость экрана.
        Shader.SetGlobalFloat(GlobalPolarizedIntensityID, currentIntensityForScreen);
        // Debug.Log($"P1 Angle: {polarizer1Angle}, P2 Angle: {polarizer2Angle}, Screen Intensity: {currentIntensityForScreen}");


        // --- НОВЫЙ КОД: Управление ПРОЗРАЧНОСТЬЮ поляризаторов ---
        // Когда свет полностью проходит (cosSquared = 1), поляризаторы должны быть ПРОЗРАЧНЫМИ.
        // Когда свет полностью блокируется (cosSquared = 0), поляризаторы должны быть НЕПРОЗРАЧНЫМИ (черными).

        // Значение непрозрачности для поляризаторов:
        // Если cosSquared = 1 (прозрачно), opacity = 0
        // Если cosSquared = 0 (непрозрачно), opacity = 1
        float polarizerDisplayOpacity = 1.0f - cosSquared;

        // Применяем это значение к материалам поляризаторов
        if (polarizer1Material != null)
        {
            // Здесь мы можем решить, как каждый поляризатор должен отображать эффект.
            // Обычно, когда свет заблокирован, оба поляризатора выглядят темными.
            polarizer1Material.SetFloat(PolarizerOpacityID, polarizerDisplayOpacity);
        }
        if (polarizer2Material != null)
        {
            polarizer2Material.SetFloat(PolarizerOpacityID, polarizerDisplayOpacity);
        }

        Debug.Log($"P1 Angle: {polarizer1Angle}, P2 Angle: {polarizer2Angle}, Screen Intensity: {currentIntensityForScreen}, Polarizer Visual Opacity: {polarizerDisplayOpacity}");
    }
}