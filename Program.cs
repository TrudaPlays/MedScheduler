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
            while (true)
            {
                Console.WriteLine("\n--- Add New Appointment ---");
                Console.WriteLine("(type 'cancel' at any prompt to return to menu)\n");

                try
                {
                    string id = Prompt("Enter Appointment ID: ").Trim();
                    if (id.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Add operation cancelled.");
                        return;
                    }

                    string patient = Prompt("Enter Patient Name: ").Trim();
                    if (patient.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Add operation cancelled.");
                        return;
                    }

                    string provider = Prompt("Enter Provider Name: ").Trim();
                    if (provider.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Add operation cancelled.");
                        return;
                    }

                    string room = Prompt("Enter Room: ").Trim();
                    if (room.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Add operation cancelled.");
                        return;
                    }

                    DateTime start;
                    while (true)
                    {
                        try
                        {
                            start = PromptDateTime("Enter Start (yyyy-MM-dd HH:mm): ");
                            break;
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine("Invalid format. Use yyyy-MM-dd HH:mm (e.g. 2025-11-15 09:30). Try again.");
                        }
                    }
                    if (start == DateTime.MinValue) // This won't happen, just defensive
                        continue;

                    DateTime end;
                    while (true)
                    {
                        try
                        {
                            end = PromptDateTime("Enter End (yyyy-MM-dd HH:mm): ");
                            break;
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine("Invalid format. Use yyyy-MM-dd HH:mm (e.g. 2025-11-15 09:30). Try again.");
                        }
                    }

                    var appt = new Appointment(id, patient, provider, start, end, room);

                    scheduler.Add(appt);

                    Console.WriteLine("\nAppointment added successfully.");
                    return;  // ← success → back to main menu
                }
                catch (OperationCanceledException) // if you decide to throw it later
                {
                    Console.WriteLine("Add operation cancelled.");
                    return;
                }
                catch (DoubleBookingException ex)
                {
                    Console.WriteLine("\nCannot add appointment: " + ex.Message);
                    Logger.Warn(ex.Message);
                    // loop continues → ask again
                }
                catch (InvalidAppointmentTimeException ex)
                {
                    Console.WriteLine("\nInvalid time: " + ex.Message);
                    Logger.Warn(ex.Message);
                    // loop continues
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("\nInvalid input: " + ex.Message);
                    Logger.Warn($"Invalid input for new appointment: {ex.Message}");
                    // loop continues
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nUnexpected error adding appointment. Please try again.");
                    Logger.Error($"Error adding appointment: {ex.Message}");
                    // loop continues
                }
            }
        }

        private static void CancelAppointmentMenu(AppointmentScheduler scheduler)
        {
            while (true)
            {
                Console.WriteLine("\n--- Cancel Appointment ---");
                Console.WriteLine("(type 'cancel' to return to main menu)\n");

                string input = Prompt("Enter Appointment ID to cancel: ").Trim();

                if (input.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Cancel operation cancelled.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("ID cannot be empty. Please enter a valid ID or type 'cancel'.");
                    continue;
                }

                try
                {
                    if (scheduler.Cancel(input))
                    {
                        Console.WriteLine($"\nAppointment {input} cancelled successfully.");
                        Logger.Info($"Cancelled appointment {input}");
                        return;  // success → back to main menu
                    }
                    else
                    {
                        Console.WriteLine($"\nAppointment '{input}' not found.");
                        Logger.Warn($"Cancel attempt failed: appointment '{input}' not found");
                        // loop continues → ask again
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nError while trying to cancel appointment.");
                    Logger.Error($"Error cancelling appointment '{input}': {ex.Message}");
                    // loop continues
                }
            }
        }

        private static void RescheduleAppointmentMenu(AppointmentScheduler scheduler)
        {
            while (true)
            {
                Console.WriteLine("\n--- Reschedule Appointment ---");
                Console.WriteLine("(type 'cancel' at any prompt to return to menu)\n");

                try
                {
                    string id = Prompt("Enter Appointment ID: ").Trim();
                    if (id.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Reschedule operation cancelled.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        Console.WriteLine("ID cannot be empty. Try again.");
                        continue;
                    }

                    DateTime newStart;
                    while (true)
                    {
                        try
                        {
                            newStart = PromptDateTime("Enter new Start (yyyy-MM-dd HH:mm): ");
                            break;
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine("Invalid format. Use yyyy-MM-dd HH:mm. Try again.");
                        }
                    }

                    DateTime newEnd;
                    while (true)
                    {
                        try
                        {
                            newEnd = PromptDateTime("Enter new End (yyyy-MM-dd HH:mm): ");
                            break;
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine("Invalid format. Use yyyy-MM-dd HH:mm. Try again.");
                        }
                    }

                    scheduler.Reschedule(id, newStart, newEnd);

                    Console.WriteLine($"\nAppointment {id} rescheduled successfully.");
                    return;  // ← success → back to main menu
                }
                catch (KeyNotFoundException ex)
                {
                    Console.WriteLine("\nAppointment not found: " + ex.Message);
                    Logger.Warn(ex.Message);
                    // loop continues
                }
                catch (DoubleBookingException ex)
                {
                    Console.WriteLine("\nCannot reschedule: " + ex.Message);
                    Logger.Warn(ex.Message);
                    // loop continues
                }
                catch (InvalidAppointmentTimeException ex)
                {
                    Console.WriteLine("\nInvalid new time: " + ex.Message);
                    Logger.Warn(ex.Message);
                    // loop continues
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("\nInvalid input: " + ex.Message);
                    Logger.Warn($"Invalid input for reschedule: {ex.Message}");
                    // loop continues
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nUnexpected error rescheduling appointment. Please try again.");
                    Logger.Error($"Error rescheduling: {ex.Message}");
                    // loop continues
                }
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
