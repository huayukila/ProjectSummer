using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbstractBind : MonoBehaviour, IBind
{
    [HideInInspector] [SerializeField] private string m_ComponentName;

    public virtual string TypeName
    {
        get
        {
            if (string.IsNullOrEmpty(m_ComponentName))
            {
                m_ComponentName = GetComponentName();
            }

            return m_ComponentName;
        }
    }

    public Transform Transform => transform;

    public string GetComponentName()
    {
        if (GetComponent<ViewController>()) return GetComponent<ViewController>().GetType().FullName;

        if (GetComponent<ScrollRect>()) return "UnityEngine.UI.ScrollRect";
        if (GetComponent<InputField>()) return "UnityEngine.UI.InputField";

        if (GetComponent<TextMeshProUGUI>()) return "TMPro.TextMeshProUGUI";
        if (GetComponent<TextMeshPro>()) return "TMPro.TextMeshPro";
        if (GetComponent<TMP_InputField>()) return "TMPro.TMP_InputField";

        if (GetComponent<Dropdown>()) return "UnityEngine.UI.Dropdown";
        if (GetComponent<Button>()) return "UnityEngine.UI.Button";
        if (GetComponent<Text>()) return "UnityEngine.UI.Text";
        if (GetComponent<RawImage>()) return "UnityEngine.UI.RawImage";
        if (GetComponent<Toggle>()) return "UnityEngine.UI.Toggle";
        if (GetComponent<Slider>()) return "UnityEngine.UI.Slider";
        if (GetComponent<Scrollbar>()) return "UnityEngine.UI.Scrollbar";
        if (GetComponent<Image>()) return "UnityEngine.UI.Image";
        if (GetComponent<ToggleGroup>()) return "UnityEngine.UI.ToggleGroup";

        return "Transform";
    }
}