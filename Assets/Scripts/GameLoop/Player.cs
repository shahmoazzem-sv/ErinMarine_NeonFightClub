using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // This field will be assigned the live HeroCard component by the State.
    public HeroCard playerHeroCard;

    // The state now directly manages the assignment of the instantiated HeroCard component.


    public void OnTapPerformed(InputAction.CallbackContext context)
    {
        // Vector2 screenPosition = context.ReadValue<Vector2>();
        // Debug.Log("Tap performed at screen position: " + screenPosition);
        if (context.performed)
        {
            Vector2 screenPostion = context.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(screenPostion);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Try to get the HeroCard component from the hit object
                HeroCard detectedCard = hit.collider.gameObject.GetComponent<HeroCard>();

                if (detectedCard != null)
                {
                    Debug.Log("Hero Card Detected: " + detectedCard.CardSO.name);

                    // 🆕 Trigger the selection logic on the detected card
                    detectedCard.SelectCard();
                }
            }
        }
    }
}