using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HospitalManagementSystem
{
    // --- MAIN PROGRAM ---
    // This is the entry point and main controller of the application.
    class Program
    {
        // In-memory database using lists. This can be replaced with a real database later.
        static readonly List<User> users = new List<User>();
        static readonly List<Patient> patients = new List<Patient>();
        static readonly List<Doctor> doctors = new List<Doctor>();
        static readonly List<Appointment> appointments = new List<Appointment>();
        static User loggedInUser = null;

        static void Main(string[] args)
        {
            SetupInitialData();
            Console.Title = "Gemini Hospital Management System";

            while (true)
            {
                if (loggedInUser == null)
                {
                    Login();
                }
                else
                {
                    ShowMainMenu();
                }
            }
        }

        // --- AUTHENTICATION ---
        static void Login()
        {
            Console.Clear();
            Console.WriteLine("=============================================");
            Console.WriteLine("   HOSPITAL MANAGEMENT SYSTEM - LOGIN");
            Console.WriteLine("=============================================");
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = ReadPassword(); // Mask password input
            Console.WriteLine();

            // Find user in our "database"
            loggedInUser = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password);

            if (loggedInUser != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nWelcome, {loggedInUser.FullName} ({loggedInUser.Role})!");
                Console.ResetColor();
                Thread.Sleep(2000);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nInvalid username or password. Please try again.");
                Console.ResetColor();
                Thread.Sleep(2000);
            }
        }

        static void Logout()
        {
            Console.WriteLine("\nLogging out...");
            Thread.Sleep(1000);
            loggedInUser = null;
        }

        // --- MAIN MENU ---
        static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("=============================================");
            Console.WriteLine($"   Welcome, {loggedInUser.FullName} | Role: {loggedInUser.Role}");
            Console.WriteLine("=============================================");
            Console.WriteLine("Please select an option:");

            // Display options based on user role
            if (loggedInUser.Role == UserRole.Admin || loggedInUser.Role == UserRole.Receptionist)
            {
                Console.WriteLine("1. Patient Management");
            }
            if (loggedInUser.Role == UserRole.Admin)
            {
                Console.WriteLine("2. Doctor Management");
            }
            if (loggedInUser.Role == UserRole.Admin || loggedInUser.Role == UserRole.Receptionist)
            {
                Console.WriteLine("3. Appointment Management");
            }
            // Add more options for other roles here...

            Console.WriteLine("9. Logout");
            Console.WriteLine("0. Exit Application");
            Console.WriteLine("---------------------------------------------");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    if (loggedInUser.Role == UserRole.Admin || loggedInUser.Role == UserRole.Receptionist)
                        PatientManagement.ShowMenu(patients);
                    else
                        ShowAccessDenied();
                    break;
                case "2":
                    if (loggedInUser.Role == UserRole.Admin)
                        DoctorManagement.ShowMenu(doctors);
                    else
                        ShowAccessDenied();
                    break;
                case "3":
                    if (loggedInUser.Role == UserRole.Admin || loggedInUser.Role == UserRole.Receptionist)
                        AppointmentManagement.ShowMenu(appointments, patients, doctors);
                    else
                        ShowAccessDenied();
                    break;
                case "9":
                    Logout();
                    break;
                case "0":
                    Console.WriteLine("\nThank you for using the Hospital Management System. Exiting...");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\nInvalid choice. Please try again.");
                    Pause();
                    break;
            }
        }

        // --- UTILITY METHODS ---
        static void SetupInitialData()
        {
            // Add default users for testing
            users.Add(new User("admin", "pass123", "Admin User", UserRole.Admin));
            users.Add(new User("reception", "pass123", "Receptionist User", UserRole.Receptionist));
            users.Add(new User("doctor", "pass123", "Dr. Evelyn Reed", UserRole.Doctor));

            // Add sample doctors
            doctors.Add(new Doctor("D001", "Dr. Evelyn Reed", "Cardiology"));
            doctors.Add(new Doctor("D002", "Dr. Marcus Chen", "Neurology"));

            // Add sample patients
            var p1 = new Patient("P001", "John Doe", new DateTime(1985, 5, 20), "123-456-7890");
            p1.MedicalHistory.Add("2023-01-15: Annual Checkup. All vitals normal.");
            patients.Add(p1);

            var p2 = new Patient("P002", "Jane Smith", new DateTime(1992, 8, 15), "987-654-3210");
            p2.MedicalHistory.Add("2023-03-22: Presented with cough. Diagnosed with bronchitis.");
            patients.Add(p2);

            // Add sample appointments
            appointments.Add(new Appointment("A001", new DateTime(2025, 6, 10, 10, 0, 0), "P001", "D001", "Follow-up consultation"));
        }

        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            } while (key.Key != ConsoleKey.Enter);
            return password;
        }

        public static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void ShowAccessDenied()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nACCESS DENIED: Your role does not have permission for this module.");
            Console.ResetColor();
            Pause();
        }
    }

    // --- MODULES ---

    public static class PatientManagement
    {
        public static void ShowMenu(List<Patient> patients)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=============================================");
                Console.WriteLine("           PATIENT MANAGEMENT");
                Console.WriteLine("=============================================");
                Console.WriteLine("1. Register New Patient");
                Console.WriteLine("2. View All Patients");
                Console.WriteLine("3. Update Patient Details");
                Console.WriteLine("4. View/Add Patient Medical History");
                Console.WriteLine("9. Back to Main Menu");
                Console.WriteLine("---------------------------------------------");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        RegisterNewPatient(patients);
                        break;
                    case "2":
                        ViewAllPatients(patients);
                        break;
                    case "3":
                        UpdatePatientDetails(patients);
                        break;
                    case "4":
                        ManageMedicalHistory(patients);
                        break;
                    case "9":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        Program.Pause();
                        break;
                }
            }
        }

        private static void RegisterNewPatient(List<Patient> patients)
        {
            Console.Clear();
            Console.WriteLine("--- Register New Patient ---");
            try
            {
                Console.Write("Enter Full Name: ");
                string name = Console.ReadLine();

                Console.Write("Enter Date of Birth (YYYY-MM-DD): ");
                DateTime dob = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter Contact Number: ");
                string contact = Console.ReadLine();

                string newId = "P" + (patients.Count + 1).ToString("D3");

                Patient newPatient = new Patient(newId, name, dob, contact);
                patients.Add(newPatient);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nPatient '{name}' registered successfully with ID: {newId}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError registering patient: {ex.Message}. Please check your input.");
                Console.ResetColor();
            }
            Program.Pause();
        }

        private static void ViewAllPatients(List<Patient> patients)
        {
            Console.Clear();
            Console.WriteLine("--- All Registered Patients ---");
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-7} | {"Full Name",-25} | {"Date of Birth",-15} | {"Contact",-15}");
            Console.WriteLine("----------------------------------------------------------------------");

            if (patients.Count == 0)
            {
                Console.WriteLine("No patients registered yet.");
            }
            else
            {
                foreach (var patient in patients)
                {
                    Console.WriteLine($"{patient.Id,-7} | {patient.FullName,-25} | {patient.DateOfBirth.ToShortDateString(),-15} | {patient.ContactNumber,-15}");
                }
            }
            Console.WriteLine("----------------------------------------------------------------------");
            Program.Pause();
        }

        private static void UpdatePatientDetails(List<Patient> patients)
        {
            Console.Clear();
            Console.WriteLine("--- Update Patient Details ---");
            Console.Write("Enter Patient ID to update (e.g., P001): ");
            string id = Console.ReadLine();

            Patient patientToUpdate = patients.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (patientToUpdate == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nPatient not found.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"\nUpdating details for {patientToUpdate.FullName} (ID: {patientToUpdate.Id})");
                Console.WriteLine("Press ENTER to keep the current value.");

                Console.Write($"Full Name ({patientToUpdate.FullName}): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    patientToUpdate.FullName = newName;
                }

                Console.Write($"Contact Number ({patientToUpdate.ContactNumber}): ");
                string newContact = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newContact))
                {
                    patientToUpdate.ContactNumber = newContact;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nPatient details updated successfully.");
                Console.ResetColor();
            }
            Program.Pause();
        }

        private static void ManageMedicalHistory(List<Patient> patients)
        {
            Console.Clear();
            Console.WriteLine("--- Patient Medical History ---");
            Console.Write("Enter Patient ID to view/add history (e.g., P001): ");
            string id = Console.ReadLine();

            Patient patient = patients.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (patient == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nPatient not found.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"\n--- History for {patient.FullName} ---");
                if (patient.MedicalHistory.Any())
                {
                    foreach (var entry in patient.MedicalHistory)
                    {
                        Console.WriteLine($"- {entry}");
                    }
                }
                else
                {
                    Console.WriteLine("No medical history recorded.");
                }
                Console.WriteLine("-------------------------------------");

                Console.Write("Do you want to add a new history entry? (y/n): ");
                if (Console.ReadLine().Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write("Enter new history note: ");
                    string newNote = Console.ReadLine();
                    patient.MedicalHistory.Add($"{DateTime.Now:yyyy-MM-dd HH:mm}: {newNote}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nHistory entry added.");
                    Console.ResetColor();
                }
            }
            Program.Pause();
        }
    }

    public static class DoctorManagement
    {
        public static void ShowMenu(List<Doctor> doctors)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=============================================");
                Console.WriteLine("            DOCTOR MANAGEMENT");
                Console.WriteLine("=============================================");
                Console.WriteLine("1. Add New Doctor");
                Console.WriteLine("2. View All Doctors");
                Console.WriteLine("3. Update Doctor Details");
                Console.WriteLine("9. Back to Main Menu");
                Console.WriteLine("---------------------------------------------");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddDoctor(doctors);
                        break;
                    case "2":
                        ViewAllDoctors(doctors);
                        break;
                    case "3":
                        UpdateDoctor(doctors);
                        break;
                    case "9":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        Program.Pause();
                        break;
                }
            }
        }

        private static void AddDoctor(List<Doctor> doctors)
        {
            Console.Clear();
            Console.WriteLine("--- Add New Doctor ---");
            try
            {
                Console.Write("Enter Full Name (e.g., Dr. John Smith): ");
                string name = Console.ReadLine();

                Console.Write("Enter Specialty (e.g., Cardiology): ");
                string specialty = Console.ReadLine();

                string newId = "D" + (doctors.Count + 1).ToString("D3");

                doctors.Add(new Doctor(newId, name, specialty));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nDr. '{name}' added successfully with ID: {newId}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError adding doctor: {ex.Message}. Please check your input.");
                Console.ResetColor();
            }
            Program.Pause();
        }

        private static void ViewAllDoctors(List<Doctor> doctors)
        {
            Console.Clear();
            Console.WriteLine("--- All Doctors ---");
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-7} | {"Full Name",-25} | {"Specialty",-20}");
            Console.WriteLine("----------------------------------------------------------------------");

            if (doctors.Any())
            {
                foreach (var doc in doctors)
                {
                    Console.WriteLine($"{doc.Id,-7} | {doc.FullName,-25} | {doc.Specialty,-20}");
                }
            }
            else
            {
                Console.WriteLine("No doctors found in the system.");
            }
            Console.WriteLine("----------------------------------------------------------------------");
            Program.Pause();
        }

        private static void UpdateDoctor(List<Doctor> doctors)
        {
            Console.Clear();
            Console.WriteLine("--- Update Doctor Details ---");
            Console.Write("Enter Doctor ID to update (e.g., D001): ");
            string id = Console.ReadLine();

            Doctor doctorToUpdate = doctors.FirstOrDefault(d => d.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (doctorToUpdate == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nDoctor not found.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"\nUpdating details for {doctorToUpdate.FullName} (ID: {doctorToUpdate.Id})");
                Console.WriteLine("Press ENTER to keep the current value.");

                Console.Write($"Full Name ({doctorToUpdate.FullName}): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    doctorToUpdate.FullName = newName;
                }

                Console.Write($"Specialty ({doctorToUpdate.Specialty}): ");
                string newSpecialty = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newSpecialty))
                {
                    doctorToUpdate.Specialty = newSpecialty;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nDoctor details updated successfully.");
                Console.ResetColor();
            }
            Program.Pause();
        }
    }

    public static class AppointmentManagement
    {
        public static void ShowMenu(List<Appointment> appointments, List<Patient> patients, List<Doctor> doctors)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=============================================");
                Console.WriteLine("         APPOINTMENT MANAGEMENT");
                Console.WriteLine("=============================================");
                Console.WriteLine("1. Book New Appointment");
                Console.WriteLine("2. View All Appointments");
                Console.WriteLine("3. Cancel Appointment");
                Console.WriteLine("9. Back to Main Menu");
                Console.WriteLine("---------------------------------------------");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        BookAppointment(appointments, patients, doctors);
                        break;
                    case "2":
                        ViewAllAppointments(appointments, patients, doctors);
                        break;
                    case "3":
                        CancelAppointment(appointments);
                        break;
                    case "9":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        Program.Pause();
                        break;
                }
            }
        }

        private static void BookAppointment(List<Appointment> appointments, List<Patient> patients, List<Doctor> doctors)
        {
            Console.Clear();
            Console.WriteLine("--- Book New Appointment ---");

            // Select Patient
            Console.WriteLine("\n--- Select Patient ---");
            foreach (var p in patients) { Console.WriteLine($"{p.Id}: {p.FullName}"); }
            Console.Write("Enter Patient ID: ");
            string patientId = Console.ReadLine();
            var patient = patients.FirstOrDefault(p => p.Id.Equals(patientId, StringComparison.OrdinalIgnoreCase));
            if (patient == null) { Console.WriteLine("Invalid Patient ID."); Program.Pause(); return; }

            // Select Doctor
            Console.WriteLine("\n--- Select Doctor ---");
            foreach (var d in doctors) { Console.WriteLine($"{d.Id}: {d.FullName} ({d.Specialty})"); }
            Console.Write("Enter Doctor ID: ");
            string doctorId = Console.ReadLine();
            var doctor = doctors.FirstOrDefault(d => d.Id.Equals(doctorId, StringComparison.OrdinalIgnoreCase));
            if (doctor == null) { Console.WriteLine("Invalid Doctor ID."); Program.Pause(); return; }

            try
            {
                Console.Write("\nEnter Appointment Date & Time (YYYY-MM-DD HH:mm): ");
                DateTime apptDateTime = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter Reason for Appointment: ");
                string reason = Console.ReadLine();

                string newId = "A" + (appointments.Count + 1).ToString("D3");
                appointments.Add(new Appointment(newId, apptDateTime, patientId, doctorId, reason));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nAppointment booked successfully!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError booking appointment: {ex.Message}");
                Console.ResetColor();
            }
            Program.Pause();
        }

        private static void ViewAllAppointments(List<Appointment> appointments, List<Patient> patients, List<Doctor> doctors)
        {
            Console.Clear();
            Console.WriteLine("--- All Appointments ---");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-7} | {"Date & Time",-20} | {"Patient Name",-25} | {"Doctor Name",-25} | {"Reason"}");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------");

            if (appointments.Any())
            {
                foreach (var appt in appointments)
                {
                    var patientName = patients.FirstOrDefault(p => p.Id == appt.PatientId)?.FullName ?? "N/A";
                    var doctorName = doctors.FirstOrDefault(d => d.Id == appt.DoctorId)?.FullName ?? "N/A";
                    Console.WriteLine($"{appt.Id,-7} | {appt.AppointmentDateTime,-20:yyyy-MM-dd HH:mm} | {patientName,-25} | {doctorName,-25} | {appt.Reason}");
                }
            }
            else
            {
                Console.WriteLine("No appointments scheduled.");
            }
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------");
            Program.Pause();
        }

        private static void CancelAppointment(List<Appointment> appointments)
        {
            Console.Clear();
            Console.WriteLine("--- Cancel Appointment ---");
            Console.Write("Enter Appointment ID to cancel (e.g., A001): ");
            string id = Console.ReadLine();

            var apptToRemove = appointments.FirstOrDefault(a => a.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (apptToRemove != null)
            {
                appointments.Remove(apptToRemove);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nAppointment cancelled successfully.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nAppointment not found.");
                Console.ResetColor();
            }
            Program.Pause();
        }
    }


    // --- ENTITY CLASSES ---

    public enum UserRole
    {
        Admin,
        Doctor,
        Receptionist,
        Pharmacist,
        Accountant
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public UserRole Role { get; set; }

        public User(string username, string password, string fullName, UserRole role)
        {
            Username = username;
            Password = password;
            FullName = fullName;
            Role = role;
        }
    }

    public class Patient
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ContactNumber { get; set; }
        public List<string> MedicalHistory { get; set; }

        public Patient(string id, string fullName, DateTime dob, string contact)
        {
            Id = id;
            FullName = fullName;
            DateOfBirth = dob;
            ContactNumber = contact;
            MedicalHistory = new List<string>();
        }
    }

    public class Doctor
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Specialty { get; set; }

        public Doctor(string id, string fullName, string specialty)
        {
            Id = id;
            FullName = fullName;
            Specialty = specialty;
        }
    }

    public class Appointment
    {
        public string Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public string Reason { get; set; }

        public Appointment(string id, DateTime dt, string patientId, string doctorId, string reason)
        {
            Id = id;
            AppointmentDateTime = dt;
            PatientId = patientId;
            DoctorId = doctorId;
            Reason = reason;
        }
    }
}
