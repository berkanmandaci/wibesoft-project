using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WibeSoft.UI.Views.HudViews
{
    public class LevelContainerView : MonoBehaviour
    {
        [SerializeField] private Slider _levelSlider;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _nameText;
        
        
        public void Init(int maxExp, int currentExp, int level, string username)
        {
            _nameText.text = username;
            float normalizedExp = (float)currentExp / maxExp;
            _levelSlider.value = normalizedExp;
            _levelText.text = level.ToString();
        }
    }
}
