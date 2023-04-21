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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // Player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                } else {
                    // Player is carrying something but not a plate
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        // Counter is holding a plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            } else {
                // Player isn't carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
