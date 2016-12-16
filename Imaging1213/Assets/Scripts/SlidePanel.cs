using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace UGUIExtension
{
    public class SlidePanel<T> : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : UIBehaviour
    {

        public bool canDrag;
        // Item个数
        [Range(2, 20)]
        public int num;

        // 间距
        [Range(0, 300)]
        public float margin;

        // Node 长度
        [Range(0, 300)]
        public float width;

        // Node 宽度
        [Range(0, 300)]
        public float height;

        // 衰减系数
        [Range(0, 1)]
        public float falloff = 0.1f;

        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1.0f, 1.0f);

        private float prevDragTime;
        private bool towardRight;
        private bool isDraging;
        private float angle;

        public Vector2 distance = Vector2.one;
        public List<RectTransform> transforms = new List<RectTransform>();

        public Canvas canvas;



        private float Degree
        {
            get { return 2 * Mathf.PI / num; }
        }

        // Update is called once per frame
        void Update()
        {
           
        }

        #region Input Drag
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!canDrag)
            {
                return;
            }
            angle = 0;
            
            prevDragTime = Time.realtimeSinceStartup;
            isDraging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!canDrag)
            {
                return;
            }
            Debug.Log("OnDrag");
            float deltaTime = Time.realtimeSinceStartup - prevDragTime;
            prevDragTime = Time.realtimeSinceStartup;
            float first = transforms[0].anchoredPosition.x + transforms[0].rect.width * transforms[0].pivot.x;

            float last = transforms[transforms.Count - 1].anchoredPosition.x
               - transforms[transforms.Count - 1].rect.width * (1 - transforms[transforms.Count - 1].pivot.x);

            RectTransform rectTransform = GetComponent<RectTransform>();
            if (first > rectTransform.rect.xMax)
            {
                if (eventData.delta.x > 0)
                {
                    return;
                }
            }
            else if (last < rectTransform.rect.xMin)
            {
                if (eventData.delta.x < 0)
                {
                    return;
                }
            }

            int sign = eventData.delta.x > 0 ? 1 : -1;
            foreach (var rect in transforms)
            {
                rect.anchoredPosition += eventData.delta.x * Vector2.right;
            }

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!canDrag)
            {
                return;
            }
            isDraging = false;
            int sign = transforms[0].anchoredPosition.x > 0 ? 1 : -1;

        }

        #endregion



        #region  VR

        public void VrDrag(float x, int n)
        {

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            float left = canvasRect.anchoredPosition.x - canvasRect.rect.width * canvasRect.pivot.x;

            float right = canvasRect.anchoredPosition.x + canvasRect.rect.width * (1 - canvasRect.pivot.x);
            float first = transforms[0].anchoredPosition.x;//+ transforms[0].rect.width * transforms[0].pivot.x;

            float last = transforms[transforms.Count - 1].anchoredPosition.x;
              // - transforms[transforms.Count - 1].rect.width * (1 - transforms[transforms.Count - 1].pivot.x);

            if (x < 0)
            {
                if (last < left)
                {
                   return;
                }
                x = Mathf.Clamp(x, -300, 0);
            }
            else
            {
                if (first > right)
                {
                    return;
                }

                x = Mathf.Clamp(x, 0, 300);
            }

            StartCoroutine(SlideAct(x, n));
        }


        private IEnumerator SlideAct(float x, int n)
        {
            float unit = x / n;
            for (int i = 0; i < n; i++)
            {
                transform.Translate(unit, 0, 0, Space.World);
                yield return new WaitForEndOfFrame();
            }

            var rect = transform.GetComponent<RectTransform>();

            float xAxis = Mathf.Clamp(rect.anchoredPosition.x, -600, 700);
            rect.anchoredPosition = new Vector2(xAxis, rect.anchoredPosition.y);
        }

        #endregion

#if UNITY_EDITOR

        private void Adjust()
        {
            
            int middle = num >> 1;

            float offset = (num & 0x1) > 0 ? 0 : 0.5f;
            for (int i = 0; i < num; i++)
            {
                float delta = i - middle + offset;
                transforms[i].gameObject.SetActive(true);
                transforms[i].anchoredPosition = delta * margin * Vector2.right;
                transforms[i].localScale = Vector3.one;
                transforms[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                transforms[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                
            }

  
        }

        protected override void OnValidate()
        {
            if (transforms.Count < num)
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


