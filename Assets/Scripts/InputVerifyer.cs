﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

[System.Serializable]
public class InputVerifyer : MonoBehaviour {

    private InputField mainInput;
    private Button fakeInputFieldButton;
    private Text fakeInputFieldButtonText;
    private string prevInput = null;
    private string[] requiredFunctions;
    private int prevPos = 0;

    // Use this for initialization
    void Start()
    {
        var fakeInputFieldButtonGO = GameObject.Find("FakeInputFieldButton");
        fakeInputFieldButton = fakeInputFieldButtonGO.GetComponent<Button>();
        fakeInputFieldButtonText = fakeInputFieldButton.GetComponentInChildren<Text>();
        var mainInputGO = GameObject.Find("InputField");
        mainInput = mainInputGO.GetComponent<InputField>();
        fakeInputFieldButtonText.text = mainInput.text.Replace(requiredFunctions[0], "<color=#E12F0BFF>" + requiredFunctions[0] + "</color>");
    }

    // Update is called once per frame
    
   void Update()
   {
        /*
        Debug.Log(mainInput.caretPosition);
        correctCaretPosition(mainInput);
        
        prevPos = mainInput.caretPosition;
        */
    }

    public void activeInputField()
    {
        EventSystem.current.SetSelectedGameObject(mainInput.gameObject, null);
        mainInput.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private void correctCaretPosition(InputField input)
    {
        Debug.Log("open " + isInsideOfOpenTag(input.text, input.caretPosition));

        //We assume that player wants to edit text before unchangeble string
        if (isInsideOfOpenTag(input.text, input.caretPosition) && 
            input.caretPosition - prevPos > 1)
        {
            moveToClosestBeginningOfTag(input);
        }

        Debug.Log("close " + isInsideOfCloseTag(input.text, input.caretPosition));

        //Same, except after unchangable string
        if (isInsideOfCloseTag(input.text, input.caretPosition) &&
            input.caretPosition - prevPos > 1)
        {
            moveToClosestEndOfTag(input);
        }

        //1 symbol difference most probably means pressing arrow buttons,
        //so player moves from left to right
        if (isInsideOfOpenTag(input.text, input.caretPosition) &&
            input.caretPosition - prevPos == 1)
        {
            //moveToEnd(input);
        }

        if (isInsideOfCloseTag(input.text, input.caretPosition) &&
            input.caretPosition - prevPos == 1)
        {
            //moveToStart(input);
        }
    }

    private void moveToEnd(InputField input)
    {
        var text = input.text;
        int i = input.caretPosition;
        bool flag = false;

        while (text[i] != '>' && !flag && i < text.Length)
        {
            ++i;
            if (text[i] == '>')
            {
                flag = true;
            }
        }

        input.caretPosition = i + 1;
    }

    private void moveToStart(InputField input)
    {
        var text = input.text;
        int i = input.caretPosition;
        bool flag = false;

        while (text[i] != '<' && !flag && i > 0)
        {
            --i;
            if (text[i] == '<')
            {
                flag = true;
            }
        }
        input.caretPosition = i;
    }

    private bool isInsideTag(string text, int pos)
    {
        for (int i = pos; i < text.Length; ++i)
        {
            if (text[i] == '>')
            {
                return true;
            }
            else if (text[i] == '<')
            {
                return false;
            }
        }
        return false;
    }

    private void moveToClosestEndOfTag(InputField input)
    {
        var text = input.text;
        int i = input.caretPosition;

        while (text[i] != '>' && i < text.Length - 1) ++i;

        input.caretPosition = i + 1 < text.Length ? i + 1 : text.Length;
        input.selectionFocusPosition = i + 1 < text.Length ? i + 1 : text.Length;
    }

    private void moveToClosestBeginningOfTag(InputField input)
    {
        var text = input.text;
        int i = input.caretPosition;

        while (text[i] != '<' && i >= 0) --i;

        input.caretPosition = i > 0 ? i : 0;
        input.selectionFocusPosition = i > 0 ? i : 0;
    }

    private bool isInsideOfOpenTag(string text, int pos)
    {
        if (pos == text.Length) --pos;

        for (int i = pos; i >= 0; --i)
        {
            if (text[i] == '<' && text[i + 1] != '/')
            {
                return true;

            } else if (text[i] == '<')
            {
                return false;
            }
        }
        return false;
    }

    private bool isInsideOfCloseTag(string text, int pos)
    {
        if (pos == text.Length) --pos;
        bool flag = false;

        for (int i = pos; i >= 0; --i)
        {
            if (text[i] == '<' && text[i + 1] == '/')
            {
                return true;
            }
            if (text[i] == '>' && flag)
            {
                return false;
            }
        }
        return false;
    }

    public void setPrevInput(string input)
    {
        if (prevInput == null)
        {
            prevInput = input;
        }
    }

    public void setReqiredFunctions(string[] functions)
    {
        requiredFunctions = functions;
    }

    public void verifyInput()
    {
        if (mainInput != null)
        {
            var text = mainInput.text;

            bool isValid = true;

            foreach (string function in requiredFunctions)
            {
                if (text.IndexOf(function) == -1)
                {
                    isValid = false;
                }
            }

            string clearString = (string) text.Clone();

            foreach (string function in requiredFunctions)
            {
                clearString = clearString.Replace(function, string.Empty);
            }

            clearString = Regex.Replace(clearString, @"[\d-]", string.Empty)
                .Replace("+", string.Empty)
                .Replace("-", string.Empty)
                .Replace("*", string.Empty)
                .Replace("/", string.Empty)
                .Replace("#", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace(".", string.Empty);

            if (clearString.Length != 0)
            {
                isValid = false;
            }

            if (isValid)
            {
                prevInput = text;
            }

            mainInput.text = prevInput;
            fakeInputFieldButtonText.text = prevInput.Replace(requiredFunctions[0], "<color=#E12F0BFF>" + requiredFunctions[0] + "</color>");
        }
    }
}