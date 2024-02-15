using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ComboScript : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> comboSprites; // 콤보 숫자 모음


    public void SetComboImage(int _index)
    {
        GetComponent<Image>().sprite = comboSprites[_index];
    }
}
