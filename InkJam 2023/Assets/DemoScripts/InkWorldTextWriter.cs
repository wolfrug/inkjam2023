using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InkEngine
{


    public class InkWorldTextWriter : InkWriter
    {
        public InkWorldSpaceCharacter m_currentSpeakingCharacter;
        public List<InkWorldSpaceCharacter> m_worldCharacters = new List<InkWorldSpaceCharacter> { };
        public float m_displayTimePerSay = 5f;
        private SimpleInkDialogBox m_simpleDialogBox;
        public override void Awake()
        {
            base.Awake();
            // Add the default searchable INTERACTABLE tag - used by choices to make them inactive if this function tag is found
            m_storyData.AddSearchableFunction(new InkTextVariable
            {
                variableName = "INTERACTABLE",
                VariableArguments = new List<string> { "false" }
            });
            m_simpleDialogBox = m_currentDialogBox as SimpleInkDialogBox;
            foreach (InkWorldSpaceCharacter chara in m_worldCharacters)
            {
                chara.Init();
            }
        }

        public void AddInkWorldSpaceCharacter(InkWorldSpaceCharacter characterToAdd)
        {
            if (!m_worldCharacters.Contains(characterToAdd))
            {
                m_worldCharacters.Add(characterToAdd);
            }
        }
        public void SetCurrentSpeakingCharacter(InkWorldSpaceCharacter currentCharacter)
        {
            m_currentSpeakingCharacter = currentCharacter;
        }

        public SimpleInkDialogBox CurrentDialogBoxSimple
        {
            get
            {
                return m_simpleDialogBox;
            }
            set
            {
                m_simpleDialogBox = value;
            }
        }
        // Where most of the magic happens: takes the line of dialogue + possible expected choices, displays them one by one by spawning text objects
        // And then displayes the options by spawning buttons
        public override IEnumerator DisplayText(InkDialogueLine[] dialogueLines, List<InkChoiceLine> gatherChoices)
        {
            if (CurrentDialogBoxSimple.HasContinueButton)
            {
                CurrentDialogBoxSimple.SetContinueButtonActive(true);
                CurrentDialogBoxSimple.m_canContinue = false;
            }
            else
            {
                CurrentDialogBoxSimple.m_canContinue = true;
            }
            for (int i = 0; i < dialogueLines.Length; i++)
            {
                InkDialogueLine currentLine = dialogueLines[i];
                InvokeDialogueEvents(currentLine);
                yield return StartCoroutine(ParseSpecialTags(currentLine.inkTags));
                GameObject currentBox = null;
                if (currentLine.displayText.Trim() != "")
                {
                    currentBox = SpawnTextObject(currentLine);
                    m_dialogueShownEvent.Invoke(currentLine);
                };
                if (currentBox != null)
                {
                    yield return new WaitUntil(() => CurrentDialogBoxSimple.m_canContinue);
                    yield return new WaitForSeconds(m_displayTimePerSay);
                    currentBox?.GetComponent<Animator>()?.SetTrigger("hide");
                }
                if (CurrentDialogBoxSimple.HasContinueButton)
                {
                    CurrentDialogBoxSimple.m_canContinue = false;
                }
            }
            if (gatherChoices != null)
            {
                if (gatherChoices.Count > 0)
                {
                    List<(InkChoiceLine, Button)> allButtons = new List<(InkChoiceLine, Button)> { };
                    foreach (InkChoiceLine choice in gatherChoices)
                    {
                        Button button = SpawnOptionObject(choice);
                        button.interactable = SetChoiceInteractable(choice.choiceText);
                        allButtons.Add((choice, button));
                    }
                    foreach ((InkChoiceLine, Button) set in allButtons)
                    {
                        set.Item2.onClick.AddListener(() => PressOptionButton(set.Item1, set.Item2, allButtons));
                    }
                    m_optionPressed = false;
                    m_waitingOnOptionPress = true;
                    CurrentDialogBoxSimple.SetContinueButtonActive(false);
                    m_choicesShownEvent.Invoke(allButtons);
                    yield return new WaitUntil(() => m_optionPressed);
                }
                else
                { // we only invoke the writer finished event if there really are no more choices
                    m_writerFinishedEvent.Invoke(this);
                    m_displayCoroutine = null;
                }
            }
            else
            { // we only invoke the writer finished event if there really are no more choices
                m_writerFinishedEvent.Invoke(this);
                m_displayCoroutine = null;
            };
        }

        IEnumerator ParseSpecialTags(List<string> tags)
        {
            // These are just hard-coded Ink tags we might want to use for various things to do with the writer...
            foreach (string tag in tags)
            {
                if (tag.Contains("wait."))
                {
                    string tagNr = tag.Replace("wait.", ""); // remove the wait
                    float waitTime = float.Parse(tagNr);
                    yield return new WaitForSeconds(waitTime);
                }
            }
            if (tags.Contains("hideDialogue"))
            {
                CloseCurrentDialogBox(false);
            }
            if (tags.Contains("showDialogue"))
            {
                OpenCurrentDialogBox(false);
            }
            if (tags.Contains("continue"))
            {
                CurrentDialogBoxSimple.m_canContinue = true;
            }

            // Keep this one at the bottom, or it will pause before the other tags!
            if (tags.Contains("pause"))
            {
                PauseWriter(true);
                yield return new WaitUntil(() => CurrentDialogBoxSimple.m_canContinue);
            }
        }

        bool SetChoiceInteractable(InkDialogueLine choiceLine)
        {
            // Hard coded thing to set a button to interactable or not...
            if (choiceLine.HasVariableWithArgument("INTERACTABLE", "false"))
            {
                return false;
            }
            return true;
        }
        protected override void PressOptionButton(InkChoiceLine optionButton, Button selectedButton, List<(InkChoiceLine, Button)> allButtons)
        {
            // We only press one
            m_optionPressed = true;
            m_waitingOnOptionPress = false;
            InvokeDialogueEvents(optionButton.choiceText);
            PlayChoice(optionButton.choice);
            // Make the selected button uninteractable but stay behind, hide all the others
            selectedButton.interactable = false;
            foreach ((InkChoiceLine, Button) set in allButtons)
            {
                //if (set.Item2 != selectedButton)
                //{
                set.Item2.gameObject.SetActive(false);
                //}
            }
        }
        public void PauseWriter(bool pause = true)
        {
            if (pause)
            {
                if (m_displayCoroutine != null)
                {
                    CurrentDialogBoxSimple.m_canContinue = false;
                    CurrentDialogBoxSimple.SetContinueButtonActive(false);
                }
            }
            else
            {
                if (m_displayCoroutine != null)
                {
                    CurrentDialogBoxSimple.m_canContinue = true;
                    if (!m_waitingOnOptionPress)
                    {
                        CurrentDialogBoxSimple.SetContinueButtonActive(true);
                    };
                }
            }
        }

        GameObject SpawnTextObject(InkDialogueLine currentLine)
        {
            string boxPrefab = "default";
            InkWorldSpaceCharacter targetCharacter = null;
            if (currentLine.HasVariable("SET_TEXTBOX"))
            {
                boxPrefab = currentLine.GetVariable("SET_TEXTBOX").VariableArguments[0];
                if (currentLine.GetVariable("SET_TEXTBOX").VariableArguments.Count > 1)
                {
                    string targetTag = currentLine.GetVariable("SET_TEXTBOX").VariableArguments[1];
                    if (targetTag != "current")
                    {
                        targetCharacter = m_worldCharacters.Find((x) => x.m_characterTag == targetTag);
                    }
                    else
                    {
                        targetCharacter = m_currentSpeakingCharacter;
                    }
                }
            }
            GameObject box = CurrentDialogBoxSimple.SpawnTextObject(currentLine.displayText, boxPrefab);
            if (targetCharacter != null)
            {
                box.transform.SetParent(CurrentDialogBoxSimple.transform);
                GenericWorldSpaceToCanvasIcon icon = targetCharacter.Icon;
                icon.canvasObject = box.GetComponent<RectTransform>();
                // Add destroy timer
                box.GetComponentInChildren<TypeWriter>().stoppedEvent_.AddListener(delegate
                {
                    CurrentDialogBoxSimple.m_canContinue = true;
                    Destroy(box, m_displayTimePerSay * 2f);
                    Destroy(icon, m_displayTimePerSay * 1.5f);
                });
                m_currentSpeakingCharacter = targetCharacter;
            }
            return box;
        }
        Button SpawnOptionObject(InkChoiceLine choice)
        {
            string optionButtonPrefab = "default";
            if (choice.choiceText.HasVariable("SET_OPTIONBOX"))
            {
                optionButtonPrefab = choice.choiceText.GetVariable("SET_OPTIONBOX").VariableArguments[0];
            }
            GameObject buttonGO = CurrentDialogBoxSimple.SpawnButtonObject(choice.choiceText.displayText, optionButtonPrefab);
            if (buttonGO.GetComponent<InkEngine.InkOptionButton>() != null)
            {
                buttonGO.GetComponent<InkEngine.InkOptionButton>().SetDialogueLine(choice.choiceText);
            }
            Button button = buttonGO.GetComponent<Button>();
            return button;
        }

        public void CloseCurrentDialogBox(bool clear = true)
        {
            if (clear)
            {
                CurrentDialogBoxSimple.ClearAllText();
                CurrentDialogBoxSimple.ClearAllOptions();
            }
            CurrentDialogBoxSimple.Active = false;
        }
        public void OpenCurrentDialogBox(bool clear = true)
        {
            if (clear)
            {
                CurrentDialogBoxSimple.ClearAllText();
                CurrentDialogBoxSimple.ClearAllOptions();
            }
            CurrentDialogBoxSimple.Active = true;
        }
        public void ChangeCurrentDialogBox(SimpleInkDialogBox newBox, bool closeOld = true, bool openNew = true)
        {
            if (closeOld)
            {
                CloseCurrentDialogBox();
            }
            CurrentDialogBoxSimple = newBox;
            if (openNew)
            {
                OpenCurrentDialogBox();
            }
        }
        public void ChangeCurrentDialogBox(SimpleInkDialogBox newBox)
        {
            ChangeCurrentDialogBox(newBox, true, true);
        }
    }
}