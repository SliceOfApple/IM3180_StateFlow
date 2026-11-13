using System;
using UnityEngine;
using static Utility;

[CreateAssetMenu(fileName = "Combo", menuName = "Scriptable Objects/ComboCreatorScriptableObject")]
public class ComboCreatorScriptableObject : ScriptableObject
{
    public string comboName;
    public ArrowDirection[] arrowSequence = new ArrowDirection[4];
    public virtual void ComboFunction() { Debug.Log("asdfghjkl"); }
}
