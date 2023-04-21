using System;
using UnityEngine;

public class ContainerCounter : BaseCounter {
    [SerializeField] private KitchenObjectSO KitchenObjectSO;

    public event EventHandler OnPlayerGrabbedObject;

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            // Player isn't carrying anything
            KitchenObject.SpawnKitchenObject(KitchenObjectSO, player);

            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
}
