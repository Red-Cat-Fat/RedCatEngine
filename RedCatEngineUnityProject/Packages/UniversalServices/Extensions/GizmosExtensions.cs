using System.Collections.Generic;
using UnityEngine;

namespace RedCatEngine.CommonServices.Extensions
{
	public static class GizmosExtensions
	{
		public static void DrawCube(
			Vector3 a,
			Vector3 b,
			Color color
		)
		{
			Gizmos.color = color;
			Gizmos.DrawLine(
				a,
				new Vector3(
					a.x,
					b.y,
					a.z));
			Gizmos.DrawLine(
				a,
				new Vector3(
					a.x,
					a.y,
					b.z));
			Gizmos.DrawLine(
				a,
				new Vector3(
					b.x,
					a.y,
					a.z));

			Gizmos.DrawLine(
				b,
				new Vector3(
					b.x,
					a.y,
					b.z));
			Gizmos.DrawLine(
				b,
				new Vector3(
					b.x,
					b.y,
					a.z));
			Gizmos.DrawLine(
				b,
				new Vector3(
					a.x,
					b.y,
					b.z));

			Gizmos.DrawLine(
				new Vector3(
					a.x,
					b.y,
					a.z),
				new Vector3(
					b.x,
					b.y,
					a.z));
			Gizmos.DrawLine(
				new Vector3(
					a.x,
					b.y,
					a.z),
				new Vector3(
					a.x,
					b.y,
					b.z));
			Gizmos.DrawLine(
				new Vector3(
					a.x,
					a.y,
					b.z),
				new Vector3(
					a.x,
					b.y,
					b.z));
			Gizmos.DrawLine(
				new Vector3(
					b.x,
					a.y,
					a.z),
				new Vector3(
					b.x,
					a.y,
					b.z));
			Gizmos.DrawLine(
				new Vector3(
					a.x,
					a.y,
					b.z),
				new Vector3(
					b.x,
					a.y,
					b.z));
			Gizmos.DrawLine(
				new Vector3(
					b.x,
					a.y,
					a.z),
				new Vector3(
					b.x,
					b.y,
					a.z));
		}

		public static void DrawBox(
			Vector2 a,
			Vector2 b,
			Color color
		)
		{
			Gizmos.color = color;
			Gizmos.DrawLine(
				new Vector2(
					a.x,
					a.y),
				new Vector2(
					a.x,
					b.y));
			Gizmos.DrawLine(
				new Vector2(
					a.x,
					a.y),
				new Vector2(
					b.x,
					a.y));
			Gizmos.DrawLine(
				new Vector2(
					b.x,
					b.y),
				new Vector2(
					a.x,
					b.y));
			Gizmos.DrawLine(
				new Vector2(
					b.x,
					b.y),
				new Vector2(
					b.x,
					a.y));
		}

		public static void DrawBox(
			Vector3 a,
			Vector3 b,
			Vector3 shift,
			Color color
		)
		{
			Gizmos.color = color;
			a = new Vector3(a.x + shift.x, a.y + shift.y);
			b = new Vector3(b.x + shift.x, b.y + shift.y);
			Gizmos.DrawLine(
				new Vector2(
					a.x,
					a.y),
				new Vector2(
					a.x,
					b.y));
			Gizmos.DrawLine(
				new Vector2(
					a.x,
					a.y),
				new Vector2(
					b.x,
					a.y));
			Gizmos.DrawLine(
				new Vector2(
					b.x,
					b.y),
				new Vector2(
					a.x,
					b.y));
			Gizmos.DrawLine(
				new Vector2(
					b.x,
					b.y),
				new Vector2(
					b.x,
					a.y));
		}
		
		public static void DrawLinks(
			Component from,
			IEnumerable<Component> to,
			Color color
		)
		{
			Gizmos.color = color;
			foreach (var target in to)
			{
				Gizmos.DrawLine(from.transform.position, target.transform.position);
			}
		}
	}
}