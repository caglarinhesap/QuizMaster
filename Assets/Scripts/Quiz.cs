using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();
    QuestionSO currentQuestion;

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;
    private int correctAnswerIndex;
    bool hasAnsweredEarly = true;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite selectedAnswerSprite;
    [SerializeField] Sprite wrongAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;

    [Header("Timer")]
    [SerializeField] float showAnswerDelay = 1.5f;
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("ProgressBar")]
    [SerializeField] Slider progressBar;

    public bool isComplete;

    void Awake()
    {
        timer = FindObjectOfType<Timer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
    }

    private void Update()
    {
        timerImage.fillAmount = timer.fillFraction;
        if (timer.loadNextQuestion)
        {
            if (progressBar.value == progressBar.maxValue)
            {
                isComplete = true;
                return;
            }

            hasAnsweredEarly = false;
            GetNextQuestion();
            timer.loadNextQuestion = false;
        }
        else if (!hasAnsweredEarly && !timer.isAnsweringQuestion)
        {
            DisplayAnswer(-1);
            DisableButtons();
        }
    }

    private void GetNextQuestion()
    {
        if (questions.Count > 0)
        {
            ResetButtonStates();
            GetRandomQuestion();
            DisplayQuestion();
            progressBar.value++;
            scoreKeeper.IncrementQuestionsSeen();
        }
    }

    private void DisplayQuestion()
    {
        questionText.text = currentQuestion.GetQuestion();
        correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.GetAnswer(i);
        }
    }

    void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];
        questions.Remove(currentQuestion);
    }

    public void OnAnswerSelected(int index)
    {
        hasAnsweredEarly = true;
        answerButtons[index].GetComponent<Image>().sprite = selectedAnswerSprite;
        DisableButtons();
        StartCoroutine(ShowAnswerAfterDelay(index));
        //DisplayAnswer(index);
        timer.CancelTimer();
    }

    IEnumerator ShowAnswerAfterDelay(int index)
    {
        yield return new WaitForSeconds(showAnswerDelay);

        if (index == currentQuestion.GetCorrectAnswerIndex())
        {
            questionText.text = "Correct!";
            scoreKeeper.IncrementCorrectAnswers();
        }
        else if (index == -1)
        {
            questionText.text = "Time Up! The correct answer was:\n" + currentQuestion.GetAnswer(correctAnswerIndex);
        }
        else
        {
            questionText.text = "Incorrect! The correct answer was:\n" + currentQuestion.GetAnswer(correctAnswerIndex);
            answerButtons[index].GetComponent<Image>().sprite = wrongAnswerSprite;
        }
        answerButtons[correctAnswerIndex].GetComponent<Image>().sprite = correctAnswerSprite;
        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";
    }

    void DisplayAnswer(int index)
    {
        if (index == currentQuestion.GetCorrectAnswerIndex())
        {
            questionText.text = "Correct!";
            scoreKeeper.IncrementCorrectAnswers();
        }
        else if (index == -1)
        {
            questionText.text = "Time Up! The correct answer was:\n" + currentQuestion.GetAnswer(correctAnswerIndex);
        }
        else
        {
            questionText.text = "Incorrect! The correct answer was:\n" + currentQuestion.GetAnswer(correctAnswerIndex);
            answerButtons[index].GetComponent<Image>().sprite = wrongAnswerSprite;
        }
        answerButtons[correctAnswerIndex].GetComponent<Image>().sprite = correctAnswerSprite;
        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";
    }
    void ResetButtonStates()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponent<Button>().interactable = true;
            answerButtons[i].GetComponent<Image>().sprite = defaultAnswerSprite;
        }
    }

    void DisableButtons()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<Button>().interactable = false;
        }
    }
}
