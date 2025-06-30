using UnityEngine;
using UnityEngine.UI;

namespace YOPO.SIM.UI {
    public class SceneSwitcher : MonoBehaviour {
        [SerializeField] private Button _evaluationButton;
        [SerializeField] private Button _dataGenerationButton;

        void Start() {
            _evaluationButton.onClick.AddListener(() => {
                Loader.LoadScene(Loader.Scene.EvaluationScene);
            });
            _dataGenerationButton.onClick.AddListener(() => {
                Loader.LoadScene(Loader.Scene.DataGenerationScene);
            });
        }
    }

}