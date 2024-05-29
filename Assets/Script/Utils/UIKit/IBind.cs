using UnityEngine;

public interface IBind
{
    string TypeName { get; }
    Transform transform { get; }
}
