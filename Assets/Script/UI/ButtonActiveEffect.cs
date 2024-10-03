using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonActiveEffect : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.5f, 10, 1);
    }
}
