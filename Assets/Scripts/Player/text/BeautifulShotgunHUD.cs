using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BeautifulShotgunHUD : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private GameObject shellPrefab;
    [SerializeField] private GameObject player;

    [Header("Visual Settings")]
    [SerializeField] private Color emptyColor = new Color(1, 1, 1, 0.5f);
    [SerializeField] private Color fullColor = new Color(1, 1, 1, 1);
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private int shakeVibrato = 15;
    [SerializeField] private float shakeRandomness = 90f;

    private List<Image> shellFills = new List<Image>();
    private List<bool> isShellFull = new List<bool>();
    private List<Transform> shellTransforms = new List<Transform>();

    // Список для хранения ссылок на активные DelayedCall для каждой гильзы
    private List<Tween> flashDelayedCalls = new List<Tween>();

    private PlayerController playerController;

    private float maxAmmo => playerController.maxAmmo;
    private float currentAmmo => playerController.currentAmmo;
    private float lastAmmo;

    private void Start()
    {
        // Полная очистка
        foreach (Transform child in transform) Destroy(child.gameObject);
        shellFills.Clear();
        isShellFull.Clear();
        shellTransforms.Clear();
        flashDelayedCalls.Clear();

        playerController = player.GetComponent<PlayerController>();
        lastAmmo = currentAmmo;

        // Инициализация по индексам
        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject newShell = Instantiate(shellPrefab, transform);
            Image fillImage = newShell.transform.GetChild(0).GetComponent<Image>();

            float currentShellFill = Mathf.Clamp01(currentAmmo - i);
            fillImage.fillAmount = currentShellFill;

            bool isFull = currentShellFill >= 1f;
            fillImage.color = isFull ? fullColor : emptyColor;

            shellFills.Add(fillImage);
            isShellFull.Add(isFull);
            shellTransforms.Add(newShell.transform);

            // Заполняем список пустыми значениями (пока твинов нет)
            flashDelayedCalls.Add(null);
        }
    }

    private void Update()
    {
        if (Mathf.Approximately(lastAmmo, currentAmmo)) return;

        UpdateHUD();
        lastAmmo = currentAmmo;
    }

    private void UpdateHUD()
    {
        for (int i = 0; i < maxAmmo; i++)
        {
            float currentShellFill = Mathf.Clamp01(currentAmmo - i);
            shellFills[i].fillAmount = currentShellFill;

            if (currentShellFill >= 1f)
            {
                if (!isShellFull[i])
                {
                    isShellFull[i] = true;
                    PlayChargeFx(shellFills[i], shellTransforms[i], i);
                }
            }
            else
            {
                if (isShellFull[i])
                {
                    isShellFull[i] = false;
                    PlayShakeRotationFx(shellFills[i], shellTransforms[i], i);
                }
            }
        }
    }

    private void PlayChargeFx(Image fillImage, Transform shellTransform, int index)
    {
        // 1. Убиваем старые анимации UI и трансформа по индексу
        fillImage.DOKill();
        shellTransform.DOKill();

        // 2. Убиваем старый DelayedCall по индексу, если он еще тикал
        KillDelayedCall(index);

        // Сброс визуала в дефолт
        shellTransform.localRotation = Quaternion.identity;
        shellTransform.localScale = Vector3.one;
        fillImage.material = flashMaterial;
        fillImage.color = Color.white;
        fillImage.fillAmount = 1f;

        // Запуск анимации шкалы
        shellTransform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 1, 1);

        // 3. Сохраняем новый DelayedCall в список по индексу гильзы
        flashDelayedCalls[index] = DOVirtual.DelayedCall(flashDuration, () => {
            if (fillImage != null)
            {
                fillImage.material = null;
                fillImage.DOColor(fullColor, 0.1f);
            }
        });
    }

    private void PlayShakeRotationFx(Image fillImage, Transform shellTransform, int index)
    {
        // Убиваем всё активное по этому индексу
        fillImage.DOKill();
        shellTransform.DOKill();
        KillDelayedCall(index);

        // Возвращаем гильзу в исходное "пустое" состояние
        fillImage.material = null;
        fillImage.color = emptyColor;
        shellTransform.localScale = Vector3.one;
        shellTransform.localRotation = Quaternion.identity;

        // Тряска
        shellTransform.DOShakeRotation(
            duration: shakeDuration,
            strength: new Vector3(0, 0, 15f),
            vibrato: shakeVibrato,
            randomness: shakeRandomness,
            fadeOut: true
        );
    }

    // Вспомогательный метод для безопасного удаления твина из списка
    private void KillDelayedCall(int index)
    {
        if (flashDelayedCalls[index] != null && flashDelayedCalls[index].IsActive())
        {
            flashDelayedCalls[index].Kill();
        }
        flashDelayedCalls[index] = null;
    }

    private void OnDestroy()
    {
        // 1. Сначала уничтожаем все DelayedCall по индексам
        if (flashDelayedCalls != null)
        {
            for (int i = 0; i < flashDelayedCalls.Count; i++)
            {
                KillDelayedCall(i);
            }
        }

        // 2. Вместо перебора списков, в которых объекты уже могут быть null,
        // просто очищаем абсолютно ВСЕ твины, связанные с этим HUD и его детьми
        DOTween.Kill(transform);

        // 3. Безопасно чистим твины на заполнениях и трансформах, если они еще живы
        if (shellTransforms != null)
        {
            foreach (var t in shellTransforms)
            {
                if (t != null) t.DOKill();
            }
        }

        if (shellFills != null)
        {
            foreach (var f in shellFills)
            {
                if (f != null) f.DOKill();
            }
        }
    }
}