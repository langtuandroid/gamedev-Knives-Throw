using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using WS.Script.GameManagers;
using WS.Script.Weapon;
using Zenject;

namespace WS.Script.UI
{
    public class WeaponBuy : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        
        [FormerlySerializedAs("weapon")] [SerializeField] private Weapon.Weapon _weapon;
        [FormerlySerializedAs("itemIcon")] [SerializeField] private Image _icon;
        [FormerlySerializedAs("coinHolder")] [SerializeField] private GameObject _coinHolder;
        [FormerlySerializedAs("unlockedHolder")] [SerializeField] private GameObject _unlockHolder;
        [FormerlySerializedAs("pickedIcon")] [SerializeField] private Image _pickedIcon;
        [FormerlySerializedAs("price")]
        [Header("COIN")]
        [SerializeField] private Text _priceText;

        private void OnEnable()
        {
            _icon.sprite = _weapon.weaponRenderer.sprite;
            if (_weapon.IsUnlocked)
            {
                _coinHolder.SetActive(false);
                _unlockHolder.SetActive(true);
            }
            else
            {
                _coinHolder.SetActive(true);
                _unlockHolder.SetActive(false);

                _icon.color = Color.black;
                _priceText.text = _weapon.Price + "";
            }
        }


        private void Update()
        {
            _pickedIcon.color = ValueStorage.WeaponEquipped == _weapon.gameObject.GetInstanceID() ? Color.white : Color.black;
        }

        public void Buy()
        {
            if (ValueStorage.CoinsData <= _weapon.Price) return;
            ValueStorage.CoinsData -= _weapon.Price;
            _soundManager.PlaySfx(_soundManager.soundPurchasedItem);
            _weapon.IsUnlocked = true;
            _icon.color = Color.white;
            _coinHolder.SetActive(false);
            _unlockHolder.SetActive(true);
        }

        
        public void Equip()
        {
            ValueStorage.WeaponEquipped = _weapon.gameObject.GetInstanceID();
            _soundManager.PlaySfx(_soundManager.soundPickItem);
        }
    }
}
