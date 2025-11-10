using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public Transform handArea;
    public GameObject cardPrefab;

    public bool isLocal => Object.HasInputAuthority;

    private List<GameObject> myCards = new();

    public override void Spawned()
    {
        // base.Spawned();

        if (isLocal)
        {
            //give the player 5 randomo card for testing
            for (int i = 0; i < 5; i++)
            {
                var card = Runner.Spawn(cardPrefab, handArea.position + Vector3.right * i * 0.5f, Quaternion.identity, Object.InputAuthority);

                myCards.Add(card.gameObject);

                var view = card.GetComponent<CardView>();
                view.faceUp = true;
            }

            // Move camera to player’s view
            Camera.main.transform.position = transform.position + new Vector3(0, 5, -5);
            Camera.main.transform.LookAt(transform);
        }
        else
        {
            // Non-local player cards should be face down
            foreach (var card in myCards)
                card.GetComponent<CardView>().faceUp = false;
        }
    }
}
