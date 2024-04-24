using System.Collections.Generic;

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
	}
}