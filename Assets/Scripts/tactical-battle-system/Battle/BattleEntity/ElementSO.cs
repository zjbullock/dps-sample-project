using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DPS.TacticalCombat{
[CreateAssetMenu(fileName = "Element_", menuName = "ScriptableObjects/Element")]
[Serializable]
public class ElementSO : ScriptableObject
{
    [SerializeField]
    private string elementName;

    public string ElementName { get => this.elementName; }

    [SerializeField]
    private Sprite elementSprite;

    public Sprite ElementSprite { get => this.elementSprite; }
}
}