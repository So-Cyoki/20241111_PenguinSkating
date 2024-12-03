using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI _scoreText;
    public TextMeshProUGUI _kidNumText;
    public Slider _playerSlider;

    void ScoreUpdate(int score)
    {
        string scoreText = score.ToString("D10");
        _scoreText.text = scoreText + " m";
    }
    void KidCountUpdate(int count)
    {
        string text = count.ToString("D2");
        _kidNumText.text = "X" + text;
    }
    void PlayerStaminaUpdate(float max, float current)
    {
        float value = current / max;
        _playerSlider.value = value;
    }

    private void OnEnable()
    {
        Player.OnScoreUpdate += ScoreUpdate;
        IcePlane.OnKidCount += KidCountUpdate;
        Player.OnStaminaUpdate += PlayerStaminaUpdate;
    }
    private void OnDisable()
    {
        Player.OnScoreUpdate -= ScoreUpdate;
        IcePlane.OnKidCount -= KidCountUpdate;
        Player.OnStaminaUpdate -= PlayerStaminaUpdate;
    }
}
