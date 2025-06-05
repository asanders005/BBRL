using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Event - A simple observer pattern implementation using ScriptableObject.
/// Allows broadcasting events with no parameters to multiple listeners.
/// </summary>
[CreateAssetMenu(menuName = "Events/Vector2 Event")]
public class Vector2Event : ScriptableObjectBase
{
	/// <summary>
	/// Unity Action that holds references to all subscribed methods.
	/// Allows dynamically calling multiple functions when the event is raised.
	/// </summary>
	public UnityAction<Vector2> OnEventRaised;

	/// <summary>
	/// Raises the event with no parameters.
	/// </summary>
	public void RaiseEvent(Vector2 vector)
	{
		OnEventRaised?.Invoke(vector);
	}

	/// <summary>
	/// Subscribes a listener to the event.
	/// </summary>
	/// <param name="listener">The method that will be called when the event is raised.</param>
	public void Subscribe(UnityAction<Vector2> listener)
	{
		if (listener != null) OnEventRaised += listener;
	}

	/// <summary>
	/// Unsubscribes a listener from the event.
	/// </summary>
	/// <param name="listener">The method that should no longer be called when the event is raised.</param>
	public void Unsubscribe(UnityAction<Vector2> listener)
	{
		if (listener != null) OnEventRaised -= listener;
	}
}