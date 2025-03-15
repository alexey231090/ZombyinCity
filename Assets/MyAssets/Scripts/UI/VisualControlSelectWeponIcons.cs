using UnityEngine;
using UnityEngine.UI;

namespace FpsZomby
{

    public class VisualControlSelectWeponIcons : MonoBehaviour
    {
        public PlayerSwitchingStates.Weapons PlayeSelectWepon;
        [SerializeField] private GameObject[] WeponsIcon;
        [SerializeField] private GameObject[] selectIcons;
        PlayerSwitchingStates switchingStates;

        private void Start()
        {
            switchingStates = GameObject.FindObjectOfType<PlayerSwitchingStates>();
        }

        void Update()
        {
            PlayeSelectWepon = PlayerSwitchingStates.weapon;
            UpdateWeaponIcons();
        }

        //Обновление иконок оружия
        private void UpdateWeaponIcons()
        {
            for (int i = 0; i < selectIcons.Length; i++)
            {
                selectIcons[i].SetActive(false);
                SetIconTransparency(WeponsIcon[i], 150);
            }

            switch (PlayeSelectWepon)
            {
                case PlayerSwitchingStates.Weapons.Crowbar:
                    if (WeponsIcon[0].activeSelf)
                    {
                        selectIcons[0].SetActive(true);
                    }
                    else
                    {
                        WeponsIcon[0].SetActive(true);
                    }
                    SetIconTransparency(WeponsIcon[0], 255);
                    break;
                case PlayerSwitchingStates.Weapons.Gun:
                    if (WeponsIcon[1].activeSelf)
                    {
                        selectIcons[1].SetActive(true);
                    }
                    else
                    {
                        WeponsIcon[1].SetActive(true);
                    }
                    SetIconTransparency(WeponsIcon[1], 255);
                    break;
                case PlayerSwitchingStates.Weapons.Bennelli_M4:
                    if (WeponsIcon[2].activeSelf)
                    {
                        selectIcons[2].SetActive(true);
                    }
                    else
                    {
                        WeponsIcon[2].SetActive(true);
                    }
                    SetIconTransparency(WeponsIcon[2], 255);
                    break;
                case PlayerSwitchingStates.Weapons.AK74:
                    if (WeponsIcon[3].activeSelf)
                    {
                        selectIcons[3].SetActive(true);
                    }
                    else
                    {
                        WeponsIcon[3].SetActive(true);
                    }
                    SetIconTransparency(WeponsIcon[3], 255);
                    break;
            }
        }

        //Установка прозрачности иконки
        private void SetIconTransparency(GameObject icon, int alpha)
        {
            Image image = icon.GetComponent<Image>();
            if (image != null)
            {
                Color color = image.color;
                color.a = alpha / 255f;
                image.color = color;
            }
        }
    }
}
