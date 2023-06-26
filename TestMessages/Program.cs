// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

string file = @"F:\BirdLogger\20230623_093309_19.ubx";

AFlight LastFlight = new AFlight();

LastFlight.LoadUBX(file);
LastFlight.WriteKML();

