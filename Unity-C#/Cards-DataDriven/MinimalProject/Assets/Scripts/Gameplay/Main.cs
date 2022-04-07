using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class Main : MonoBehaviour
    {
        [Header("Cards")]
        [SerializeField] private Data_Deck _deck;
        [SerializeField] private CardCreation.CardCreator _cardCreator;

        [Header("References")]
        [SerializeField] private Camera _cam;
        [SerializeField] private TransformLayoutGroup _player1;
        [SerializeField] private TransformLayoutGroup _player2;

        [Header("Settings")]
        [SerializeField, Range(1, 10)] int _cardCount = 6;

        private void Start()
        {
            var z = _player1.transform.localPosition.z;
            _player1.transform.position = _cam.ViewportToWorldPoint(new Vector3(0.5f, 0.75f, z));
            _player2.transform.position = _cam.ViewportToWorldPoint(new Vector3(0.5f, 0.25f, z));

            var width = Vector3.Distance(_cam.ViewportToWorldPoint(new Vector3(0, 0, z)), _cam.ViewportToWorldPoint(new Vector3(1, 0, z)));
            var height = Vector3.Distance(_cam.ViewportToWorldPoint(new Vector3(0, 0, z)), _cam.ViewportToWorldPoint(new Vector3(0, 1, z)));

            _player1.AreaDimensions = new Vector2(width, height * 0.5f);
            _player2.AreaDimensions = new Vector2(width, height * 0.5f);


            _deck.Shuffle();

            for (int i = 0; i < _cardCount; i++)
            {
                DrawCard().transform.parent = _player1.transform;
            }

            for (int i = 0; i < _cardCount; i++)
            {
                DrawCard().transform.parent = _player2.transform;
            }

            _player1.UpdateLayout();
            _player2.UpdateLayout();
        }

        private Card DrawCard()
        {
            return _cardCreator.Create(_deck.DrawNext());
        }
    }
}
