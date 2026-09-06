using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BeautifulShotgunHUD : MonoBehaviour
{
    [Header("GayObjects")]
    [SerializeField] private GameObject shellPrefab;
    [SerializeField] private GameObject player;

    [Header("Sasnye Nastroyki")]
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


    private PlayerController playerController;
    private float maxAmmo => playerController.maxAmmo;
    private float currentAmmo => playerController.currentAmmo;
    private void Start()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
        shellFills.Clear();
        isShellFull.Clear();


        playerController = player.GetComponent<PlayerController>();

        for (int i = 0; i < playerController.maxAmmo; i++)
        {
            GameObject newShell = Instantiate(shellPrefab, transform);

            Image fillImage = newShell.transform.GetChild(0).GetComponent<Image>();
            fillImage.fillAmount = 0f;
            fillImage.color = emptyColor;

            shellFills.Add(fillImage);
            isShellFull.Add(false);
            shellTransforms.Add(newShell.transform);
        }

    }


    private float currentAmmoHUD = 0;
    private void FixedUpdate()
    {
        if (currentAmmoHUD == currentAmmo) return;

        for (int i = 0; i < maxAmmo; i++)
        {
            float currentShellFill = Mathf.Clamp01(currentAmmo - i);
            shellFills[i].fillAmount = currentShellFill;
            if (currentShellFill == 1)
            {
                if (!isShellFull[i])
                {
                    isShellFull[i] = true;
                    shellFills[i].color = fullColor;
                    PlayChargeFx(shellFills[i], shellTransforms[i]);
                }
            }
            else if (isShellFull[i])
            {
                isShellFull[i] = false;
                shellFills[i].color = emptyColor;
                PlayShakeRotationFx(shellTransforms[i]);
            }
        }

        currentAmmoHUD = currentAmmo;
    }


    private void PlayChargeFx(Image fillImage, Transform shellTransform)
    {
        fillImage.DOKill();
        shellTransform.DOKill();
        shellTransform.rotation = Quaternion.identity;

        fillImage.material = flashMaterial;
        fillImage.color = Color.white;

        shellTransform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 1, 1);
        DOVirtual.DelayedCall(flashDuration, () =>
        {
            if (fillImage != null)
            {
                fillImage.material = null;
                fillImage.DOColor(fullColor, 0.1f);
            }
        });
    }

    private void PlayShakeRotationFx(Transform shellTransform)
    {
        shellTransform.DOKill();

        // Сбрасываем поворот в дефолт
        shellTransform.localRotation = Quaternion.identity;

        // Трясем вращение. Силу (strength) для UI лучше ставить в районе 10-20 градусов
        
        shellTransform.DOShakeRotation(
            duration: shakeDuration,
            // Трясем только по оси Z (Vector3.forward), чтобы пуля качалась влево-вправо
            strength: new Vector3(0, 0, 15f),
            vibrato: shakeVibrato,
            randomness: shakeRandomness,
            fadeOut: true
        );
    }
}
