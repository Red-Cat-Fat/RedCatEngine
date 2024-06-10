using System;

namespace RedCatEngine.Configs
{
	public sealed class ConfigID<TBaseConfig>
		: IEquatable<ConfigID<TBaseConfig>>,
			IComparable<ConfigID<TBaseConfig>>
		where TBaseConfig : BaseConfig
	{
		private const int InvalidId = 0;
		public static ConfigID<TBaseConfig> Invalid => new(InvalidId);
		private readonly int _id;

		private ConfigID(int id) => _id = id;
		public ConfigID(TBaseConfig config) => _id = config.ID;

		public bool Equals(ConfigID<TBaseConfig> other) => _id == (other != null ? other._id : InvalidId);
		public override bool Equals(object obj) => obj is ConfigID<TBaseConfig> other && Equals(other);
		public override int GetHashCode() => _id;
		public override string ToString() => $"{typeof(TBaseConfig)}: {_id.ToString()}";

		public static bool operator ==(ConfigID<TBaseConfig> a, ConfigID<TBaseConfig> b) => (a != null ? a._id : InvalidId) == (b != null ? b._id : InvalidId);
		public static bool operator !=(ConfigID<TBaseConfig> a, ConfigID<TBaseConfig> b) => !(a == b);
		public static bool operator ==(ConfigID<TBaseConfig> a, TBaseConfig b) => (a != null ? a._id : InvalidId) == (b != null ? b.ID : Invalid._id);
		public static bool operator !=(ConfigID<TBaseConfig> a, TBaseConfig b) => (a != null ? a._id : InvalidId) != (b != null ? b.ID : Invalid._id);

		public static explicit operator ConfigID<TBaseConfig>(int id) => new(id);
		public static explicit operator int(ConfigID<TBaseConfig> configId) => configId._id;

		public static implicit operator ConfigID<TBaseConfig>(TBaseConfig config) =>
			config != null ? new ConfigID<TBaseConfig>(config.ID) : Invalid;

		public int CompareTo(ConfigID<TBaseConfig> other)
		{
			if (ReferenceEquals(this, other))
				return 0;
			if (ReferenceEquals(null, other))
				return 1;
			return _id.CompareTo(other._id);
		}
	}
}