using System;
using System.Collections.Generic;
using UnityEngine;

namespace MM.Server.UI
{
    public class ListWithPoolTemplate<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        [Tooltip("Number of list elements to add into the pool by using the 'Increase List Pool' option on the Context Menu")]
        [SerializeField]
        private int m_PoolChunkSize = 10;

        // This field is assigned in Editor, therefore the warning CS0649 about the lack of assignation is disabled
        // Its assignation is checked on the Awake method and throwing an exception if is still null in runtime
#pragma warning disable 649
        [SerializeField] private T m_ElementPrefab;
#pragma warning restore 649
        [SerializeField] protected List<T> m_AvailableElements = new List<T>();
        
        protected List<T> m_UsedElements = new List<T>();

        private void Awake()
        {
            if (m_ElementPrefab == null)
                throw new NullReferenceException($"The element prefab reference of the list on {gameObject.name} is not assigned!");
        }

        protected T AddElement()
        {
            if (m_AvailableElements.Count == 0)
            {
                Debug.LogWarning($"The pool for the List on {gameObject.name} was not populated with enough " +
                   $"elements in Editor-time. Consider increasing the pool for avoiding Instantiation of its elements in runtime.");
                IncreasePoolInternal();
            }

            var newElement = m_AvailableElements[0];
            m_AvailableElements.RemoveAt(0);

            m_UsedElements.Add(newElement);
            newElement.gameObject.SetActive(true);

            return newElement;
        }

        protected void RemoveElement(T elementToRemove)
        {
            if (elementToRemove == default(PlayerListElement))
            {
                Debug.LogError($"Trying to remove from the List {gameObject.name} the Player " +
                               $"'{elementToRemove.gameObject.name}' but is not the list!");
                return;
            }

            m_UsedElements.Remove(elementToRemove);
            elementToRemove.gameObject.SetActive(false);
            m_AvailableElements.Add(elementToRemove);
        }

        protected void IncreasePoolInternal()
        {
            for (var i = 0; i < m_PoolChunkSize; ++i)
            {
                var newElement = Instantiate(m_ElementPrefab, transform);
                m_AvailableElements.Add(newElement);
                newElement.gameObject.SetActive(false);
            }
        }
    }
}