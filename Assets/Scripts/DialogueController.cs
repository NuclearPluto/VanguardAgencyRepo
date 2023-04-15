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
        /*0*/dialogueLines.Add("Hello Professor. (Click to Advance The Tutorial)");
        /*1*/dialogueLines.Add("Looks like a sorry state of things, doesn't it?");
        /*2*/dialogueLines.Add("That's what I get for procastinating I guess. (You'd rather not know when I started this.)");
        /*3*/dialogueLines.Add("Anyways, here's a brief tutorial to demonstrate what I've implemented.");
        /*5*/dialogueLines.Add("You can pan the camera using the WASD keys.");
        /*6*/dialogueLines.Add("Great work!");
        /*7*/dialogueLines.Add("You can also zoom in and out using the Mouse Scroll Wheel.");
        /*8*/dialogueLines.Add("Awesome!");
        /*9*/dialogueLines.Add("As you can see, the current map seems to be a little nonuniform.");
        /*10*/dialogueLines.Add("This is because the map is procedurally generated! (Though the code is a little messy, I'll refactor it later.)");
        /*11*/dialogueLines.Add("Unfortunately the only way to really demonstrate it for now is to restart the exe.");
        /*12*/dialogueLines.Add("There are also currently different possible map sizes, small medium and large.");
        /*13*/dialogueLines.Add("Currently the only way to demonstrate those is to change the serialized variable Stage Size attached to the Director within the editor itself.");
        /*14*/dialogueLines.Add("You can also drag and select units by holding the Left Mouse Button and releasing.");
        /*15*/dialogueLines.Add("Try selecting me!");
        /*16*/dialogueLines.Add("Now that I'm selected, you can also send me to go somewhere using the Right Mouse Button.");
        /*17*/dialogueLines.Add("Try it!");
        /*18*/dialogueLines.Add("Well. Ideally, I won't be floating to places and phasing through walls.");
        /*19*/dialogueLines.Add("That's pretty much it for now.");
        /*20*/dialogueLines.Add("Now that I've finally started, that initial dreadful barrier is gone and I should be working on this project a lot more now.");
        /*21*/dialogueLines.Add("Hopefully next time you see me, I'll be a lot more impressive.");
        /*22*/dialogueLines.Add("Goodbye professor.");
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
            if (currentLine == 4)
            {
                ShowNextDialogueLine();
                currentCoroutine = StartCoroutine(ListenForWASD());
                StartCoroutine(WaitForAnswer());
            }
            else if (currentLine == 6)
            {
                ShowNextDialogueLine();
                currentCoroutine = StartCoroutine(ListenForScroll());
                StartCoroutine(WaitForAnswer());
            }
            else if (currentLine == 14) 
            {
                ShowNextDialogueLine();
                //Debug.Log("fiosadjfioasdj");
                GameObject tempManager = GameObject.FindWithTag("Director");
                //Debug.Log(tempManager);
                //Debug.Log(tempManager.GetComponent<SelectionManager>());
                currentCoroutine = StartCoroutine(ListenForPlayerSelect(tempManager.GetComponent<SelectionManager>()));
                StartCoroutine(WaitForAnswer());
            }
            else if (currentLine == 16) 
            {
                ShowNextDialogueLine();
                GameObject tempManager = GameObject.FindWithTag("Director");
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
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)));
        answer = true;
    }

    private IEnumerator ListenForScroll()
    {
        yield return new WaitUntil(() => (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0f));
        answer = true;
    }

    private IEnumerator ListenForPlayerSelect(SelectionManager tempManager)
    {
        yield return new WaitUntil(() => (tempManager.hasSelectedUnits()));
        answer = true;
    }
    private IEnumerator ListenForPlayerMovement(SelectionManager tempManager)
    {
        yield return new WaitUntil(() => (tempManager.hasSelectedUnits() && Input.GetMouseButtonDown(1)));
        answer = true;
    }
}
