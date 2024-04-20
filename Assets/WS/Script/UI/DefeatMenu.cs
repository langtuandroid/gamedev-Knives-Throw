using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using WS.Script.GameManagers;
using Zenject;

namespace WS.Script.UI
{
    public class DefeatMenu : MonoBehaviour
    {
        [Inject] private GameController _gameManager;
        [FormerlySerializedAs("bestTxt")] [SerializeField] private Text _bestText;
        [FormerlySerializedAs("scoreTxt")] [SerializeField] private Text _scoreText;

        private void Start()
        {
            _bestText.text = "BEST: " + ValueStorage.BestResult;
            _scoreText.text = _gameManager.GameScore.ToString();
        }
    }
}
