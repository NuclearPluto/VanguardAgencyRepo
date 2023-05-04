using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject uiContainer;

    public float moveDuration = 1.0f;
    public Vector2 hiddenPosition;
    public Vector2 shownPosition;
    protected RectTransform containerRectTransform;

    protected virtual void Awake()
    {
        containerRectTransform = uiContainer.GetComponent<RectTransform>();
        containerRectTransform.anchoredPosition = new Vector3(0f, 200f, 0f);
    }

    protected virtual void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(MoveContainer(shownPosition, moveDuration));
    }

    public void HideAndDisable()
    {
        StopAllCoroutines();
        StartCoroutine(MoveContainer(hiddenPosition, moveDuration, true));
    }

    IEnumerator MoveContainer(Vector2 targetPosition, float duration, bool disableAfter = false)
    {
        Vector2 startPosition = containerRectTransform.anchoredPosition;

        float elapsedTime = 0;
        Debug.Log("this runs once?");

        yield return new WaitForEndOfFrame(); // Wait for the end of the frame

        while (elapsedTime < duration)
        {
            Debug.Log("the t value is " + elapsedTime / duration);
            Debug.Log("Time.deltaTime: " + Time.fixedDeltaTime);
            containerRectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);

            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }

        containerRectTransform.anchoredPosition = targetPosition;

        if (disableAfter)
        {
            gameObject.SetActive(false);
        }
    }


}
