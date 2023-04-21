using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour {
    private const string NUMBER_POUP = "NumberPopup";

    [SerializeField] private TextMeshProUGUI countDownText;

    private Animator animator;
    private int previousCountdownNumber;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManager.Instance.IsCountDownToStartActive()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Update() {
        int countdownNumber = Mathf.CeilToInt(KitchenGameManager.Instance.GetCountDownToStartTimer());
        countDownText.text = countdownNumber.ToString();
        if (previousCountdownNumber != countdownNumber) {
            previousCountdownNumber = countdownNumber;
            animator.SetTrigger(NUMBER_POUP);
            SoundManager.Instance.PlayCountdownSound();
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
}
