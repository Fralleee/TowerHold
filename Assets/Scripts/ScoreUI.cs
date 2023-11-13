using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    public void SetScores(string scores)
    {
        scoreText.text = scores;
    }
}
