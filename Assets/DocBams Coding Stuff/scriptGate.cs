using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scriptGate : MonoBehaviour
{
	public string countdownText = "30";

	private TextMeshProUGUI textUI;

	//Coroutines
	private IEnumerator countdownRoutine;

	#region Monobehaviour Stuff
	private void Start()
	{
		textUI = GetComponentInChildren<TextMeshProUGUI>();
		if (textUI == null) Debug.LogError("Gate missing Text component.", gameObject);
		
		textUI.text = countdownText;
	}
	#endregion

	#region External Inputs
	public void StartCountdown()
	{
		if (countdownRoutine == null)
		{
			countdownRoutine = CountdownRoutine();
			StartCoroutine(countdownRoutine);
		}
	}
	#endregion

	#region Coroutines
	private IEnumerator CountdownRoutine()
	{
		int currentValue = int.Parse(textUI.text);

		while (currentValue > 0)
		{
			currentValue--;

			textUI.text = currentValue.ToString();

			yield return new WaitForSeconds(1);
		}

		//audio

		//destroy
		Destroy(gameObject);

		countdownRoutine = null;
	}
	#endregion
}
