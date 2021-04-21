using UnityEngine;
using UnityEngine.UI;

public class GUIManager : Singleton<GUIManager>
{
    [SerializeField] private Text scoreTxt;
    
    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            scoreTxt.text = _score.ToString();
        }
    }
}
