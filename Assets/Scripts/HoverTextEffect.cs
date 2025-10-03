   using UnityEngine;
   using TMPro;
   using UnityEngine.EventSystems;

   public class HoverTextEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
   {
       private TMP_Text textMeshPro;
       private string originalText;

       private void Start()
       {
           textMeshPro = GetComponent<TMP_Text>();
           originalText = textMeshPro.text;
       }

       public void OnPointerEnter(PointerEventData eventData)
       {
           textMeshPro.text = $">{originalText}";
       }

       public void OnPointerExit(PointerEventData eventData)
       {
           textMeshPro.text = originalText;
       }
   }
