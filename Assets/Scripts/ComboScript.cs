using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ComboScript : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> comboSprites; // �޺� ���� ����


    public void SetComboImage(int _index)
    {
        GetComponent<Image>().sprite = comboSprites[_index];
    }
}
