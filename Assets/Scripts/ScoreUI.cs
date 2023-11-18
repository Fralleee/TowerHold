using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI _scoreText;

	public void SetScores(string scores) => _scoreText.text = scores;
}
