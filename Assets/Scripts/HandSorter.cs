using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSorter : MonoBehaviour
{
	public static float MinSpacing = 200;
	public static float MaxSpacing = 200;
	public static float minRotationAngle = -3;
	public static float maxRotationAngle = -6f;
	public static float MaxRaiseAmount = 15;
	public static int MaxHandSize = 5;

	public static void UpdateHandPositionsAndRotations(Transform parentTransform)
	{
		int childCount = parentTransform.childCount;

		// Calculate the spacing based on the hand size
		float spacing = Mathf.Lerp(MaxSpacing, MinSpacing, (float)childCount / (float)MaxHandSize);

		// Calculate the total width required for spacing out the children
		float totalWidth = (childCount - 1) * spacing;

		// Calculate the starting local x position
		float startX = -totalWidth / 2f;

		// Calculate the maximum and minimum rotation angles based on the number of cards
		float maxRotation = Mathf.Lerp(minRotationAngle, maxRotationAngle, (float)childCount / (float)MaxHandSize);

		// Iterate through each child and position them horizontally
		for (int i = 0; i < childCount; i++)
		{
			var card = parentTransform.GetChild(i).GetComponent<InGameCard>();

			Vector3 pos = new Vector3();
			Quaternion rot = new Quaternion();

			// Calculate the target local x position for the child
			float targetLocalX = startX + i * spacing;

			// Calculate the distance from the center
			float distanceFromCenter = Mathf.Abs(targetLocalX);

			// Calculate the rotation angle based on the distance from the center and the card count
			float rotationAngle = 0;
			if (totalWidth != 0)
				rotationAngle = maxRotation * (distanceFromCenter / (totalWidth / 2f));

			// Adjust the rotation based on the target local x position
			Quaternion rotation = Quaternion.Euler(0f, 0f, rotationAngle * Mathf.Sign(targetLocalX));

			// Set the child's local position and rotation
			Vector3 newLocalPosition = new Vector3(targetLocalX, 0, 0);
			rot = rotation;

			// Calculate the raise amount based on the distance from the center
			float raiseAmount = 0;
			if (totalWidth != 0)
				raiseAmount = Mathf.Lerp(0f, MaxRaiseAmount, 1f - (distanceFromCenter / (totalWidth / 2f)));

			if(childCount == 5)
            {
				if(i == 1 || i == 3)
                {
					raiseAmount += 2.5f;
                }
            }

			raiseAmount = Mathf.Lerp(0f, raiseAmount, childCount / 5f);

			// Adjust the y position based on the raise amount
			newLocalPosition.y += raiseAmount;

			// Calculate the z value based on the child's order
			float zValue = -i * 0.05f;

			// Set the child's local z position
			newLocalPosition.z = zValue;
			pos = newLocalPosition;

			card.SetHandPositionAndRotation(pos, rot);
			card.LerpToHandDestination();
		}
	}
}
