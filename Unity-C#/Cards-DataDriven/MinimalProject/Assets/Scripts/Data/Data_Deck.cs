using System;
using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "Deck", menuName = "ScriptableObjects/Deck", order = 1)]
	public class Data_Deck : ScriptableObject
	{
		[SerializeField] private Data_Card[] _cards = new Data_Card[1];

		private Data_Card[] _cardsActive;
		private int _index = -1;

		public void Shuffle()
		{
			_index = -1;

			if (_cardsActive == null || _cardsActive.Length != _cards.Length)
			{
				_cardsActive = new Data_Card[_cards.Length];
				Array.Copy(_cards, _cardsActive, _cards.Length);
			}

			// Algorithm: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
			for (int i = 0; i < _cardsActive.Length; i++)
			{
				var original = _cardsActive[i];
				int random = UnityEngine.Random.Range(i, _cardsActive.Length);
				_cardsActive[i] = _cardsActive[random];
				_cardsActive[random] = original;
			}
		}

		public Data_Card DrawNext()
		{
			_index = (_index + 1) % _cards.Length;

			return _cards[_index];
		}
	}
}