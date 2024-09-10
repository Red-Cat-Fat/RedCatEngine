﻿using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Logics.Operands
{
	[Serializable]
	[SRName("Logic/Or")]
	public class OrValue : IBoolValue
	{
		[SR]
		[SerializeReference]
		private IBoolValue _left = ConstantBoolValue.False;

		[SR]
		[SerializeReference]
		private IBoolValue _right = ConstantBoolValue.False;

		public bool GetValue(IApplicationContainer applicationContainer)
			=> _left.GetValue(applicationContainer) || _right.GetValue(applicationContainer);
	}
}