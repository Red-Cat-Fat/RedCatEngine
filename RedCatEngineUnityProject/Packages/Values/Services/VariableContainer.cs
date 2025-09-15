using System;
using System.Collections.Generic;
using RedCatEngine.CommonServices.Containers.Components;
using RedCatEngine.CommonServices.SpecialTypes.ChangedValues.BasedTypes;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.Values.Variants.Contents.Configs.Variables;

namespace RedCatEngine.Values.Services
{
	public class VariableContainer : IRedComponent
	{
		public event Action<VariableConfig, FloatDeltaChangeValue> ChangeVariableEvent;
		private readonly VariableContainer _parent;
		private readonly Dictionary<VariableConfig, float> _values = new();

		[Inject]
		public VariableContainer()
		{
		}

		private VariableContainer(VariableContainer parent)
		{
			_parent = parent;
		}

		public VariableContainer MakeChild() 
			=> new(this);

		private bool TryGetValue(VariableConfig variableConfig, out float result)
		{
			return _values.TryGetValue(variableConfig, out result);
		}

		public float GetValue(VariableConfig variableConfig)
		{
			if (TryGetValue(variableConfig, out var result)
				|| (_parent != null
					&& _parent.TryGetValue(variableConfig, out result)))
				return result;

			_values.Add(variableConfig, variableConfig.DefaultValue);
			return variableConfig.DefaultValue;
		}

		public void SetValue(VariableConfig variableConfig, float value)
		{
			if (!_values.TryGetValue(variableConfig, out _))
			{
				ChangeVariableEvent?.Invoke(variableConfig, new FloatDeltaChangeValue(0, value));
				_values.Add(variableConfig, value);
				return;
			}

			var oldValue = _values[variableConfig];
			_values[variableConfig] = value;
			ChangeVariableEvent?.Invoke(variableConfig, new FloatDeltaChangeValue(oldValue, value));
		}

		public void Clear()
		{
			_values.Clear();
		}
	}
}