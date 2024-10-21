namespace AngleSharp.Browser.Dom.Events
{
    using AngleSharp.Dom.Events;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    /// <summary>
    /// The event that is published in case of an interactivity
    /// request coming from the dynamic DOM.
    /// </summary>
    public class InteractivityEvent<T> : Event
    {
        private Task? _result;

        /// <summary>
        /// Creates a new event for an interactivity request.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">The data to be transported.</param>
        public InteractivityEvent(String eventName, T data)
            : base(eventName)
        {
            Data = data;
        }

        /// <summary>
        /// Gets the currently set result, if any.
        /// </summary>
        public Task? Result => _result;

        /// <summary>
        /// Sets the result to the given value. Multiple results
        /// will be combined accordingly.
        /// </summary>
        /// <param name="value">The resulting task.</param>
        [MemberNotNull(nameof(_result))]
        public void SetResult(Task value)
        {
            if (_result != null)
            {
                _result = Task.WhenAll(_result, value);
            }
            else
            {
                _result = value;
            }
        }

        /// <summary>
        /// Gets the transported data.
        /// </summary>
        public T Data { get; }
    }
}
