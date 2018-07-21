using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PwC.Training.Applications
{
    public class TelemetryData
    {
        public int SensorId { get; set; }
        public DateTime RecordedTime { get; set; }
        public string Location { get; set; }
        public string Building { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }

        public override string ToString()
        {
            return string.Format(@"{0}, {1}, {2}, {3}, {4}, {5}",
                this.SensorId, this.Location, this.Building, this.Temperature, this.Humidity, this.RecordedTime.ToString());
        }
    }

    public static class MainClass
    {
        private const int MAX_LIMIT = 2000;
        public static void Main(string[] args)
        {
            Task.Run(() =>
            {
                var eventHubConnectionString = @"Endpoint=sb://iomeganamespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ZdU56cRdnXJ0N2Q+HGOydjyRhMXJQbmDWCOY8/sP7Ko=";
                var eventHubPath = "sensormessages";

                try
                {
                    var eventHubConnectionStringBuilder = new EventHubsConnectionStringBuilder(eventHubConnectionString)
                    {
                        EntityPath = eventHubPath
                    };

                    var eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionStringBuilder.ToString());
                    var messageCount = 1;
                    var random = new Random();
                    var registeredBuildings = new string[] { "Building - A", "Building - B", "Building - C", "Building - D" };
                    var registeredLocations = new string[] { "Bangalore", "Chennai", "Hyderabad", "Mumbai", "New Delhi", "Trivandrum", "Kolkata" };

                    while (true)
                    {
                        if (messageCount % 100 == 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Breaking to Emit Messages ...");
                            Console.WriteLine();

                            Thread.Sleep(random.Next(2000, 5000));
                        }

                        var telemetryData = new TelemetryData
                        {
                            Building = registeredBuildings[random.Next(0, registeredBuildings.Length - 1)],
                            Humidity = random.Next(30, 60),
                            Location = registeredLocations[random.Next(0, registeredLocations.Length - 1)],
                            RecordedTime = DateTime.Now,
                            SensorId = random.Next(1, 10),
                            Temperature = random.Next(23, 32)
                        };

                        var eventData = new EventData(
                            Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(telemetryData)));

                        eventHubClient.SendAsync(eventData).Wait();

                        Console.Write("*");

                        messageCount++;

                        if (messageCount >= MAX_LIMIT)
                            break;

                        Thread.Sleep(50);
                    }
                }
                catch (Exception exceptionObject)
                {
                    Console.WriteLine("Error Occurred, Details : " + exceptionObject.Message);
                }
            });

            Console.WriteLine("End of App!");
            Console.ReadLine();
        }
    }
}