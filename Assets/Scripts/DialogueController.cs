using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float dialogueSize = 24;
    private List<string> dialogueLines;

    private bool answer = false;
    private bool waitForInput = false;
    private Coroutine currentCoroutine;

    private int currentLine = 0;

    void Awake()
    {
        dialogueLines = new List<string>();
        dialogueLines.Add("Hello Professor. (Click to Advance The Tutorial)");
        dialogueLines.Add("Looks like a sorry state of things, doesn't it?");
        dialogueLines.Add("That's what I get for procastinating I guess. (You'd rather not know when I started this.)");
        dialogueLines.Add("Anyways, here's a brief tutorial to demonstrate what I've implemented.");
        dialogueLines.Add("You can pan the camera using the WASD keys.");
        dialogueLines.Add("Great work!");
        dialogueLines.Add("You can also zoom in and out using the Mouse Scroll Wheel.");
        dialogueLines.Add("Awesome!");
        dialogueLines.Add("As you can see, the current map seems to be a little nonuniform.");
        dialogueLines.Add("This is because the map is procedurally generated! (Though the code is a little messy, I'll refactor it later.)");
        dialogueLines.Add("Unfortunately the only way to really demonstrate it for now is to restart the exe.");
        dialogueLines.Add("There are also currently different possible map sizes, small medium and large.");
        dialogueLines.Add("Currently the only way to demonstrate those is to change the serialized variable Stage Size attached to the Director within the editor itself.");
        dialogueLines.Add("You can also drag and select units by holding the Left Mouse Button and releasing.");
        dialogueLines.Add("Try selecting me!");
        dialogueLines.Add("Now that I'm selected, you can also send me to go somewhere using the Right Mouse Button.");
        dialogueLines.Add("Try it!");
        dialogueLines.Add("Well. Ideally, I won't be floating to places and phasing through walls.");
        dialogueLines.Add("That's pretty much it for now.");
        dialogueLines.Add("Now that I've finally started, that initial dreadful barrier is gone and I should be working on this project a lot more now.");
        dialogueLines.Add("Hopefully next time you see me, I'll be a lot more impressive.");
        dialogueLines.Add("Goodbye professor.");
    }

    void Start()
    {
        dialogueText.fontSize = dialogueSize;
        if (currentLine < dialogueLines.Count)
        {
            ShowNextDialogueLine();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentLine < dialogueLines.Count && !waitForInput)
        {
            if (currentCoroutine != null) {
                StopCoroutine(currentCoroutine);
            }
            if (currentLine == 5)
            {
                currentCoroutine = StartCoroutine(ListenForWASD());
                StartCoroutine(WaitForAnswer());
            }
            else if (currentLine == 7)
            {
                currentCoroutine = StartCoroutine(ListenForScroll());
                StartCoroutine(WaitForAnswer());
            }
            else if (currentLine == 15) 
            {
                //Debug.Log("fiosadjfioasdj");
                GameObject tempManager = GameObject.FindWithTag("SelectionManager");
                //Debug.Log(tempManager);
                //Debug.Log(tempManager.GetComponent<SelectionManager>());
                currentCoroutine = StartCoroutine(ListenForPlayerSelect(tempManager.GetComponent<SelectionManager>()));
                StartCoroutine(WaitForAnswer());
            }
            else if (currentLine == 17) 
            {
                GameObject tempManager = GameObject.FindWithTag("SelectionManager");
                currentCoroutine = StartCoroutine(ListenForPlayerMovement(tempManager.GetComponent<SelectionManager>()));
                StartCoroutine(WaitForAnswer());
            }
            else
            {
                ShowNextDialogueLine();
            }
        }
        else if (currentLine == dialogueLines.Count && Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowNextDialogueLine()
    {
        dialogueText.text = dialogueLines[currentLine];
        currentLine++;
    }

    private IEnumerator WaitForAnswer()
    {
        waitForInput = true;
        yield return new WaitUntil(() => answer);
        answer = false;
        waitForInput = false;
        ShowNextDialogueLine();
    }

    private IEnumerator ListenForWASD()
    {
        while (!answer)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Yup");
                answer = true;
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator ListenForScroll()
    {
        while (!answer)
        {
            if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0f)
            {
                answer = true;
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator ListenForPlayerSelect(SelectionManager tempManager)
    {
        while (!answer)
        {
            if (tempManager.hasSelectedUnits())
            {
                answer = true;
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator ListenForPlayerMovement(SelectionManager tempManager)
    {
        while (!answer)
        {
            if (tempManager.hasSelectedUnits() && Input.GetMouseButtonDown(1))
            {
                answer = true;
                yield break;
            }
            yield return null;
        }
    }
}
