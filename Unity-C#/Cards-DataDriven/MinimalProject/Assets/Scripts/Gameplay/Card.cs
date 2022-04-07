using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(BoxCollider))]
    public class Card : MonoBehaviour, ISize
    {
        private event Action<Card> _onClicked;
        private event Action _onHoverEnter;
        private event Action _onHoverExit;

        private BoxCollider _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
        }

        private void OnMouseEnter()
        {
            _onHoverEnter?.Invoke();
        }

        private void OnMouseExit()
        {
            _onHoverExit?.Invoke();
        }

        private void OnMouseUpAsButton()
        {
            _onClicked?.Invoke(this);
        }

        public Vector3 Size()
        {
            return _collider.size;
        }
    }
}
