using System.Collections.Generic;
using UnityEngine;

namespace Server.UI
{
    public class ListWithPoolTemplate<T> : MonoBehaviour
        where T : MonoBehaviour
    {
#if UNITY_EDITOR
        [Tooltip(
            "Number of list elements to add into the pool by using the 'Increase List Pool' option on the Context Menu")]
        [SerializeField]
        private int m_PoolChunkSize = 10;

        [SerializeField] private T m_ElementPrefab;
#endif

        [SerializeField] protected List<T> m_AvailableElements = new List<T>();
        [SerializeField] protected List<T> m_UsedElements = new List<T>();

        public T AddElement()
        {
            if (m_AvailableElements.Count == 0)
            {
                Debug.LogWarningFormat(
                    "The pool for the List on {0} was not populated with enough elements in Editor-time. " +
                    "Consider increasing the pool for avoiding Instantiation of its elements in runtime.",
                    gameObject.name);
                IncreasePoolInternal();
            }

            var newElement = m_AvailableElements[0];
            m_AvailableElements.RemoveAt(0);

            m_UsedElements.Add(newElement);
            newElement.gameObject.SetActive(true);

            return newElement;
        }

        public void RemoveElement(T elementToRemove)
        {
            if (elementToRemove == default(PlayerListElement))
            {
                Debug.LogErrorFormat("Trying to remove from the List {0} the Player '{1}' but is not the list!",
                    gameObject.name, elementToRemove.gameObject.name);
                return;
            }

            m_UsedElements.Remove(elementToRemove);
            elementToRemove.gameObject.SetActive(false);
            m_AvailableElements.Add(elementToRemove);
        }

#if UNITY_EDITOR
        protected void IncreasePoolInternal()
        {
            for (var i = 0; i < m_PoolChunkSize; ++i)
            {
                var newElement = Instantiate(m_ElementPrefab, transform);
                m_AvailableElements.Add(newElement);
                newElement.gameObject.SetActive(false);
            }
        }
#endif
    }
}