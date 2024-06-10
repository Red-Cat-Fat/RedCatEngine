using System;
using System.Collections.Generic;
using System.Linq;

namespace RedCatEngine.Configs
{
	public static class BaseConfigExtensions
	{
		public static bool TryFindById<T>(this IEnumerable<T> configs, int id, out T targetConfig) where T : BaseConfig
		{
			foreach (var config in configs)
			{
				if (config.ID != id)
					continue;

				targetConfig = config;
				return true;
			}

			targetConfig = default;
			return false;
		}

		public static bool TryFindById<T>(this IEnumerable<T> configs, ConfigID<T> id, out T targetConfig) where T : BaseConfig
		{
			targetConfig = configs.FirstOrDefault(config => config == id);
			return targetConfig != default;
		}

		public static ConfigID<T>[] AsConfigIdArray<T>(this IEnumerable<T> array)
			where T : BaseConfig => array != null
			? array.Select(link => (ConfigID<T>) link).ToArray()
			: Array.Empty<ConfigID<T>>();
	}
}