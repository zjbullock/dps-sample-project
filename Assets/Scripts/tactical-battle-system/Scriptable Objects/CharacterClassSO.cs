using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character_Class_", menuName = "ScriptableObjects/Character Class")]
public class CharacterClassSO : ScriptableObject
{
    [SerializeField]
    private string className;


    public string ClassName { get => this.className;  }
}
