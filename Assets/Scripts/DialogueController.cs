using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float dialogueSize = 24;
    private List<string> dialogueKeys;

    private Dictionary<string, Action> dialogueLines;
    private bool answer = false;
    private bool waitForInput = false;
    private bool answerReceived = false;
    private Coroutine currentCoroutine;

    private int currentLine = 0;

    void Awake()
    {
        dialogueKeys = new List<string>();
        dialogueLines = new Dictionary<string, Action>();
        dialogueKeys.Add("Hello again, Professor. (Click on the dialogue to progress.)");
        dialogueKeys.Add("I'm going to be quite honest.");
        dialogueKeys.Add("I severely underestimated how much work was going to be required before getting a satisfactory product.");
        dialogueKeys.Add("I had big plans for this test demo.");
        dialogueKeys.Add("There was going to be this cool nice tutorial sequence where I can guide you along showing all the cool stuff I implemented.");
        dialogueKeys.Add("And then it was going to end with some big boss showing up early, disabling the escape and pause buttons with cool special UI effects and shooting beams everywhere.");
        dialogueKeys.Add("And then I would die and the tutorial would end.");
        dialogueKeys.Add("Well, unfortunately that doesn't get to happen because it turns out writing clean maintainable code is hard.");
        dialogueKeys.Add("I had to implement Dijkstra's algorithm, learn about Quadtrees, and implement a hashmap just to move correctly.");
        dialogueKeys.Add("Okay, rant over let's move on with the tutorial.");
        dialogueKeys.Add("I'm sure you remember doing this last time, but let's go over it again just as procedure.");
        dialogueKeys.Add("You can pan the camera using WASD.");
        dialogueKeys.Add("You can also zoom in and out using the mouse wheel.");
        dialogueKeys.Add("As a side note, that listening for input functionality took another hour of debugging despite me having completed it during the Progress Report.");
        dialogueKeys.Add("At least it's refactored to be a lot more maintainable now.");
        dialogueKeys.Add("Anyways, remember that you can select me (or any playable character if those were implemented) by dragging and releasing the left mouse button.");
        dialogueKeys.Add("And of course, you can tell me where to go using right mouse click while I'm selected.");
        dialogueKeys.Add("Perhaps you've noticed already, but there's a sort of fog of war visible on the map.");
        dialogueKeys.Add("Player characters have a vision range that affects the amount of visible rooms around them.");
        dialogueKeys.Add("As demonstrated, I should be able to see a decent amount farther away now.");
        dialogueKeys.Add("And now, I can only see my current room.");
        dialogueKeys.Add("In addition, any enemy type entities are not visible by the player when in fog of war.");
        dialogueKeys.Add("Speaking of enemies, there should be a stop sign looking thing somewhere on this map - try to find it");
        dialogueKeys.Add("When a player character is selected and you right click on a visible enemy, they will attempt to attack it.");
        dialogueKeys.Add("Try it on this defenseless target!");
        dialogueKeys.Add("While you wait and appreciate my neat little attack animations, I can tell you about some other features as well.");
        dialogueKeys.Add("You can pause the game by pressing Spacebar.");
        dialogueKeys.Add("Pressing Escape will bring up the world's worst escape menu.");
        dialogueKeys.Add("I guess that's kind of it? Unless there was a day or two before you were able to grade this project, in which case I might have added something.");
        dialogueKeys.Add("As a closing statement, I will say I'm frustrated with what I've achieved thus far.");
        dialogueKeys.Add("In our last class, you said that we could contact you once we're graduated.");
        dialogueKeys.Add("I'm graduating this semester, and I will take you up on that offer.");
        dialogueKeys.Add("This project may be incomplete, but I swear on my honor I will complete it.");
        dialogueKeys.Add("And perhaps a month, a few months, or even later I will show something that will impress.");
        dialogueKeys.Add("Well, that's all the necessary things I wanted to say.");
        dialogueKeys.Add("Until next time, Professor.");
        //dialogueKeys.Add("You can keep clicking through my dialogues for any more of my thoughts, but nothing I will say will have any bearing on the grade of this project, so you don't have to stay for any of it.");
        //dialogueKeys.Add("It turns out ha");

        dialogueLines.Add("You can pan the camera using WASD.", handleWASD);
        dialogueLines.Add("You can also zoom in and out using the mouse wheel.", handleScroll);
        dialogueLines.Add("Anyways, remember that you can select me (or any playable character if those were implemented) by dragging and releasing the left mouse button.", handlePlayerSelect);
        dialogueLines.Add("And of course, you can tell me where to go using right mouse click while I'm selected.", handlePlayerMovement);
        dialogueLines.Add("As demonstrated, I should be able to see a decent amount farther away now.", handleCarrot);
        dialogueLines.Add("And now, I can only see my current room.", handleAntiCarrot);
        dialogueLines.Add("In addition, any enemy type entities are not visible by the player when in fog of war.", handleNormalVision);
        dialogueLines.Add("Speaking of enemies, there should be a stop sign looking thing somewhere on this map - try to find it", handleFindTutorialDude);
        dialogueLines.Add("Try it on this defenseless target!", handleAttackTarget);
        dialogueLines.Add("You can pause the game by pressing Spacebar.", handleSpaceBar);
        dialogueLines.Add("Pressing Escape will bring up the world's worst escape menu.", handleEscape);
    }

    void Start()
    {
        dialogueText.fontSize = dialogueSize;
        if (currentLine < dialogueKeys.Count)
        {
            ShowNextDialogueLine();
        }
    }

    private void handleEscape() {
        currentCoroutine = StartCoroutine(ListenForEscape());
        StartCoroutine(WaitForAnswer());
    }

    private IEnumerator ListenForEscape() {
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Escape)));
        answer = true;
    }

    private void handleSpaceBar() {
        currentCoroutine = StartCoroutine(ListenForSpacebar());
        StartCoroutine(WaitForAnswer());
    }

    private IEnumerator ListenForSpacebar() {
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Space)));
        answer = true;
    }

    private void handleAttackTarget() {
        GameObject tempEnemy = GameObject.FindWithTag("Enemy");
        currentCoroutine = StartCoroutine(ListenForAttack(tempEnemy.GetComponent<TutorialEnemy>()));
        StartCoroutine(WaitForAnswer());
    }

    private IEnumerator ListenForAttack(TutorialEnemy enemy) {
        yield return new WaitUntil(() => (enemy.isDamaged()));
        answer = true;
    }

    private void handleFindTutorialDude() {
        GameObject tempEnemy = GameObject.FindWithTag("Enemy");
        if (!tempEnemy.GetComponent<EnemyController>().IsInFog()) {
            dialogueKeys.Insert(currentLine + 1, "Oh, I see you've already found the tutorial enemy.");
        }
        else {
            dialogueKeys.Insert(currentLine + 1, "Oh there it is.");
            currentCoroutine = StartCoroutine(ListenForFoundEnemy(tempEnemy.GetComponent<EnemyController>()));
            StartCoroutine(WaitForAnswer());
        }
    }

    private void handleCarrot() {
        GameObject tempPlayer = GameObject.FindWithTag("Player");
        tempPlayer.GetComponent<PlayerController>().changeVisionRange(2);
        StartCoroutine(WaitBeforeNextLine(3f));
    }

    private void handleAntiCarrot() {
        GameObject tempPlayer = GameObject.FindWithTag("Player");
        tempPlayer.GetComponent<PlayerController>().changeVisionRange(0);
        StartCoroutine(WaitBeforeNextLine(3f));
    }

    private void handleNormalVision() {
        GameObject tempPlayer = GameObject.FindWithTag("Player");
        tempPlayer.GetComponent<PlayerController>().changeVisionRange(1);
    }

    private void handleWASD() {
        currentCoroutine = StartCoroutine(ListenForWASD());
        StartCoroutine(WaitForAnswer());
    }

    private void handleScroll() {
        currentCoroutine = StartCoroutine(ListenForScroll());
        StartCoroutine(WaitForAnswer());
    }

    private void handlePlayerSelect() {
        GameObject tempManager = GameObject.FindWithTag("Director");
        currentCoroutine = StartCoroutine(ListenForPlayerSelect(tempManager.GetComponent<SelectionManager>()));
        StartCoroutine(WaitForAnswer());
    }

    private void handlePlayerMovement() {
        GameObject tempManager = GameObject.FindWithTag("Director");
        currentCoroutine = StartCoroutine(ListenForPlayerMovement(tempManager.GetComponent<SelectionManager>()));
        StartCoroutine(WaitForAnswer());
    }

    private bool IsMouseOverUIElement(RectTransform rectTransform)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        graphicRaycaster.Raycast(eventData, results);

        return results.Exists(result => result.gameObject.GetComponent<RectTransform>() == rectTransform);
    }

    private void Update()
    {
        RectTransform childRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        bool isMouseOverChild = IsMouseOverUIElement(childRectTransform);

        if ((Input.GetMouseButtonDown(0) && currentLine < dialogueKeys.Count && !waitForInput && isMouseOverChild) || answerReceived)
        {
            if (answerReceived) {
                answerReceived = false;
            }
            string currentKey = dialogueKeys[currentLine];
            Action currentAction;
            if (dialogueLines.ContainsKey(currentKey)) {
                Debug.Log("is in dict");
                currentAction = dialogueLines[currentKey];
            }
            else currentAction = null;

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            if (currentAction != null)
            {
                currentCoroutine = StartCoroutine(currentAction.Method.Name);
            }

            ShowNextDialogueLine();
        }
        else if (currentLine == dialogueKeys.Count && Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);
        }
    }


    private void ShowNextDialogueLine()
    {
        dialogueText.text = dialogueKeys[currentLine];
        currentLine++;
    }

    private IEnumerator WaitForAnswer()
    {
        waitForInput = true;
        yield return new WaitUntil(() => answer);
        answer = false;
        waitForInput = false;
        answerReceived = true;
        //ShowNextDialogueLine();
    }

    private IEnumerator WaitBeforeNextLine(float t) {
        waitForInput = true;
        yield return new WaitForSeconds(t);
        waitForInput = false;
    }

    private IEnumerator ListenForWASD()
    {
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)));
        answer = true;
    }

    private IEnumerator ListenForFoundEnemy(EnemyController enemy) {
        yield return new WaitUntil(() => (!enemy.IsInFog()));
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
