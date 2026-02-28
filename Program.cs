using System;
using System.Globalization;
using System.Linq;

namespace MedScheduler
{
    internal static class Program
    {
        private static void Main()
        {
            var scheduler = new AppointmentScheduler();

            Console.WriteLine("=== Medical Appointment Scheduler ===");

            //User Input handled in a while loop
            bool running = true;
            while (running)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Add Appointment");
                Console.WriteLine("2. Cancel Appointment");
                Console.WriteLine("3. Reschedule Appointment");
                Console.WriteLine("4. List All Appointments");
                Console.WriteLine("5. List by Provider");
                Console.WriteLine("6. List by Day");
                Console.WriteLine("7. Exit");
                Console.Write("Choose: ");

                string? choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        AddAppointmentMenu(scheduler);
                        break;
                    case "2":
                        CancelAppointmentMenu(scheduler); break;
                    case "3":
                        RescheduleAppointmentMenu(scheduler); break;
                    case "4":
                        ListAllMenu(scheduler); break;
                    case "5":
                        ListByProviderMenu(scheduler); break;
                    case "6":
                        ListByDayMenu(scheduler); break;
                    case "7": running = false; break;

                    default: Console.WriteLine("Invalid option."); break;
                }
            }
            //Leave the user a Goodbye Message
            Console.WriteLine("\nGoodbye!");
        }

        // ---------- Menu Handlers ----------

        private static void AddAppointmentMenu(AppointmentScheduler scheduler)
        {
            try
            {
                Console.WriteLine("\n --- Add New Appointment ---");

                string id = Prompt("Enter Appointment ID: ");
                string patient = Prompt("Enter Patient Name: ");
                string provider = Prompt("Enter Provider Name: ");
                string room = Prompt("Enter Room: ");
                DateTime start = PromptDateTime("Enter Start (yyyy-MM-dd HH:mm): ");
                DateTime end = PromptDateTime("Enter End (yyyy-MM-dd HH:mm): ");

                var appt = new Appointment(id, patient, provider, start, end, room);

                scheduler.Add(appt);

                Console.WriteLine("Appointment added successfully.");
                Logger.Info($"Added [{appt.Id}] {appt.Start:HH:mm}-{appt.End:HH:mm} {appt.ProviderName} Room {appt.Room}");


            }
            catch (DoubleBookingException ex)
            {
                Console.WriteLine("Cannot add appointment: " + ex.Message);
                Logger.Warn(ex.Message);
            }
            catch (InvalidAppointmentTimeException ex)
            {
                Console.WriteLine("Invalid time: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Invalid input: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error adding appointment.");
                Logger.Error($"Error adding appointment: {ex.Message}");
            }
        }

        private static void CancelAppointmentMenu(AppointmentScheduler scheduler)
        {
            try
            {
                Console.WriteLine("\n --- Cancel Appointment ---");
                string id = Prompt("Enter Appointment ID to cancel: ");

                if (scheduler.Cancel(id))
                {
                    Console.WriteLine($"Appointment {id} cancelled successfully.");
                    Logger.Info($"Cancelled appointment {id}");

                }
                else
                {
                    Console.WriteLine($"Appointment {id} not found.");
                    Logger.Warn($"Cancel attempt failed: appointment {id} not found");

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error cancelling appointment.");
                Logger.Error($"Error cancelling appointment: {ex.Message}");

            }
        }

        private static void RescheduleAppointmentMenu(AppointmentScheduler scheduler)
        {

            try
            {
                Console.WriteLine("\n ---Reschedule Appointment ---");
                string id = Prompt("Enter Appointment ID: ");
                DateTime newStart = PromptDateTime("Enter new start (yyyy-MM-dd HH:mm): ");
                DateTime newEnd = PromptDateTime("Enter new End (yyyy-MM-dd HH:mm): ");

                scheduler.Reschedule(id, newStart, newEnd);

                Console.WriteLine($"Appointment {id} rescheduled successfully.");
                Logger.Info($"Rescheduled {id} to {newStart:yyyy-MM-dd HH:mm}-{newEnd:HH:mm}");

            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine("Appointment not found: " + ex.Message);
                Logger.Warn(ex.Message);
            }
            catch (DoubleBookingException ex)
            {
                Console.WriteLine("Cannot reschedule: " + ex.Message);
                Logger.Warn(ex.Message);

            }
            catch (InvalidAppointmentTimeException ex)
            {
                Console.WriteLine("Invalid new time: " + ex.Message);
                Logger.Warn(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Invalid input: " + ex.Message);
                Logger.Warn($"Invalid input for reschedule: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error rescheduling appointment.");
                Logger.Error($"Error rescheduling: {ex.Message}");

            }
        }

        private static void ListAllMenu(AppointmentScheduler scheduler)
        {
            var appointments = scheduler.Appointments.ToList();

            if (!appointments.Any())
            {
                Console.WriteLine("No appointments scheduled.");
                return;
            }

            Console.WriteLine("\n--- All Appointments ---");
            foreach (var appt in appointments)
            {
                Console.WriteLine(appt);
            }
        }

        private static void ListByProviderMenu(AppointmentScheduler scheduler)
        {
            string provider = Prompt("\nEnter provider name: ").Trim();

            var appts = scheduler.ListByProvider(provider).ToList();

            if (!appts.Any())
            {
                Console.WriteLine($"\nNo appointments found for {provider}.");
                return;
            }

            Console.WriteLine($"\n--- Appointments for {provider} ---");
            foreach (var appt in appts)
            {
                Console.WriteLine(appt);
            }
        }

        private static void ListByDayMenu(AppointmentScheduler scheduler)
        {
            try
            {
                DateTime day = PromptDateTime("\nEnter date (yyyy-MM-dd HH:mm will be truncated to date): ");
                day = day.Date;  // Normalize to start of day

                var appts = scheduler.ListByDay(day).ToList();

                if (!appts.Any())
                {
                    Console.WriteLine($"\nNo appointments on {day:yyyy-MM-dd}.");
                    return;
                }

                Console.WriteLine($"\n--- Appointments on {day:yyyy-MM-dd} ---");
                foreach (var appt in appts)
                {
                    Console.WriteLine(appt);
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Invalid date format: " + ex.Message);
            }
        }

        //!!! No need to modify these methods. They are there to help you
        // ---------- Helpers ----------

        private static string Prompt(string label)
        {
            Console.Write(label);
            var s = Console.ReadLine() ?? "";
            return s.Trim();
        }

        private static DateTime PromptDateTime(string label)
        {
            Console.Write(label);
            var s = Console.ReadLine();
            if (!DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out var dt))
            {
                throw new ArgumentException("Invalid date/time format. Use yyyy-MM-dd HH:mm (e.g., 2025-11-15 09:30).");
            }
            return dt;
        }
    }
}
