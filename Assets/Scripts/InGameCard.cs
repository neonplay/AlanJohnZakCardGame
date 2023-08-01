using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCard : MonoBehaviour
{
	protected Vector3 HandPosition;
	protected Quaternion HandRotation;
	protected Vector3 baseScale = new Vector3(1, 1, 1);
	protected Vector3 enlargedScale = new Vector3(2.5f, 2.5f, 1);

	public void SetHandPositionAndRotation(Vector3 handPos, Quaternion handRot)
	{
		HandPosition = handPos;
		HandRotation = handRot;
	}

	public void LerpToHandDestination()
	{
		StartCoroutine(ReturnToHand(transform.localPosition, HandPosition, 0.25f));
	}

	private IEnumerator ReturnToHand(Vector3 startPos, Vector3 targetPos, float duration)
	{
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / duration);
			transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
			transform.localRotation = Quaternion.Lerp(transform.localRotation, HandRotation, t);
			yield return null;

			transform.localScale = Vector3.Lerp(transform.localScale, baseScale, t);

		}

		transform.localPosition = HandPosition;
		transform.localRotation = HandRotation;
		transform.localScale = baseScale;
	}
}
