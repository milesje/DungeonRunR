using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI timerText;

    private void Awake()
    {
        Instance = this;
    }


    public void UpdateTimer(float time)
    {
        timerText.text = $"Time: {time:F2}";
    }

}
