using UnityEngine;
using TMPro;
using UnityEngine.UI;

public sealed class IKGUI : MonoBehaviour
{
    [Header("Refs")] [SerializeField] private IKController controller;

    [Header("UI"), SerializeField] private TextMeshProUGUI algorithmText;
    [SerializeField] private TextMeshProUGUI iterationsText;
    [SerializeField] private Slider iterationsSlider;
    [SerializeField] private TextMeshProUGUI epsilonText;
    [SerializeField] private Slider epsilonSlider;
    [SerializeField] private TextMeshProUGUI infoText;

    private void Start()
    {
        algorithmText.text = $"Algorithm: {controller.Algorithm}";
        iterationsSlider.value = controller.MaxIterations;
        iterationsSlider.onValueChanged.AddListener(v => controller.MaxIterations = Mathf.RoundToInt(v));

        epsilonSlider.value = controller.Epsilon;
        epsilonSlider.onValueChanged.AddListener(v => controller.Epsilon = v);
    }

    private void Update()
    {
        if (!controller || !infoText) return;

        IKResult r = controller.LastResult;
        iterationsText.text = $"Max Iterations: {controller.MaxIterations}";
        epsilonText.text = $"Epsilon: {controller.Epsilon:0.0000}";
        infoText.text =
            $"IterUsed: {r.IterationsUsed}\n" +
            $"Error: {r.FinalError:0.0000}\n" +
            $"Reached: {r.Reached}";
    }
}