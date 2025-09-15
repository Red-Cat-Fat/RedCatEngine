using UnityEngine;

namespace RedCatEngine.CommonServices.Extensions
{
	public static class VectorsExtensions
	{
		public static Vector2 NewWithX(this Vector2 baseValue, float xValue)
			=> new(
				xValue,
				baseValue.y);

		public static Vector2 NewWithY(this Vector2 baseValue, float yValue)
			=> new(
				baseValue.x,
				yValue);

		public static Vector3 NewWithX(this Vector3 baseValue, float xValue)
			=> new(
				xValue,
				baseValue.y,
				baseValue.z);

		public static Vector3 NewWithY(this Vector3 baseValue, float yValue)
			=> new(
				baseValue.x,
				yValue,
				baseValue.z);

		public static Vector3 NewWithZ(this Vector3 baseValue, float zValue)
			=> new(
				baseValue.x,
				baseValue.y,
				zValue);
	}
}