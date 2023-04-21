using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObejctParent {

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    public static Player Instance { get; private set; }

    private bool isWalking;
    private Vector3 lastIntereactDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    public bool IsWalking() {
        return isWalking;
    }

    private void Awake() {
        if (Instance != null) Debug.LogError("There is more than one player instance!");

        Instance = this;
    }

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }
    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        float rotateSpeed = 10;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        // Can't move towards moveDir
        if (!canMove) {
            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            // Can move only on the X
            if (canMove) moveDir = moveDirX;
            else { // Can't move only on the X
                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                //Can move only on the Z
                if (canMove) moveDir = moveDirZ;
            }
        }
        if (canMove) transform.position += moveSpeed * Time.deltaTime * moveDir;

        isWalking = moveDir != Vector3.zero;
        if (isWalking) transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }


    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }
    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new(inputVector.x, 0, inputVector.y);

        if (moveDir != Vector3.zero) lastIntereactDir = moveDir;

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastIntereactDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (baseCounter != selectedCounter) SetSelectedCounter(baseCounter);
            } else SetSelectedCounter(null);
        } else SetSelectedCounter(null);
    }
    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        this.kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}
