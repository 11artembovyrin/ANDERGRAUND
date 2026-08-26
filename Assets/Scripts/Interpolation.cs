using UnityEngine;

public class Interpolation : MonoBehaviour
{
    [Header("Настройки вращения")]
    [Tooltip("Если включено, объект копирует поворот родителя (для игрока). Если выключено, сохраняет свой стартовый поворот (для меча).")]
    public bool updateRotation = true;

    private Transform logicParent;

    // Переменные для сглаживания позиции
    private Vector3 previousPhysicsPosition;
    private Vector3 currentPhysicsPosition;

    // Переменные для сохранения стартовых настроек из инспектора
    private Vector3 localOffset;
    private Quaternion startingLocalRotation;
    private Vector3 startingLocalScale;

    void Start()
    {
        logicParent = transform.parent;

        if (logicParent == null)
        {
            Debug.LogError($"[Interpolation] На объекте {name} нет родителя! Скрипт отключен.");
            Destroy(gameObject);
            enabled = false;
            return;
        }

        // 1. ЗАПОМИНАЕМ СМЕЩЕНИЕ, ПОВОРОТ И СКЕЙЛ ИЗ ИНСПЕКТОРА
        localOffset = transform.localPosition;
        startingLocalRotation = transform.localRotation;
        startingLocalScale = transform.localScale;

        // Отвязываем визуал от родителя для плавной интерполяции, как в первой версии
        transform.parent = null;

        previousPhysicsPosition = logicParent.position;
        currentPhysicsPosition = logicParent.position;
    }

    void FixedUpdate()
    {
        if (logicParent == null)
        {
            Destroy(gameObject);
            return;
        }
        previousPhysicsPosition = currentPhysicsPosition;
        currentPhysicsPosition = logicParent.position;
    }

    void Update()
    {
        if (logicParent == null) 
        {
            Destroy(gameObject);
            return;
        }

        // Безопасный расчет фактора интерполяции
        float interpolationFactor = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        interpolationFactor = Mathf.Clamp01(interpolationFactor);

        // 1. Плавно считаем позицию логического родителя
        Vector3 interpolatedTarget = Vector3.Lerp(previousPhysicsPosition, currentPhysicsPosition, interpolationFactor);

        // 2. УПРАВЛЕНИЕ ВРАЩЕНИЕМ
        Quaternion finalRotation = updateRotation ? logicParent.rotation : startingLocalRotation;
        transform.rotation = finalRotation;

        // 3. ПРИМЕНЯЕМ СМЕЩЕНИЕ
        Vector3 rotatedOffset = finalRotation * localOffset;
        transform.position = interpolatedTarget + rotatedOffset;

        // 4. КОРРЕТНЫЙ ВОЗВРАТ СКЕЙЛА (перемножаем стартовый локальный на масштаб родителя)
        transform.localScale = Vector3.Scale(startingLocalScale, logicParent.localScale);
    }
}
