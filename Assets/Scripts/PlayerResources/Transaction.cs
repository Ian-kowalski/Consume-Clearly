using UnityEngine;

namespace PlayerResources
{
    public class Transaction : MonoBehaviour
    {
        [SerializeField] public ResourceManager GameManagerObject;
        [SerializeField] public int TransactionValue;
        [SerializeField] public float RSValue;
        [SerializeField] public float MSValue;

        public void ChangeMoneyValue(int amount)
        {
            amount = TransactionValue;
            GameManagerObject.UpdateMoney(amount);
        }
        
        public void ChangeRSValue(float amount)
        {
            GameManagerObject.UpdateRS(amount);
        }
        
        public void ChangeMSValue(float amount)
        {
            GameManagerObject.UpdateMS(amount);
        }
    }
}
