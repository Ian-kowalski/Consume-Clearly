using UnityEngine;

[CreateAssetMenu(fileName = "Item Object", menuName = "Scriptable Objects/itemObject")]
public class itemObject : ScriptableObject
{
    [field: SerializeField]
    public bool IsStackable { get; set; }

    public int ID => GetInstanceID();

    [field: SerializeField]
    public int MaxStackSize { get; set; } = 1;
    [field: SerializeField]
    string Name { get; set; }
    [field: SerializeField]
    [field: TextArea]
    public string Description { get; set; }
    [field: SerializeField]
    public Sprite ItemImage { get; set; }
}
