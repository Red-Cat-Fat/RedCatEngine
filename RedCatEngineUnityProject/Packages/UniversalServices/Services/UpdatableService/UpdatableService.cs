using System;
using RedCatEngine.CommonServices.Services.Logs;
using System.Collections.Generic;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.UpdatableService
{
	public class UpdatableService : IUpdatableService, IDisposable
	{
		private readonly List<IFixedUpdatable> _fixedUpdateListeners = new();
		private readonly List<ILateUpdatable> _lateUpdateListeners = new();
		private readonly List<IUpdatable> _pendingAdd = new();
		private readonly List<IFixedUpdatable> _pendingFixedAdd = new();
		private readonly List<IFixedUpdatable> _pendingFixedRemove = new();
		private readonly List<ILateUpdatable> _pendingLateAdd = new();
		private readonly List<ILateUpdatable> _pendingLateRemove = new();
		private readonly List<IUpdatable> _pendingRemove = new();
		private readonly string _tagName;
		private readonly List<IUpdatable> _updateListeners = new();
		private readonly IUpdateTrigger _updateTrigger;
		private bool _isUpdatingProgress;

		[Inject]
		public UpdatableService(IUpdateTrigger updateTrigger, string tagName)
		{
			_updateTrigger = updateTrigger;
			_tagName = tagName;
			_updateTrigger.UpdateEvent += Update;
			_updateTrigger.FixedUpdateEvent += FixedUpdate;
			_updateTrigger.LateUpdateEvent += LateUpdate;
		}

		public void Dispose()
		{
			_updateTrigger.UpdateEvent -= Update;
			_updateTrigger.FixedUpdateEvent -= FixedUpdate;
			_updateTrigger.LateUpdateEvent -= LateUpdate;

			_updateListeners.Clear();
			_fixedUpdateListeners.Clear();
			_lateUpdateListeners.Clear();
			_pendingAdd.Clear();
			_pendingRemove.Clear();
			_pendingFixedAdd.Clear();
			_pendingFixedRemove.Clear();
			_pendingLateAdd.Clear();
			_pendingLateRemove.Clear();
		}

		public void AddUpdatable(IUpdatable updatable)
		{
			if(updatable == null)
				return;

			if(_isUpdatingProgress)
			{
				if(!_pendingAdd.Contains(updatable))
					_pendingAdd.Add(updatable);

				return;
			}

			if(!_updateListeners.Contains(updatable))
				_updateListeners.Add(updatable);
		}

		public void AddUpdatable(IUpdatable[] updatedElements)
		{
			foreach(var updated in updatedElements)
				AddUpdatable(updated);
		}

		public void AddFixedUpdatable(IFixedUpdatable updatable)
		{
			if(updatable == null)
				return;

			if(_isUpdatingProgress)
			{
				if(!_pendingFixedAdd.Contains(updatable))
					_pendingFixedAdd.Add(updatable);

				return;
			}

			if(!_fixedUpdateListeners.Contains(updatable))
				_fixedUpdateListeners.Add(updatable);
		}

		public void AddFixedUpdatable(IFixedUpdatable[] updatedElements)
		{
			foreach(var updated in updatedElements)
				AddFixedUpdatable(updated);
		}

		public void AddLateUpdatable(ILateUpdatable updatable)
		{
			if(updatable == null)
				return;

			if(_isUpdatingProgress)
			{
				if(!_pendingLateAdd.Contains(updatable))
					_pendingLateAdd.Add(updatable);

				return;
			}

			if(!_lateUpdateListeners.Contains(updatable))
				_lateUpdateListeners.Add(updatable);
		}

		public void AddLateUpdatable(ILateUpdatable[] updatedElements)
		{
			foreach(var updated in updatedElements)
				AddLateUpdatable(updated);
		}

		public void RemoveUpdated(IUpdatable updatableElement)
		{
			if(updatableElement == null)
				return;

			if(_isUpdatingProgress)
			{
				if(!_pendingRemove.Contains(updatableElement))
					_pendingRemove.Add(updatableElement);

				return;
			}

			_updateListeners.Remove(updatableElement);
		}

		public void RemoveFixedUpdated(IFixedUpdatable updatableElement)
		{
			if(updatableElement == null)
				return;

			if(_isUpdatingProgress)
			{
				if(!_pendingFixedRemove.Contains(updatableElement))
					_pendingFixedRemove.Add(updatableElement);

				return;
			}

			_fixedUpdateListeners.Remove(updatableElement);
		}

		public void RemoveUpdated(IUpdatable[] updatedElements)
		{
			foreach(var updated in updatedElements)
				RemoveUpdated(updated);
		}

		public void RemoveFixedUpdated(IFixedUpdatable[] updatedElements)
		{
			foreach(var updated in updatedElements)
				RemoveFixedUpdated(updated);
		}

		public void RemoveLateUpdated(ILateUpdatable updatableElement)
		{
			if(updatableElement == null)
				return;

			if(_isUpdatingProgress)
			{
				if(!_pendingLateRemove.Contains(updatableElement))
					_pendingLateRemove.Add(updatableElement);

				return;
			}

			_lateUpdateListeners.Remove(updatableElement);
		}

		public void RemoveLateUpdated(ILateUpdatable[] updatedElements)
		{
			foreach(var updated in updatedElements)
				RemoveLateUpdated(updated);
		}

		private void Update()
		{
			_isUpdatingProgress = true;

			try
			{
				for(var i = 0; i < _updateListeners.Count; i++)
				{
					var listener = _updateListeners[i];
					try
					{
						listener.OnUpdate();
					}
					catch(Exception ex)
					{
						LogHub.Root.WriteException(
								ex,
								$"UpdatableService ({_tagName}) failed on listener: {DescribeUpdatable(listener)}"
						);
						throw;
					}
				}
			}
			finally
			{
				_isUpdatingProgress = false;
				ApplyPendingChanges();
			}
		}

		private void FixedUpdate()
		{
			_isUpdatingProgress = true;

			try
			{
				for(var i = 0; i < _fixedUpdateListeners.Count; i++)
				{
					var listener = _fixedUpdateListeners[i];
					try
					{
						listener.OnFixedUpdate();
					}
					catch(Exception ex)
					{
						LogHub.Root.WriteException(
								ex,
								$"UpdatableService ({_tagName}) failed on fixed listener: {DescribeUpdatable(listener)}"
						);
						throw;
					}
				}
			}
			finally
			{
				_isUpdatingProgress = false;
				ApplyPendingFixedChanges();
			}
		}

		private void ApplyPendingChanges()
		{
			foreach(var updatable in _pendingRemove)
				_updateListeners.Remove(updatable);

			_pendingRemove.Clear();

			foreach(var updatable in _pendingAdd)
			{
				if(!_updateListeners.Contains(updatable))
					_updateListeners.Add(updatable);
			}

			_pendingAdd.Clear();
		}

		private static string DescribeUpdatable(IUpdatable updatable) =>
				DescribeListener(updatable);

		private static string DescribeUpdatable(IFixedUpdatable updatable) =>
				DescribeListener(updatable);

		private static string DescribeUpdatable(ILateUpdatable updatable) =>
				DescribeListener(updatable);

		private static string DescribeListener(object updatable)
		{
			if(updatable is Component component)
				return $"{component.GetType().Name} on GameObject '{component.gameObject.name}' (path: {GetGameObjectPath(component.gameObject)})";

			return updatable?.ToString() ?? "null";
		}

		private static string GetGameObjectPath(GameObject gameObject)
		{
			if(gameObject == null)
				return "<destroyed>";

			var path = gameObject.name;
			var parent = gameObject.transform.parent;

			while(parent != null)
			{
				path = parent.name + "/" + path;
				parent = parent.parent;
			}

			return path;
		}

		private void ApplyPendingFixedChanges()
		{
			foreach(var updatable in _pendingFixedRemove)
				_fixedUpdateListeners.Remove(updatable);

			_pendingFixedRemove.Clear();

			foreach(var updatable in _pendingFixedAdd)
			{
				if(!_fixedUpdateListeners.Contains(updatable))
					_fixedUpdateListeners.Add(updatable);
			}

			_pendingFixedAdd.Clear();
		}

		private void LateUpdate()
		{
			_isUpdatingProgress = true;

			try
			{
				for(var i = 0; i < _lateUpdateListeners.Count; i++)
				{
					var listener = _lateUpdateListeners[i];
					try
					{
						listener.OnLateUpdate();
					}
					catch(Exception ex)
					{
						LogHub.Root.WriteException(
								ex,
								$"UpdatableService ({_tagName}) failed on late listener: {DescribeUpdatable(listener)}"
						);
						throw;
					}
				}
			}
			finally
			{
				_isUpdatingProgress = false;
				ApplyPendingLateChanges();
			}
		}

		private void ApplyPendingLateChanges()
		{
			foreach(var updatable in _pendingLateRemove)
				_lateUpdateListeners.Remove(updatable);

			_pendingLateRemove.Clear();

			foreach(var updatable in _pendingLateAdd)
			{
				if(!_lateUpdateListeners.Contains(updatable))
					_lateUpdateListeners.Add(updatable);
			}

			_pendingLateAdd.Clear();
		}
	}
}
