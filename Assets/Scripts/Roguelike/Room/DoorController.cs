using DG.Tweening;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private DoorTrigger trigger;
    [SerializeField] private float doorOpenSpeed = 20f;
    [SerializeField] private float doorOpenDistance = 20f;
    [SerializeField] private Vector2 openDirection = new Vector2(1, 0);

    private Vector3 startPosition;
    private Vector3 openPosition;


    private RoomController roomController;

    private void Start()
    {
        startPosition = transform.position;
        openPosition = new Vector3(startPosition.x + doorOpenDistance * openDirection.x, startPosition.y + doorOpenDistance * openDirection.y, startPosition.z);
    }

    public void Setup(RoomController room)
    {
        roomController = room;
    }
    

    public void TryOpen()
    {
        if (roomController != null && roomController.State != RoomState.Active)
        {
            Open();
        }
    }

    public void ForceOpen()
    {
        Open();
    }

    public void Open()
    {
        transform.DOKill();
        float distance = Vector3.Distance(transform.position, openPosition);
        transform.DOMove(openPosition, distance / doorOpenSpeed).SetEase(Ease.OutCubic);
    }
    

    public void Close()
    {
        transform.DOKill();
        float distance = Vector3.Distance(transform.position, startPosition);
        transform.DOMove(startPosition, distance / doorOpenSpeed).SetEase(Ease.OutCubic);
    }

}
