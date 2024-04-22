using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using WS.Script.GameManagers;
using Zenject;

namespace WS.Script.UI
{
    public class SaveMenu : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        [Inject] private GameController _gameManager;
        [FormerlySerializedAs("timerTxt")] [SerializeField] private Text _timerText;
        [FormerlySerializedAs("coinCostTxt")] [SerializeField] private Text _coinsText;
        [FormerlySerializedAs("timerImage")] [SerializeField] private Image _timerImage;
        [FormerlySerializedAs("btnSaveByCoin")] [SerializeField] private Button _saveByCoins;
        
        [Header("SAVE BY COINS")]
        private int _price = 10;
        private float timeToThink = 3;
        private float timePassed;
        private bool _isCounting = true;

        private void OnEnable()
        {
            _saveByCoins.interactable = ValueStorage.CoinsData >= _price;
        
            timePassed = timeToThink;
            _coinsText.text = _price.ToString();
        }

        public bool CheckSave()
        {
            return ValueStorage.CoinsData >= _price;
        }

        private void Update()
        {
            if (!_isCounting)
                return;

            timePassed -= 1f / 60f;
            _timerText.text = Mathf.RoundToInt(timePassed) + "";
            if(_timerImage)
                _timerImage.fillAmount = Mathf.Clamp01(timePassed / timeToThink);

            if (timePassed <= 0)
            {
                DeactivateMenu();
            }
        }

        public void DeactivateMenu()
        {
            _soundManager.Click();
            Time.timeScale = 1;
            _gameManager.Fail();
            gameObject.SetActive(false);
        }

        public void PayCoins()
        {
            _isCounting = false;
            _soundManager.Click();
            ValueStorage.CoinsData -= _price;
            MoveOn();
        }
        
        private void MoveOn()
        {
            Time.timeScale = 1;
            _gameManager.Next();
            gameObject.SetActive(false);
        }
    }
}
