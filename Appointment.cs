using System;

namespace MedScheduler
{
    public class Appointment
    {
        //Class Variables for this assignment
        public string Id { get; }
        public string PatientName { get; }
        public string ProviderName { get; }
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public string Room { get; }

        public Appointment(string id, string patientName, string providerName,
                          DateTime start, DateTime end, string room)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Appointment ID cannot be null or empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(patientName))
                throw new ArgumentException("Patient name cannot be null or empty.", nameof(patientName));

            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException("Provider name cannot be null or empty.", nameof(providerName));

            if (string.IsNullOrWhiteSpace(room))
                throw new ArgumentException("Room cannot be null or empty.", nameof(room));

            if (end <= start)
                throw new ArgumentException("End time must be after start time.", nameof(end));

            Id = id.Trim();
            PatientName = patientName.Trim();
            ProviderName = providerName.Trim();
            Start = start;
            End = end;
            Room = room.Trim();
        }

        public void Reschedule(DateTime newStart, DateTime newEnd)
        {
            if (newEnd <= newStart)
            {
                throw new ArgumentException("New end time must be after new start time.");

            }

            var oldTimes = $"{Start:yyyy-MM-dd HH-mm}-{End:HH:mm}";
            Start = newStart;
            End = newEnd;

            Logger.Info($"Rescheduled {Id}: {oldTimes} → {newStart:yyyy-MM-dd HH:mm}–{newEnd:HH:mm}");
        }

        public override string ToString()
        {
            return $"[{Id}] {Start:yyyy-MM-dd HH:mm}–{End:HH:mm} {PatientName} with {ProviderName} in {Room}";
        }

    }
}
