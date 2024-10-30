using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RideSharingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            RideSharingSystem rideSharingSystem = new RideSharingSystem();
            rideSharingSystem.Start();
        }
    }

    class RideSharingSystem
    {
        private List<Rider> registeredRiders = new List<Rider>();
        private List<Driver> registeredDrivers = new List<Driver>();
        private List<Trip> availableTrips = new List<Trip>();
        private int tripCounter = 1; 

        public void Start()
        {
            while (true)
            {
                Console.WriteLine("Welcome to the Ride-Sharing System!");
                Console.WriteLine("User Menu:");
                Console.WriteLine("1. Register as Rider");
                Console.WriteLine("2. Register as Driver");
                Console.WriteLine("3. Request a Ride (Riders only)");
                Console.WriteLine("4. Accept a Ride (Drivers only)");
                Console.WriteLine("5. Complete a Trip");
                Console.WriteLine("6. View Ride History (Riders only)");
                Console.WriteLine("7. View Trip History (Drivers only)");
                Console.WriteLine("8. Display All Trips");
                Console.WriteLine("9. Exit");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterUser(isRider: true);
                        break;
                    case "2":
                        RegisterUser(isRider: false);
                        break;
                    case "3":
                        
                        if (registeredRiders.Count == 0) Console.WriteLine("No registered riders.");
                        else RequestRide();
                        break;
                    case "4":
                       
                        if (registeredDrivers.Count == 0) Console.WriteLine("No registered drivers.");
                        else AcceptRide();
                        break;
                    case "5":
                        CompleteTrip();
                        break;
                    case "6":
                        ViewRideHistory();
                        break;
                    case "7":
                        ViewTripHistory();
                        break;
                    case "8":
                        DisplayAllTrips();
                        break;
                    case "9":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        public void RegisterUser(bool isRider)
        {
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Phone Number (10 digits): ");
            string phoneNumber = Console.ReadLine();

            while (!IsValidPhoneNumber(phoneNumber))
            {
                Console.Write("Invalid phone number. Please enter again (10 digits): ");
                phoneNumber = Console.ReadLine();
            }

            if (isRider)
            {
                Rider rider = new Rider(name, phoneNumber);
                registeredRiders.Add(rider);
                Console.WriteLine("Rider registered successfully!");
            }
            else
            {
                Driver driver = new Driver(name, phoneNumber);
                registeredDrivers.Add(driver);
                Console.WriteLine("Driver registered successfully!");
            }
        }

        private void RequestRide()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            var rider = registeredRiders.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (rider != null)
            {
                rider.RequestRide(availableTrips, ref tripCounter);
            }
            else
            {
                Console.WriteLine("Rider not found.");
            }
        }

        private void AcceptRide()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            var driver = registeredDrivers.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (driver != null)
            {
                driver.AcceptRide(availableTrips);
            }
            else
            {
                Console.WriteLine("Driver not found.");
            }
        }

        private void CompleteTrip()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            var rider = registeredRiders.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            var driver = registeredDrivers.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (rider != null)
            {
                rider.CompleteTrip(availableTrips);
            }
            else if (driver != null)
            {
                driver.CompleteTrip(availableTrips);
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        private void ViewRideHistory()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            var rider = registeredRiders.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (rider != null)
            {
                rider.ViewRideHistory();
            }
            else
            {
                Console.WriteLine("Rider not found.");
            }
        }

        private void ViewTripHistory()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            var driver = registeredDrivers.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (driver != null)
            {
                driver.ViewTripHistory();
            }
            else
            {
                Console.WriteLine("Driver not found.");
            }
        }

        private void DisplayAllTrips()
        {
            if (availableTrips.Count == 0)
            {
                Console.WriteLine("No trips available.");
                return;
            }

            foreach (var trip in availableTrips)
            {
                trip.DisplayTripDetails();
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            string pattern = @"^\d{10}$"; 
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }

    class User
    {
        public string UserID { get; private set; }
        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }

        protected User(string name, string phoneNumber)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            UserID = Guid.NewGuid().ToString();
        }

        public void DisplayProfile()
        {
            Console.WriteLine($"Name: {Name}, Phone: {PhoneNumber}");
        }
    }

    class Rider : User
    {
        private List<Trip> rideHistory = new List<Trip>();

        public Rider(string name, string phoneNumber) : base(name, phoneNumber) { }

        public void RequestRide(List<Trip> availableTrips, ref int tripCounter)
        {
            Console.Write("Enter Start Location: ");
            string startLocation = Console.ReadLine();
            Console.Write("Enter Destination: ");
            string destination = Console.ReadLine();

            Trip newTrip = new Trip(tripCounter++, this.Name, null, startLocation, destination);
            availableTrips.Add(newTrip);
            Console.WriteLine("Ride requested successfully!");
        }

        public void CompleteTrip(List<Trip> availableTrips)
        {
            Console.Write("Enter Trip ID to complete: ");
            string tripId = Console.ReadLine();
            if (int.TryParse(tripId, out int tripIdInt))
            {
                var trip = availableTrips.FirstOrDefault(t => t.TripID == tripIdInt);
                if (trip != null && trip.Status == TripStatus.InProgress)
                {
                    trip.EndTrip();
                    rideHistory.Add(trip);
                    availableTrips.Remove(trip);
                    Console.WriteLine("Trip completed successfully!");
                }
                else
                {
                    Console.WriteLine("Invalid Trip ID or Trip not in progress.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Trip ID format.");
            }
        }

        public void ViewRideHistory()
        {
            Console.WriteLine("Ride History:");
            foreach (var trip in rideHistory)
            {
                trip.DisplayTripDetails();
            }
        }
    }

    class Driver : User
    {
        public string DriverID { get; private set; }
        public string VehicleDetails { get; set; }
        public bool IsAvailable { get; private set; } = true;
        private List<Trip> tripHistory = new List<Trip>();

        public Driver(string name, string phoneNumber) : base(name, phoneNumber)
        {
            DriverID = Guid.NewGuid().ToString();
        }

        public void AcceptRide(List<Trip> availableTrips)
        {
            if (!IsAvailable)
            {
                Console.WriteLine("Driver is not available.");
                return;
            }

            if (availableTrips.Count == 0)
            {
                Console.WriteLine("No ride requests available.");
                return;
            }

            Console.WriteLine("Available Ride Requests:");
            foreach (var trip in availableTrips)
            {
                Console.WriteLine($"Trip ID: {trip.TripID}, From: {trip.StartLocation}, To: {trip.Destination}");
            }

            Console.Write("Enter Trip ID to accept: ");
            string tripId = Console.ReadLine();
            if (int.TryParse(tripId, out int tripIdInt))
            {
                var trip = availableTrips.FirstOrDefault(t => t.TripID == tripIdInt);
                if (trip != null)
                {
                    trip.DriverName = this.Name;
                    trip.StartTrip();
                    IsAvailable = false;
                    tripHistory.Add(trip);
                    Console.WriteLine("Ride accepted successfully!");
                }
                else
                {
                    Console.WriteLine("Invalid Trip ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Trip ID format.");
            }
        }

        public void CompleteTrip(List<Trip> availableTrips)
        {
            Console.Write("Enter Trip ID to complete: ");
            string tripId = Console.ReadLine();
            if (int.TryParse(tripId, out int tripIdInt))
            {
                var trip = tripHistory.FirstOrDefault(t => t.TripID == tripIdInt);
                if (trip != null && trip.Status == TripStatus.InProgress)
                {
                    trip.EndTrip();
                    availableTrips.Remove(trip);
                    IsAvailable = true;
                    Console.WriteLine("Trip completed successfully!");
                }
                else
                {
                    Console.WriteLine("Invalid Trip ID or Trip not in progress.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Trip ID format.");
            }
        }

        public void ViewTripHistory()
        {
            Console.WriteLine("Trip History:");
            foreach (var trip in tripHistory)
            {
                trip.DisplayTripDetails();
            }
        }
    }

    class Trip
    {
        public int TripID { get; private set; }
        public string RiderName { get; set; }
        public string DriverName { get; set; }
        public string StartLocation { get; private set; }
        public string Destination { get; private set; }
        public decimal Fare { get; private set; }
        public TripStatus Status { get; private set; }

        public Trip(int tripID, string riderName, string driverName, string startLocation, string destination)
        {
            TripID = tripID;
            RiderName = riderName;
            DriverName = driverName;
            StartLocation = startLocation;
            Destination = destination;
            Status = TripStatus.Pending;
            CalculateFare();
        }

        public void CalculateFare()
        {
            Fare = 20.0m; 
        }

        public void StartTrip()
        {
            Status = TripStatus.InProgress;
            Console.WriteLine("Trip started.");
        }

        public void EndTrip()
        {
            Status = TripStatus.Completed;
            Console.WriteLine("Trip ended.");
        }

        public void DisplayTripDetails()
        {
            Console.WriteLine($"Trip ID: {TripID}, Rider: {RiderName}, Driver: {DriverName}, From: {StartLocation}, To: {Destination}, Fare: {Fare}, Status: {Status}");
        }
    }

    enum TripStatus
    {
        Pending,
        InProgress,
        Completed
    }
}

