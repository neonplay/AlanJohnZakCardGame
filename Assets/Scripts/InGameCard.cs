using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameCard : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
{
	public string CardName;
	public CardStats BaseStats;
	public CardStats CurrentStats { get; set; }

	protected Vector3 HandPosition;
	protected Quaternion HandRotation;
	protected Vector3 baseScale = new Vector3(1, 1, 1);
	protected Vector3 enlargedScale = new Vector3(1.5f, 1.5f, 1);

	bool pressed;
	bool isDragging;
	float timePressed = 0;
	private int handIndex;

	private RectTransform rectTransform;

	CardPlayingManager cardPlayingManager;

	public bool isReward;

    private void Awake()
    {
		cardPlayingManager = FindObjectOfType<CardPlayingManager>();
    }

    private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		UpdateCurrentStats();
	}

	private void UpdateCurrentStats()
    {
		CurrentStats = new CardStats();
		CurrentStats.Attack = BaseStats.Attack + cardPlayingManager.BuffsAndDebuffs.Strength;
	}

	private void Update()
	{
		if (pressed && !isDragging)
		{
			timePressed += Time.deltaTime;
			if (timePressed >= 0.5f)
			{
			}
		}

		MoveCard();
	}

	private void MoveCard()
    {
		if (isDragging)
		{
			float lerpSpeed = 20;

			Vector3 mousePos = Input.mousePosition;

			// Convert the screen space mouse position to local space within the Canvas
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, mousePos, null, out Vector2 localMousePos))
			{
				// Update the UI element's position to follow the mouse
				rectTransform.localPosition = localMousePos;
				transform.eulerAngles = Vector3.zero;
			}

			transform.localScale = Vector3.Lerp(transform.localScale, enlargedScale, Time.deltaTime * lerpSpeed);

		}
	}

	public void ReturnToHand(Transform handParent)
    {
		transform.SetParent(handParent);
		transform.SetSiblingIndex(handIndex);
		StartCoroutine(ReturnToHandLerp());
    }

	private IEnumerator ReturnToHandLerp()
    {
		float progress = 0;

		while(progress < 1)
        {
			progress += Time.deltaTime * 8;
			yield return new WaitForEndOfFrame();
			transform.localScale = Vector3.Lerp(enlargedScale, baseScale, progress);
		}
	}

	#region Position in hand

	public void SetHandPositionAndRotation(Vector3 handPos, Quaternion handRot)
	{
		HandPosition = handPos;
		HandRotation = handRot;
		handIndex = transform.GetSiblingIndex();
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

	#endregion

	#region Press and drag

    public void OnPointerClick(PointerEventData eventData)
    {
		if (isReward)
        {
			FindObjectOfType<CurrentRunManager>().RewardCardSelected(this);
			return;
        }

		if(pressed && !isDragging)
        {

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
		if (isReward)
		{

			return;
		}
		pressed = true;
		timePressed = 0;
	}

	public void OnPointerMove(PointerEventData eventData)
    {
		if (isReward)
		{

			return;
		}
		if (pressed)
        {
			if(!isDragging)
            {
				cardPlayingManager.CardSelectedFromHand(this);
            }
			isDragging = true;
		}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
		if (isReward)
		{

			return;
		}
		if (isDragging)
		{
			pressed = false;
			isDragging = false;

			cardPlayingManager.TryPlayCard(this, rectTransform.localPosition.y);
		}
	}

    #endregion

	public void SendToDiscard(Transform discardPosition)
    {
		IEnumerator MoveToDiscard()
        {
			float progress = 0;
			transform.parent = discardPosition;
			var startPos = rectTransform.localPosition;

			while (progress < 1)
			{
				progress += Time.deltaTime * 5;
				yield return new WaitForEndOfFrame();
				rectTransform.localPosition = Vector3.Lerp(startPos, Vector3.zero, progress);
			}
		}

		StartCoroutine(MoveToDiscard());
    }
}

[System.Serializable]
public class CardStats
{
	public int Attack;
	public int ManaCost;
}