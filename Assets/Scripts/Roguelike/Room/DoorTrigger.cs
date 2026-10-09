using UnityEngine;

public class DoorTrigger : MonoBehaviour
{

    [SerializeField] private DoorController doorController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            doorController.TryOpen();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            doorController.Close();
        }
    }
}
