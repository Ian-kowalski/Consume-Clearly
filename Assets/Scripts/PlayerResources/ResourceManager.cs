using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerResources
{
    public class ResourceManager : MonoBehaviour
    {
        //This script uses 2 Images, 1 slider and 1 text mesh. 
        [Header("Money Amount")]
        [SerializeField] public int Money = 1;
        [SerializeField] public TMP_Text TextMoney; //Simple TextMesh
        
        [Header("Clamp Settings")]
        [SerializeField] public bool ClampRMS = true;
        
        [Header("Relational Satisfaction Settings")]
        [SerializeField] public Image SliderRS; //Yellow
        [SerializeField] public float RS; //Relational Satisfaction (yellow)
        [SerializeField] private int RSDecreaseInterval = 5; //Seconds it takes for RS to decrease.
        [SerializeField] public int RSDecreaseOverTimeAmount = 5; //Amount of RS that decreases after interval.
        
        [Header("Material Satisfaction Settings")]
        [SerializeField] public Image SliderMS; //Red
        [SerializeField] public float MS; //Material Satisfaction (red)
        [SerializeField] private int MSDecreaseInterval = 5; //Seconds it takes for MS to decrease.
        [SerializeField] public int MSDecreaseOverTimeAmount = 5; //Amount of RS that decreases after interval.
        
        [Header("Community Spirit Settings")]
        [SerializeField] public int CommunitySpirit = 0;
        [SerializeField] public Slider SliderCommunitySpirit;
        [SerializeField] private int communitySpiritChangeInterval = 5;
        [SerializeField] public int CommunitySpiritDecreaseOverTimeAmount = 5;
        [SerializeField] public int CommunitySpiritIncreaseOverTimeAmount = 5;
        
        [Header("Easing Settings")]
        [SerializeField] private float easeTime = 0.25f; //Time for slider anim to finish.
        [SerializeField] private bool cubic = true;
        [SerializeField] private bool smoothStep = false;
        [SerializeField] private bool superSoft = false;
        [SerializeField] private bool bounce  = false;

        private Coroutine rsAnimRoutine;
        private Coroutine msAnimRoutine;
        
        private const float STotalLimit = 200f; // "Satisfaction Total Limit" Total value of the satisfaction meter. E.g if RS is at 200 the meter is filled.
        private float RS_max; //"Relational Satisfaction Max" Relevant if Clamp = true. Subtract total satisfaction meter value with current MS so that RS and MS doesn't overlap.
        private float MS_max; //"Material Satisfaction Max" Relevant if Clamp = true. Subtract total satisfaction meter value with current RS so that RS and MS doesn't overlap.
        private float rms; //"Relational Material Satisfaction" RS+MS to calculate the total value filled in the satisfaction meter. Relevant if Clamp = false so that increasing RS subtract MS and vice versa.
        private float satisfaction; //Subtract MS from RS to determine if Community spirit should increase or decrease over time.
        private float CStimer = 0;
        private float RStimer = 0;
        private float MStimer = 0;
        void Update()
        {
            RStimer += Time.deltaTime;
            MStimer += Time.deltaTime;
            CStimer += Time.deltaTime;
            
            UpdateRMS();
            DecreaseRSOverTime();
            DecreaseMSOverTime();
            ChangeCsOverTime();
            
            SliderCommunitySpirit.GetComponent<Slider>().value = CommunitySpirit;
            CommunitySpirit = Mathf.Clamp(CommunitySpirit, (int)SliderCommunitySpirit.minValue, (int)SliderCommunitySpirit.maxValue);
            
            TextMoney.GetComponent<TMP_Text>().text = Money.ToString();
        }
        
        private void UpdateRMS()
        {
            RS_max = STotalLimit - MS;
            MS_max = STotalLimit - RS;
                    
            RS = Mathf.Clamp(RS, 0, RS_max);
            MS = Mathf.Clamp(MS, 0, MS_max); 
            
            float rsFillAmount = RS / STotalLimit;
            float msFillAmount = MS / STotalLimit;
            
            AnimateMeter(ref rsAnimRoutine, SliderRS, rsFillAmount);
            AnimateMeter(ref rsAnimRoutine, SliderMS, msFillAmount);
        }
        private void DecreaseRSOverTime()
        {
            if (RStimer >= RSDecreaseInterval)
            {
                RS -= RSDecreaseOverTimeAmount;
                UpdateTotalS();
                RStimer = 0;
            }
        }
        public void UpdateRS(float amount)
        {
            UpdateTotalS();
            if (RS < RS_max && ClampRMS)
                RS = Mathf.Min(RS + amount, RS_max);
                  
            if (ClampRMS == false)
            {
                RS = Mathf.Min(RS + amount, STotalLimit);
                if (rms == STotalLimit)
                {
                    MS -= amount;
                }
            }
            UpdateRMS();
        }
        private void DecreaseMSOverTime()
        {
            if (MStimer >= MSDecreaseInterval)
            {
                MS -= MSDecreaseOverTimeAmount;
                UpdateTotalS();
                MStimer = 0;
            }
        }
        public void UpdateMS(float amount)
        {
            UpdateTotalS();
            if (MS < MS_max && ClampRMS)
                MS = Mathf.Min(MS + amount, MS_max);

            if (ClampRMS == false)
            {
                MS = Mathf.Min(MS + amount, STotalLimit);
                if (rms == STotalLimit)
                {
                    RS -= amount;
                }
            }
            UpdateRMS();
        }

        private void UpdateTotalS()
        {
            rms = RS + MS;
            satisfaction = RS - MS;
        }
        
        private void ChangeCsOverTime()
        {
            //Below says "If satisfaction is between 0 to 89 or the time interval for Community Spirit value change (5 sec) hasn't passed, DO NOTHING."
            if (CStimer < communitySpiritChangeInterval) return;
            if (satisfaction >= 0 && satisfaction <= 90) return;
            //Below says "If satisfaction is 90 or more, increase the value. If it's the other option (-1 or less) decrease the value."
            CommunitySpirit += (satisfaction >= 90) ? CommunitySpiritIncreaseOverTimeAmount : -CommunitySpiritDecreaseOverTimeAmount;
            CStimer = 0;
        }
        
        public void UpdateCommunitySpirit(int amount)
        {
            CommunitySpirit += amount;
        }
        
        public void UpdateMoney(int amount)
        {
            Money += amount;
            UpdateMoneyText();
        }
        private void UpdateMoneyText()
        {
            TextMoney.text = Money.ToString();
        }


        private void AnimateMeter(ref Coroutine routine, Image image, float target)
        {
            if (routine != null)
                StopCoroutine(routine);
            routine = StartCoroutine(AnimateMeterRoutine(image, target));
        }

        private IEnumerator AnimateMeterRoutine(Image image, float target)
        {
            float start = image.fillAmount;
            float time = 0f;
            while (time < easeTime)
            {
                time += Time.deltaTime;
                float t = time / easeTime;
                
                //anim style/effect
                if (cubic)
                    t = 1f - Mathf.Pow(1 - t, 3f);
                else if (smoothStep)
                    t = t * t * (3f - 2f * t); 
                else if (superSoft)
                    t = 1f - Mathf.Pow(1 - t, 7f);
                else if (bounce)
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                
                image.fillAmount = Mathf.Lerp(start, target, t);
                yield return null;
            }
            image.fillAmount = target;
        }
    }
}
