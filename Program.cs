using System;
using System.Globalization;
using System.Linq;

namespace MedScheduler
{
    internal static class Program
    {
        private static void Main()
        {
            //Create a var of your AppointmentScheduler class and initialize as new()

            //Boilerplate to welcome the end user
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

                switch ((Console.ReadLine() ?? "").Trim())
                {

                    //For Cases 1-6, call the appropriate method
                    //Every case needs a break; statement

                    case "7": running = false; break;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
            //Leave the user a Goodbye Message
            Console.WriteLine("Goodbye!");
        }

        // ---------- Menus ----------

        private static void AddAppointmentMenu(AppointmentScheduler scheduler)
        {
            //Make a try catch block that uses Prompt() to get the user for all appointment information
            //In the try block, create the appointment and report if succesful
            //If it fails, create a catch exception for: 
            //DoubleBookingException
            //InvalidAppointmentTimeException
            //ArgumentException
            //Exception (the catch all for anything else)

        }

        private static void CancelAppointmentMenu(AppointmentScheduler scheduler)
        {
            //Create a Try Catch block that prompts a user for an ID
            //Inside the try block, check if the appointment exists and remove it
            //If it doesn't exist, report that to the user and log it
            //In the catch block, log an exception and write to the console that there was an error


        }

        private static void RescheduleAppointmentMenu(AppointmentScheduler scheduler)
        {
            //Make a try catch block that uses Prompt() to get the user for an appointment ID
            //Get the new start and the new end, then attempt to use reschedule. Report if succesful
            //If it fails, create a catch exception for: 
            //KeyNotFound
            //DoubleBookingException
            //InvalidAppointmentTimeException
            //ArgumentException
            //Exception (the catch all for anything else)

        }

        private static void ListAllMenu(AppointmentScheduler scheduler)
        {
            //convert all of the appointments into the scheduler into a list and store this in a var
            //If the list is empty, report this to the user and return

            //If not, use a foreach to write the contents of the list below this line-
            Console.WriteLine("\n--- All Appointments ---");

        }

        private static void ListByProviderMenu(AppointmentScheduler scheduler)
        {
            //Use Prompt() to get the provider's name from the end user
            //Then store the list by provider in a var
            //If the var is empty, report this to the end user

            //If not, use a foreach to write all appointments below this line-
            Console.WriteLine($"\n--- Appointments for {provider} ---");

        }

        private static void ListByDayMenu(AppointmentScheduler scheduler)
        {
            //Use the PromptDateTime method and store the value in a var. Use .Date to get the day
            //Store the value of ListByDay(day).ToList() in a var
            //Report if there are no dates that day and return


            //If not, use a foreach to write the list of all appointments below this line-
            Console.WriteLine($"\n--- Appointments on {day:yyyy-MM-dd} ---");

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
