using UnityEngine;

public class Interpolation : MonoBehaviour
{
    private Transform logicParent;

    // Переменные для сглаживания позиции
    private Vector3 previousPhysicsPosition;
    private Vector3 currentPhysicsPosition;

    // Переменные для сохранения стартовых настроек из инспектора
    private Vector3 localOffset;
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

        // Запоминаем смещение и скейл из инспектора (поворот больше не трогаем)
        localOffset = transform.localPosition;
        startingLocalScale = transform.localScale;

        // Отвязываем визуал от родителя для плавной интерполяции
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

        // 2. ПРИМЕНЯЕМ СМЕЩЕНИЕ (без учета поворота — просто прибавляем локальный offset)
        transform.position = interpolatedTarget + localOffset;

        // 3. КОРРЕКТНЫЙ ВОЗВРАТ СКЕЙЛА
        transform.localScale = Vector3.Scale(startingLocalScale, logicParent.localScale);

        // ПОВОРОТ (transform.rotation) БОЛЬШЕ НЕ ТРОГАЕМ — им управляет EyeBaseController
    }
}