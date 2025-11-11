using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector3 dragOffset;
    private Camera gameCamera;

    void Start()
    {
        gameCamera = Camera.main;
        if (gameCamera == null)
        {
            Debug.LogError("Camera not found");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || gameCamera == null)
            return;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardSelected);

        Vector3 mouseWorldPosition = gameCamera.ScreenToWorldPoint(eventData.position);

        dragOffset = transform.position - gameCamera.ScreenToWorldPoint(eventData.position);
        dragOffset.z = 0;
            
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || gameCamera == null)
            return;

        Vector3 mouseWorldPosition = gameCamera.ScreenToWorldPoint(eventData.position);

        Vector3 newPosition = mouseWorldPosition + dragOffset;
        newPosition.z = transform.position.z;

        transform.position = newPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || gameCamera == null)
            return;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardUnselected);
    }
}
