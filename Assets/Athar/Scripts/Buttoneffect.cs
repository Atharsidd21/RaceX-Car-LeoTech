//using UnityEngine;
//using DG.Tweening;
//using UnityEngine.EventSystems;
//using Unity.VisualScripting;

//public class Buttoneffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
//{
//    public Transform ButtonTransform;
//    private Vector3 OriginalScale;
//    void Awake()
//    {
//        OriginalScale = ButtonTransform.localScale;
//    }
//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        ButtonTransform.DOKill();
//        ButtonTransform.DOScale(OriginalScale * 1.1f, .2f).SetEase(Ease.OutBack);
//    }
//    public void OnPointerExit(PointerEventData eventData)
//    {
//        ButtonTransform.DOKill();
//        ButtonTransform.DOScale(OriginalScale, .2f).SetEase(Ease.InOutBack);
//    }

//}
