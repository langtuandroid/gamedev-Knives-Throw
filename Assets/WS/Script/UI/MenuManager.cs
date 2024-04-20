using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WS.Script.GameManagers;
using WS.Script.Other;
using WS.Script.Target;
using Zenject;

namespace WS.Script.UI
{
    public class MenuManager : MonoBehaviour, IObserver
    {
        [Inject] private TargetHandler _targetManager;
        [Inject] private SoundManager _soundManager;
        [Inject] private GameController _gameManager;
   
        [SerializeField] private GameObject StartUI;
        [SerializeField] private GameObject UI;
        [SerializeField] private GameObject GameoverUI;
        [SerializeField] private GameObject LoadingUI;
        [SerializeField] private GameObject GamePauseUI;
        [SerializeField] private GameObject AskForLifeUI;
        [SerializeField] private GameObject ShopUI;
        [SerializeField] private GameObject SettingUI;

        [Space]
        [SerializeField] private Text txtStage;
        [SerializeField] private Text txtBossFight;
        [SerializeField] private Text txtScore;
        [SerializeField] private Image[] levelDots;
        [SerializeField] private Text soundStatus;
        
        [Header("LOADING PROGRESS")]
        [SerializeField] private Slider slider;
        [SerializeField] private Text progressText;
        
        private void Start()
        {
            StartUI.SetActive(true);
            UI.SetActive(false);
            GameoverUI.SetActive(false);
            LoadingUI.SetActive(false);
            GamePauseUI.SetActive(false);
            AskForLifeUI.SetActive(false);
            ShopUI.SetActive(false);
            SettingUI.SetActive(false);

            if (ValueStorage.IsAutoPlay)
            {
                ValueStorage.IsAutoPlay = false;
                Play();
            }
        }

        private void Update()
        {
            if (_targetManager._currentTarget)
            {
                txtStage.text = "Stage: " + (_targetManager._stage + 1);

                if (_targetManager._currentTarget._stage != STAGE_TYPE.BossFight)
                {
                    for(int i = 0; i < levelDots.Length; i++)
                    {
                        levelDots[i].gameObject.SetActive(true);
                        levelDots[i].color = i == (_targetManager._stageToBoss-1) ? Color.yellow : Color.white;
                        levelDots[i].transform.localScale = i == (_targetManager._stageToBoss-1) ? Vector3.one * 1.2f : Vector3.one;
                    }
                }
                else
                {
                    for (int i = 0; i < levelDots.Length; i++)
                    {
                        levelDots[i].color = i == (levelDots.Length-1 )? Color.yellow : Color.white;
                        levelDots[i].transform.localScale = i == (levelDots.Length-1) ? Vector3.one * 1.5f : Vector3.one;
                        levelDots[i].gameObject.SetActive(i == (levelDots.Length - 1));
                    }
                }
            }

            soundStatus.text = ValueStorage.IsSound ? "Sound: ON" : "Sound: OFF";
            AudioListener.volume = ValueStorage.IsSound ? 1 : 0;
            txtScore.text = _gameManager.GameScore.ToString();
        }

        public void SwitchSound()
        {
            _soundManager.Click();
            ValueStorage.IsSound = !ValueStorage.IsSound;
        
        }

        public void Play()
        {
            _soundManager.Click();
            StartUI.SetActive(false);
            UI.SetActive(true);
            _gameManager.BeginGame();
        }

        public void Restart()
        {
            ValueStorage.IsAutoPlay = true;
            Time.timeScale = 1;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        public void IPlay()
        {
            StartUI.SetActive(false);
            UI.SetActive(true);
        }

        public void ISuccess()
        {
        
        }

        public void IPause()
        {
            throw new System.NotImplementedException();
        }

        public void IUnPause()
        {
            throw new System.NotImplementedException();
        }

        public void IGameOver()
        {
            StartCoroutine(GameOverCo(0));
        }
        
        public void GoHome()
        {
            _soundManager.Click();
            Time.timeScale = 1;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        public void OpenShop(bool open)
        {
            _soundManager.Click();
            ShopUI.SetActive(open);
        }

        public void OpenSettings(bool open)
        {
            _soundManager.Click();
            SettingUI.SetActive(open);
        }

        public void OpenLeaderboard()
        {
            var leaderboard = GameObject.Find("LEADERBOARD");
            if (leaderboard != null)
                leaderboard.SendMessageUpwards("OpenLeaderboard", true);
            else
                Debug.LogError("No leaderboard setup, please read the Tutorial file to know more");
        }


        private IEnumerator LoadAsynchronously(string name)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(name);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                if (slider != null)
                    slider.value = progress;
                if (progressText != null)
                    progressText.text = (int)progress * 100f + "%";
                yield return null;
            }
        }

        public void HomeScene()
        {
            _soundManager.Click();
            Time.timeScale = 1;
            LoadingUI.SetActive(true);
            StartCoroutine(LoadAsynchronously("MainMenu"));

        }

        public void Pause()
        {
            _soundManager.Click();
            if (Time.timeScale == 0)
            {
                GamePauseUI.SetActive(false);
                Time.timeScale = 1;
                _soundManager.PauseMusic(false);
            }
            else
            {
                GamePauseUI.SetActive(true);
                Time.timeScale = 0;
                _soundManager.PauseMusic(true);
            }
        }


        private IEnumerator GameOverCo(float time)
        {
            UI.SetActive(false);

            yield return new WaitForSeconds(time);
            
            _soundManager.PlaySfx(_soundManager.soundGameover, 0.8f);
            GameoverUI.SetActive(true);
        }

        public void ShowAskForLife()
        {
            StartCoroutine(AskForLifeCo(0.5f));
        }

        public void ShowUI(bool open)
        {
            UI.SetActive(open);
        }

        private IEnumerator AskForLifeCo(float time)
        {
            if (AskForLifeUI.GetComponent<SaveMenu>().CheckSave())
            {
                yield return new WaitForSeconds(time);
                AskForLifeUI.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                yield return null;
                _gameManager.Fail();
            }
        }
    }
}
