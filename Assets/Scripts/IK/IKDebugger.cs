using UnityEngine;
using TMPro;
using UnityEngine.UI;

public sealed class IKDebugger : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private IKController controller;

    [Header("UI")]
    [SerializeField] private TMP_Dropdown algorithmDropdown;
    [SerializeField] private Slider iterationsSlider;
    [SerializeField] private Slider epsilonSlider;
    [SerializeField] private TextMeshProUGUI infoText;

    private void Start()
    {
        if (controller == null) return;

        if (algorithmDropdown != null)
        {
            algorithmDropdown.ClearOptions();
            algorithmDropdown.AddOptions(new System.Collections.Generic.List<string> { "FABRIK", "CCD" });
            algorithmDropdown.value = controller.Algorithm == IKAlgorithm.FABRIK ? 0 : 1;
            algorithmDropdown.onValueChanged.AddListener(OnAlgorithmChanged);
        }

        if (iterationsSlider != null)
        {
            iterationsSlider.minValue = 1;
            iterationsSlider.maxValue = 64;
            iterationsSlider.value = controller.MaxIterations;
            iterationsSlider.onValueChanged.AddListener(v => controller.MaxIterations = Mathf.RoundToInt(v));
        }

        if (epsilonSlider != null)
        {
            epsilonSlider.minValue = 0.0005f;
            epsilonSlider.maxValue = 0.2f;
            epsilonSlider.value = controller.Epsilon;
            epsilonSlider.onValueChanged.AddListener(v => controller.Epsilon = v);
        }
    }

    private void Update()
    {
        if (controller == null || infoText == null) return;

        var r = controller.LastResult;
        infoText.text =
            $"Algorithm: {controller.Algorithm}\n" +
            $"MaxIter: {controller.MaxIterations}\n" +
            $"Epsilon: {controller.Epsilon:0.0000}\n" +
            $"IterUsed: {r.IterationsUsed}\n" +
            $"Error: {r.FinalError:0.0000}\n" +
            $"Reached: {r.Reached}";
    }

    private void OnAlgorithmChanged(int v)
    {
        if (controller == null) return;
        controller.Algorithm = v == 0 ? IKAlgorithm.FABRIK : IKAlgorithm.CCD;
    }
}
