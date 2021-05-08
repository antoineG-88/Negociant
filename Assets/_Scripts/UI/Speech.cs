using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Speech
{
    [TextArea(3, 8)] public string speechRawText;
    [HideInInspector] public List<Sentence> sentences;
    [HideInInspector] public float speakingSpeed;
    [HideInInspector] public float sentencePartPauseTime;
    [HideInInspector] public float sentencePauseToNext;

    [HideInInspector] public bool isFinished;

    private int currentSentenceIndex;
    private int currentSentencePartIndex;
    private int currentCharacterIndex;
    private string speechProgression;
    private float charProgression;
    private float waitTimeRmn;
    private string savedSentencePart;
    private bool finishing;
    private int numberOfPauses;
    private int numberOfSentences;
    private int numberOfValidCharacter;

    public Speech(string speechText)
    {
        speechRawText = speechText;
    }

    public void InitSpeechStructure(Character character)
    {
        speakingSpeed = character.speechBaseSpeed;
        sentencePauseToNext = character.speechPauseTimeBetweenSentences;
        sentencePartPauseTime = character.speechPauseTimeBetweenSentenceParts;
        int sentenceIndex = 0;
        int sentencePartIndex = 0;
        numberOfValidCharacter = 0;
        numberOfPauses = 0;
        numberOfSentences = 1;

        sentences = new List<Sentence>();

        for (int i = 0; i < speechRawText.Length; i++)
        {
            if(sentenceIndex >= sentences.Count)
            {
                Sentence newSentence = new Sentence();
                newSentence.sentencePart = new List<string>();
                sentences.Add(newSentence);
            }

            if(sentencePartIndex >= sentences[sentenceIndex].sentencePart.Count)
            {
                sentences[sentenceIndex].sentencePart.Add(string.Empty);
            }

            if(speechRawText[i] == '_')
            {
                sentencePartIndex++;
                numberOfPauses++;
            }
            else if(speechRawText[i] == '&')
            {
                sentenceIndex++;
                numberOfSentences++;
                sentencePartIndex = 0;
            }
            else
            {
                sentences[sentenceIndex].sentencePart[sentencePartIndex] += speechRawText[i];
                numberOfValidCharacter++;
            }
        }
    }

    public void InitSpeechStructure(float pauseTime, float timeBetweenSentence, float speechTime)
    {
        sentencePauseToNext = timeBetweenSentence;
        sentencePartPauseTime = pauseTime;
        int sentenceIndex = 0;
        int sentencePartIndex = 0;
        numberOfValidCharacter = 0;
        numberOfPauses = 0;
        numberOfSentences = 1;

        sentences = new List<Sentence>();

        for (int i = 0; i < speechRawText.Length; i++)
        {
            if (sentenceIndex >= sentences.Count)
            {
                Sentence newSentence = new Sentence();
                newSentence.sentencePart = new List<string>();
                sentences.Add(newSentence);
            }

            if (sentencePartIndex >= sentences[sentenceIndex].sentencePart.Count)
            {
                sentences[sentenceIndex].sentencePart.Add(string.Empty);
            }

            if (speechRawText[i] == '_')
            {
                sentencePartIndex++;
                numberOfPauses++;
            }
            else if (speechRawText[i] == '&')
            {
                sentenceIndex++;
                numberOfSentences++;
                sentencePartIndex = 0;
            }
            else
            {
                sentences[sentenceIndex].sentencePart[sentencePartIndex] += speechRawText[i];
                numberOfValidCharacter++;
            }
        }

        speakingSpeed = numberOfValidCharacter / Mathf.Max(speechTime - (numberOfPauses * pauseTime) - (numberOfSentences * timeBetweenSentence), 0.0001f);
    }

    [System.Serializable]
    public class Sentence
    {
        public List<string> sentencePart;

        public Sentence()
        {

        }
    }

    public void ResetProgression()
    {
        isFinished = false;
        finishing = false;
        currentSentenceIndex = 0;
        currentSentencePartIndex = 0;
        currentCharacterIndex = 0;
        speechProgression = string.Empty;
        savedSentencePart = string.Empty;
        waitTimeRmn = 0;
        charProgression = 0;
    }

    public string GetCurrentSpeechProgression(float deltaTime)
    {
        if(waitTimeRmn > 0)
        {
            waitTimeRmn -= deltaTime;
        }
        else
        {
            if(finishing)
            {
                isFinished = true;
            }
            else
            {
                charProgression += speakingSpeed * deltaTime;
                speechProgression = string.Empty;
                for (int i = 0; i <= currentSentencePartIndex; i++)
                {
                    if (i < currentSentencePartIndex)
                    {
                        speechProgression += sentences[currentSentenceIndex].sentencePart[i];
                    }
                    else if (i == currentSentencePartIndex)
                    {
                        while (charProgression >= 1 && currentCharacterIndex < sentences[currentSentenceIndex].sentencePart[i].Length)
                        {
                            savedSentencePart += sentences[currentSentenceIndex].sentencePart[i][currentCharacterIndex];
                            currentCharacterIndex++;
                            charProgression--;
                        }

                        if (currentCharacterIndex >= sentences[currentSentenceIndex].sentencePart[i].Length)
                        {
                            speechProgression += savedSentencePart;
                            waitTimeRmn = sentencePartPauseTime;
                            currentSentencePartIndex++;
                            currentCharacterIndex = 0;
                            savedSentencePart = string.Empty;
                            if (currentSentencePartIndex >= sentences[currentSentenceIndex].sentencePart.Count)
                            {
                                waitTimeRmn = sentencePauseToNext;
                                currentSentenceIndex++;
                                currentSentencePartIndex = 0;
                                if (currentSentenceIndex >= sentences.Count)
                                {
                                    finishing = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                speechProgression += savedSentencePart;
            }

        }

        return speechProgression;
    }
}
