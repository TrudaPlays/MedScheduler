using System;

namespace MedScheduler
{
    /// <summary>
    /// Exception thrown when attempting to schedule an appointment that would cause
    /// a double-booking (overlapping time) for the same provider or the same room.
    /// </summary>
    public class DoubleBookingException : Exception
    {
        public DoubleBookingException()
            : base("Double booking conflict detected.")
        {
        }

        public DoubleBookingException(string message)
            : base(message)
        {
        }

        public DoubleBookingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when an appointment has invalid time-related properties, such as:
    /// - End time is not after start time
    /// - Duration is shorter than the minimum required (15 minutes)
    /// - Appointment falls outside clinic business hours (08:00–17:00)
    /// </summary>
    public class InvalidAppointmentTimeException : Exception
    {
        public InvalidAppointmentTimeException()
            : base("Invalid appointment time.")
        {
        }

        public InvalidAppointmentTimeException(string message)
            : base(message)
        {
        }

        public InvalidAppointmentTimeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}