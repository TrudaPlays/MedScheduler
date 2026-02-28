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
                Console.WriteLine("7. Save appointments to file");
                Console.WriteLine("8. Load appointments from file");
                Console.WriteLine("9. Exit");
                Console.Write("Choose: ");

                string? choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1": AddAppointmentMenu(scheduler); break;
                    case "2": CancelAppointmentMenu(scheduler); break;
                    case "3": RescheduleAppointmentMenu(scheduler); break;
                    case "4": ListAllMenu(scheduler); break;
                    case "5": ListByProviderMenu(scheduler); break;
                    case "6": ListByDayMenu(scheduler); break;
                    case "7": SaveAppointmentsMenu(scheduler); break;
                    case "8": LoadAppointmentsMenu(scheduler); break;
                    case "9": running = false; break;
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
                    string id = GetValidString("Enter Appointment ID: ");
                    string patient = GetValidString("Enter Patient Name: ");
                    string provider = GetValidString("Enter Provider Name: ");
                    string room = GetValidString("Enter Room: ");

                    DateTime start = GetValidDateTime("Enter Start (yyyy-MM-dd HH:mm): ");
                    DateTime end = GetValidDateTime("Enter End (yyyy-MM-dd HH:mm): ");

                    var appt = new Appointment(id, patient, provider, start, end, room);
                    scheduler.Add(appt);

                    Console.WriteLine("\nAppointment added successfully.");
                    return;  // success → back to main menu
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("\nAdd operation cancelled.");
                    return;
                }
                catch (DoubleBookingException ex)
                {
                    Console.WriteLine("\nCannot add appointment: " + ex.Message);
                    Logger.Warn(ex.Message);
                    // loop continues → try again
                }
                catch (InvalidAppointmentTimeException ex)
                {
                    Console.WriteLine("\nInvalid time: " + ex.Message);
                    Logger.Warn(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("\nInvalid input: " + ex.Message);
                    Logger.Warn($"Invalid input for new appointment: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nUnexpected error adding appointment. Please try again.");
                    Logger.Error($"Error adding appointment: {ex.Message}");
                }
            }
        }

        // Helper: reusable non-throwing string input with cancel support
        private static string GetValidString(string label)
        {
            while (true)
            {
                string input = Prompt(label).Trim();
                if (input.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                    throw new OperationCanceledException();

                if (!string.IsNullOrWhiteSpace(input))
                    return input;

                Console.WriteLine("Input cannot be empty. Try again or type 'cancel'.");
            }
        }

        // Helper: reusable date/time input with cancel support (no exception on cancel)
        private static DateTime GetValidDateTime(string label)
        {
            while (true)
            {
                Console.Write(label);
                string input = Console.ReadLine()?.Trim() ?? "";

                if (input.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                    throw new OperationCanceledException();

                if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    return dt;
                }

                Console.WriteLine("Invalid format. Use yyyy-MM-dd HH:mm (e.g. 2025-11-15 09:30). Try again or type 'cancel'.");
            }
        }

        private static void CancelAppointmentMenu(AppointmentScheduler scheduler)
        {
            while (true)
            {
                Console.WriteLine("\n--- Cancel Appointment ---");
                Console.WriteLine("(type 'cancel' to return to main menu)\n");

                try
                {
                    string id = GetValidString("Enter Appointment ID to cancel: ");

                    if (scheduler.Cancel(id))
                    {
                        Console.WriteLine($"\nAppointment {id} cancelled successfully.");
                        Logger.Info($"Cancelled appointment {id}");
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"\nAppointment '{id}' not found. Try again.");
                        Logger.Warn($"Cancel attempt failed: appointment '{id}' not found");
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("\nCancel operation cancelled.");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nError while trying to cancel appointment.");
                    Logger.Error($"Error cancelling appointment: {ex.Message}");
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
                    string id = GetValidString("Enter Appointment ID: ");

                    DateTime newStart = GetValidDateTime("Enter new Start (yyyy-MM-dd HH:mm): ");
                    DateTime newEnd = GetValidDateTime("Enter new End (yyyy-MM-dd HH:mm): ");

                    scheduler.Reschedule(id, newStart, newEnd);

                    Console.WriteLine($"\nAppointment {id} rescheduled successfully.");
                    return;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("\nReschedule operation cancelled.");
                    return;
                }
                catch (KeyNotFoundException ex)
                {
                    Console.WriteLine("\nAppointment not found: " + ex.Message);
                    Logger.Warn(ex.Message);
                }
                catch (DoubleBookingException ex)
                {
                    Console.WriteLine("\nCannot reschedule: " + ex.Message);
                    Logger.Warn(ex.Message);
                }
                catch (InvalidAppointmentTimeException ex)
                {
                    Console.WriteLine("\nInvalid new time: " + ex.Message);
                    Logger.Warn(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("\nInvalid input: " + ex.Message);
                    Logger.Warn($"Invalid input for reschedule: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nUnexpected error rescheduling appointment. Please try again.");
                    Logger.Error($"Error rescheduling: {ex.Message}");
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

        private static readonly string AppointmentsFile = "appointments.txt";

        private static void SaveAppointmentsMenu(AppointmentScheduler scheduler)
        {
            try
            {
                using var writer = new StreamWriter(AppointmentsFile);

                foreach (var appt in scheduler.Appointments)
                {
                    writer.WriteLine(
                        $"{appt.Id}|" +
                        $"{EscapePipe(appt.PatientName)}|" +
                        $"{EscapePipe(appt.ProviderName)}|" +
                        $"{appt.Start:yyyy-MM-dd HH:mm}|" +
                        $"{appt.End:yyyy-MM-dd HH:mm}|" +
                        $"{EscapePipe(appt.Room)}"
                    );
                }

                Console.WriteLine($"\nSaved {scheduler.Appointments.Count} appointments to {AppointmentsFile}");
                Logger.Info($"Saved {scheduler.Appointments.Count} appointments to file");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError saving appointments: {ex.Message}");
                Logger.Error($"Failed to save appointments: {ex.Message}");
            }
        }

        private static void LoadAppointmentsMenu(AppointmentScheduler scheduler)
        {
            if (!File.Exists(AppointmentsFile))
            {
                Console.WriteLine($"\nFile {AppointmentsFile} not found.");
                return;
            }

            try
            {
                var lines = File.ReadAllLines(AppointmentsFile);
                int loaded = 0;
                int skipped = 0;

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split('|');
                    if (parts.Length != 6)
                    {
                        skipped++;
                        continue;
                    }

                    try
                    {
                        string id = parts[0].Trim();
                        string patient = UnescapePipe(parts[1].Trim());
                        string provider = UnescapePipe(parts[2].Trim());
                        DateTime start = DateTime.ParseExact(parts[3].Trim(), "yyyy-MM-dd HH:mm", null);
                        DateTime end = DateTime.ParseExact(parts[4].Trim(), "yyyy-MM-dd HH:mm", null);
                        string room = UnescapePipe(parts[5].Trim());

                        var appt = new Appointment(id, patient, provider, start, end, room);
                        scheduler.Add(appt);   // will validate rules & skip if conflict
                        loaded++;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Skipped invalid line in file: {line} → {ex.Message}");
                        skipped++;
                    }
                }

                Console.WriteLine($"\nLoaded {loaded} appointments from {AppointmentsFile}.");
                if (skipped > 0)
                    Console.WriteLine($"{skipped} lines were skipped (invalid format or conflicts).");

                Logger.Info($"Loaded {loaded} appointments from file (skipped {skipped})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError loading appointments: {ex.Message}");
                Logger.Error($"Failed to load appointments: {ex.Message}");
            }
        }

        // Helpers to handle names/rooms that might contain | character
        private static string EscapePipe(string s) => s.Replace("|", "\\|");
        private static string UnescapePipe(string s) => s.Replace("\\|", "|");

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
