using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Assertions.Comparers;

namespace UGUIExtension
{
    public class RotatePanel<T> : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : UIBehaviour
    {
       
        // Item个数
        [Range(2, 20)]
        public int num;

        // 长轴
        [Range(0, 300)]
        public float longAxis;

        // 短轴
        [Range(0, 200)]
        public float shortAxis;

        // 衰减系数
        [Range(0, 1)]
        public float falloff = 0.1f;

        public float translateFactor = 1000.0f;
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1.0f, 1.0f);

        private float prevDragTime;
        private bool towardRight;
        private bool isDraging;
        private float angle;

        public Vector2 distance = Vector2.one;
        public List<RectTransform> transforms = new List<RectTransform>();


        

        // Use this for initialization
        void Start()
        {
            angle = 0;
        }

        private float Degree
        {
            get { return 2*Mathf.PI/num; }
        }

        // Update is called once per frame
        void Update()
        {
            if (angle > 0)
            {
                float allTime = curve[curve.length - 1].time;
                float reflect = longAxis / shortAxis;

                float lerp = Time.deltaTime / allTime * 2 * Mathf.PI;

                lerp = angle > lerp ? lerp : angle;
                int sign = towardRight ? 1 : -1;
                float sin = Mathf.Sin(lerp * sign);
                float cos = Mathf.Cos(lerp * sign);
                foreach (var rect in transforms)
                {
                    Vector2 origin = rect.anchoredPosition;
                    rect.anchoredPosition = new Vector2(origin.x * cos - origin.y * reflect * sin,
                        origin.x / reflect * sin + origin.y * cos);
                }

                angle -= lerp;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            angle = 0;
            Debug.Log("OnBeginDrag");
            prevDragTime = Time.realtimeSinceStartup;
            isDraging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("OnDrag");
            float deltaTime = Time.realtimeSinceStartup - prevDragTime;

            float allTime = curve[curve.length - 1].time;
            int n = (int)(deltaTime / allTime);
            float time = deltaTime - n * allTime;
            float val = curve.Evaluate(time);
            float maxVal = curve.Evaluate(allTime);
            
            float lerp = time / allTime * 2 * Mathf.PI;
            float reflect = longAxis / shortAxis;

            towardRight = eventData.delta.x > 0;
            int sign = towardRight ? 1 : -1;
            float sin = Mathf.Sin(lerp * sign);
            float cos = Mathf.Cos(lerp * sign);
            foreach (var rect in transforms)
            {
                Vector2 origin = rect.anchoredPosition;
                rect.anchoredPosition = new Vector2(origin.x * cos - origin.y * reflect * sin , origin.x / reflect * sin + origin.y * cos);
            }
            prevDragTime = Time.realtimeSinceStartup;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDraging = false;
            int sign = transforms[0].anchoredPosition.x > 0 ? 1 : -1;
            angle = Vector2.Angle(new Vector2(0, -shortAxis), transforms[0].anchoredPosition);
            Debug.Log("OnEndDrag : " + angle);

            angle = Mathf.Deg2Rad * angle;

            int n = (int) (angle / Degree);
            angle = angle - n * Degree;
            if (angle >= Degree/2)
            {
                towardRight = true;
                angle = (Degree - angle) * sign;
            }
            else
            {
                towardRight = false;
                angle *= sign;
            }


            
        }


#if UNITY_EDITOR

        private void Adjust()
        {
            int middle = num >> 1;

           
            for (int i = 0; i < num; i++)
            {
                transforms[i].anchoredPosition = new Vector2(longAxis * Mathf.Sin(Degree * i), -shortAxis * Mathf.Cos(Degree * i));
                transforms[i].gameObject.SetActive(true);
                int delta = i <= middle ? i : num - i;

                // 子节点层级
                int slibling = Mathf.Abs(i - middle);
                transforms[i].SetSiblingIndex(slibling);
                transforms[i].localScale = Vector3.one*(1 - delta * falloff);
            }
        }

        protected override void OnValidate()
        {
            if(transforms.Count < num)
            {
                for (int i = 0; i < transforms.Count; i++)
                {
                    transforms[i].gameObject.SetActive(true);
                }

                for (int i = transforms.Count; i < num; i++)
                {
                    GameObject go = new GameObject("Node", typeof(T));
                    go.transform.SetParent(transform);
                    go.transform.SetAsLastSibling();
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;
                    go.layer = LayerMask.NameToLayer("UI");
                    transforms.Add(go.GetComponent<RectTransform>());
                }
            }
            else
            {
                for (int i = transforms.Count - 1; i >= num; i--)
                {
                    transforms[i].gameObject.SetActive(false);
                }
            }

            Adjust();
        }


        [ContextMenu("Clear Invalide Node")]
        protected void ClearInvalidate()
        {
            for (int i = transforms.Count - 1; i >= num; i--)
            {
                GameObject go = transforms[i].gameObject;
                transforms.RemoveAt(i);
                DestroyImmediate(go);
            }

            Adjust();
        }
#endif
    }


}


