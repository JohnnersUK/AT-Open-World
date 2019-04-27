using UnityEngine;

[ExecuteInEditMode]
public class ResourceName : MonoBehaviour
{
    public string globalName;
    void Awake()
    {
        name = globalName;
    }
}
