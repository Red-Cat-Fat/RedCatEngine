using System;

namespace RedCatEngine.StateMachine.StateMachines
{
	public class StateStepData
	{
		public readonly Type StateType;
		public readonly Type StatePayloadType;
		public readonly object Payload;

		public bool IsPayLoadState
			=> StatePayloadType != default && Payload != null;

		public StateStepData(Type stateType)
			=> StateType = stateType;

		public StateStepData(
			Type stateType,
			Type statePayloadType,
			object payload
		)
			: this(stateType)
		{
			StatePayloadType = statePayloadType;
			Payload = payload;
		}

		public override bool Equals(object obj)
			=> obj is StateStepData stepData && Equals(stepData);

		private bool Equals(StateStepData other)
		{
			return StateType == other.StateType &&
			       StatePayloadType == other.StatePayloadType &&
			       Equals(Payload, other.Payload);
		}

		public override int GetHashCode()
			=> HashCode.Combine(
				StateType,
				StatePayloadType,
				Payload);
	}
}