using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.UI.HierarchicalVisitor
{
    public class HierarchicalVisitor<T> : MonoBehaviour
    {
        #region FIELDS
        private List<T> elements;
        protected int currentIndex = 0;
        protected T selectedElement;
        #endregion

        #region PROPERTIES
        public virtual int CurrentIndex { get => currentIndex; set => currentIndex = value; }
        public List<T> Elements { get => elements; set => elements = value; }
        public virtual T SelectedElement { get => selectedElement; set => selectedElement = value; }
        #endregion

        #region METHODS
        /// <summary>
        /// Condition that a child must meet.
        /// Override this method to check specific conditions.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected virtual bool IsElementConditionMet(T element)
        {
            return element != null;
        }
        protected virtual int IterationLimit()
        {
            return elements?.Count ?? 0;
        }
        protected virtual bool UpCondition()
        {
            return Input.GetKeyDown(KeyCode.UpArrow);
        }
        protected virtual bool DownCondition()
        {
            return Input.GetKeyDown(KeyCode.DownArrow);
        }
        private void LateUpdate()
        {
            if (elements == null || IterationLimit() == 0)
            {
                return;
            }

            // Avoid checking if the keys were pressed multiple times throughout the code
            bool upCondition = UpCondition();
            bool downCondition = DownCondition();

            if (!upCondition && !downCondition)
            {
                return; // No keys are pressed
            }

            // Sailing direction: -1 up (UpArrow), 1 down (DownArrow)
            int direction = upCondition ? -1 : 1;

            // Caps the number of iterations to avoid infinite loops
            int iterationLimit = IterationLimit();
            int disposableIndex = CurrentIndex;

            for (int i = 0; i < iterationLimit; i++)
            {
                // Ensures next index within range
                disposableIndex = Mathf.Clamp(disposableIndex + direction, 0, iterationLimit - 1);

                T element = elements[disposableIndex];

                if (IsElementConditionMet(element))
                {
                    CurrentIndex = disposableIndex;
                    break;
                }
            }
        }
        #endregion
    }
}