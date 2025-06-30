using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour {
    [SerializeField] private Image progressBar;
    private const float dummyProgressTimerMax = 3.0f;
    private float dummyProgressTimer = 0.0f;

    private void Update() {
        float progress = Loader.GetLoadProgress();

        float dummyProgress = 0.0f;
        if (progress >= 0.9f) {
            dummyProgressTimer += Time.deltaTime;
            dummyProgress = dummyProgressTimer / dummyProgressTimerMax * 0.1f;
        }

        progressBar.fillAmount = progress + dummyProgress;
    }
}
