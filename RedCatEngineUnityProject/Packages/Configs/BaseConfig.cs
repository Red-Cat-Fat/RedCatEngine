#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RedCatEngine.Configs
{
	/// <summary>
	/// Assign-once id (docs/Plans/UnsignedConfigId, решение пользователя 2026-08-26 по блокеру
	/// P1): <see cref="OnValidate"/> присваивает <c>_id</c> ТОЛЬКО пока поле пусто (<c>0u</c>) и
	/// никогда не перезаписывает уже присвоенное значение. Причина — инцидент 2026-08-26: при
	/// сверке 404 конфиг-ассетов выяснилось, что <c>string.GetHashCode()</c> одной и той же
	/// GUID-строки даёт РАЗНЫЕ значения в разных процессах Unity Editor — 18 из 404 уже
	/// провалидированных конфигов имели на диске значение, не совпадающее с тем, что тот же
	/// GUID даёт при пересчёте в новом процессе. Со старой (безусловной) перезаписью повторная
	/// валидация в очередной сессии редактора молча меняла бы id персистентного ассета.
	/// Assign-once делает это безопасным: пересчёт трогает только конфиги, у которых id ещё не
	/// был присвоен ни разу (в т.ч. 37 из 404, ранее не сериализовавших валидный id вовсе).
	/// </summary>
	public abstract class BaseConfig : ScriptableObject
	{
		[SerializeField] private uint _id;

		public uint Id
			=> _id;

		public void OnValidate()
		{
#if UNITY_EDITOR
			if (!EditorUtility.IsPersistent(this))
				return;

			if (_id == 0u)
			{
				var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
				var computed = ComputeIdForGuid(guid);

				// Присвоение сразу помечается грязным: иначе новое значение живёт только в памяти
				// сессии, а следующий процесс увидит на диске всё тот же 0 и присвоит заново.
				if (computed != 0u)
				{
					_id = computed;
					EditorUtility.SetDirty(this);
				}
			}

			DoValidate();
#endif
		}

#if UNITY_EDITOR
		/// <summary>
		/// FNV-1a от GUID-строки, а НЕ <c>string.GetHashCode()</c>: последний не стабилен между
		/// процессами .NET, и именно из-за него 18 из 404 конфигов разъехались с диском
		/// (инцидент 2026-08-26, см. описание класса). Assign-once защищает уже записанные
		/// ассеты, но новый конфиг обязан получать воспроизводимое значение — иначе тот же
		/// дефект вернулся бы через вновь создаваемые конфиги.
		/// Возвращает 0u, если id присвоить нельзя: пустой GUID (ассет вне AssetDatabase — иначе
		/// все такие конфиги получили бы один и тот же id) или вырожденный хэш ровно 0
		/// (значение зарезервировано под «пусто»). В обоих случаях присвоение откладывается
		/// до следующей валидации, а не порождает коллизию.
		/// Публичный, потому что editor-инструмент проверки целостности id
		/// (<c>ConfigIdIntegrityWindow</c>) обязан считать id по ТОЙ ЖЕ формуле — иначе он
		/// чинил бы коллизии значениями, расходящимися с продакшен-логикой.
		/// </summary>
		public static uint ComputeIdForGuid(string guid)
		{
			if (string.IsNullOrEmpty(guid))
				return 0u;

			unchecked
			{
				const uint offsetBasis = 2166136261u;
				const uint prime = 16777619u;

				var hash = offsetBasis;
				foreach (var symbol in guid)
				{
					hash ^= symbol;
					hash *= prime;
				}

				return hash;
			}
		}
#endif

		protected virtual void DoValidate() { }

		public override bool Equals(object other)
		{
			return other as BaseConfig != null && other.GetType() == this.GetType() && Equals((BaseConfig)other);
		}

		private bool Equals(BaseConfig other)
		{
			return _id == other._id;
		}

		public override int GetHashCode()
			=> unchecked((int)_id);

		public override string ToString()
		{
			return $"{name} ({_id})";
		}
	}
}
