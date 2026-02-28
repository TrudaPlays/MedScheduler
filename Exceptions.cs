using System;

namespace MedScheduler
{
    /// <summary>
    /// Thrown when attempting to add or reschedule an appointment that overlaps 
    /// with an existing appointment for the same provider or the same room.
    /// </summary>
    public class DoubleBookingException : Exception
    {
        public DoubleBookingException(string message)
            : base(message)
        {
        }

        // Optional: constructor that also accepts inner exception
        public DoubleBookingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }


    /// <summary>
    /// Thrown when an appointment has invalid timing:
    /// - end time ≤ start time
    /// - duration shorter than the minimum (15 minutes)
    /// - outside clinic business hours (08:00–17:00)
    /// </summary>
    public class InvalidAppointmentTimeException : Exception
    {
        public InvalidAppointmentTimeException(string message)
            : base(message)
        {
        }

        // Optional: constructor that also accepts inner exception
        public InvalidAppointmentTimeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
