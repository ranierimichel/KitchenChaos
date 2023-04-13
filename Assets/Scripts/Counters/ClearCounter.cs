using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter {
    [SerializeField] private KitchenObjectSO KitchenObjectSO;

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // There isn't KitchenObject
            if (player.HasKitchenObject()) {
                // Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        } else {
            // There is KitchenObject
            if (player.HasKitchenObject()) {
                // Player is carrying something
            } else {
                // Player isn't carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player) {
    }
}
