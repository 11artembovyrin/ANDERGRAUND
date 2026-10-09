using UnityEngine;
using System;


public class RoomTrigger : MonoBehaviour
{
    public event Action PlayerEntered;

    [SerializeField] private string playerTag = "Player";

    private bool _fired = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_fired) return;
        if (!other.CompareTag(playerTag)) return;

        _fired = true;
        PlayerEntered?.Invoke();
    }
}

