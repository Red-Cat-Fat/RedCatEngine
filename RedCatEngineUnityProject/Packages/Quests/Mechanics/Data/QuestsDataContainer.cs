using System;
using System.Collections.Generic;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;
using UnityEngine;

namespace RedCatEngine.Quests.Mechanics.Data
{
	[Serializable]
	public class QuestsDataContainer
	{
		public static QuestsDataContainer Empty
			=> new(true);

		private bool _isEmpty;

		//todo: make generic serialization for union lists
		[SerializeField]
		private List<DeltaChangeProgressQuestData> _deltaChangeActiveQuests = new();
		[SerializeField]
		private List<CollectProgressQuestData> _collectActiveQuests = new();

		private QuestsDataContainer(bool isEmpty)
			=> _isEmpty = isEmpty;

		public QuestsDataContainer()
			: this(false) { }

		public void AddQuest(IQuestData questData)
		{
			switch (questData)
			{
				case DeltaChangeProgressQuestData deltaChangeProgressQuestData:
					_deltaChangeActiveQuests.Add(deltaChangeProgressQuestData);
					break;
				case CollectProgressQuestData collectProgressQuestData:
					_collectActiveQuests.Add(collectProgressQuestData);
					break;
				default:
					throw new Exception("Not supported quest data type");
			}
		}

		public List<IQuestData> GetQuests()
		{
			var result = new List<IQuestData>();
			result.AddRange(_deltaChangeActiveQuests);
			result.AddRange(_collectActiveQuests);
			return result;
		}
		
		public static bool operator ==(QuestsDataContainer a, QuestsDataContainer b)
			=> a is null && b is null || a is not null && b is not null && a.Equals(b);

		public static bool operator !=(QuestsDataContainer a, QuestsDataContainer b)
			=> !(a == b);

		public override bool Equals(object obj)
		{
			if (obj is not QuestsDataContainer container)
				return false;

			if (_isEmpty && container._isEmpty)
				return true;

			if (container._deltaChangeActiveQuests.Count != _deltaChangeActiveQuests.Count)
				return false;

			for (var i = 0; i < _deltaChangeActiveQuests.Count; i++)
			{
				if (!container._deltaChangeActiveQuests[i].Equals(_deltaChangeActiveQuests[i]))
					return false;
			}
			for (var i = 0; i < _collectActiveQuests.Count; i++)
			{
				if (!container._collectActiveQuests[i].Equals(_collectActiveQuests[i]))
					return false;
			}

			return true;
		}

		protected bool Equals(QuestsDataContainer other)
		{
			if (_isEmpty && other._isEmpty)
				return true;

			return Equals(_deltaChangeActiveQuests, other._deltaChangeActiveQuests) &&
			       Equals(_collectActiveQuests, other._collectActiveQuests);
		}

		public override int GetHashCode()
		{
			return (_deltaChangeActiveQuests != null ? _deltaChangeActiveQuests.GetHashCode() : 0);
		}
	}
}