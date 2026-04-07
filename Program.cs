//make a game like battle system but have player bounce around between various kinds of rooms rather than battles
//have 1 room be a combat room, have 1 room be a room where you can heal, have 1 room where you can quit

using System.Net;
using System.Runtime.CompilerServices;

public static partial class Program
{
    public static void Main()
    {
        Console.Clear();
        Console.WriteLine("WELCOME TO THE VOID HOTEL... we hope you enjoy your stay!");
        Console.WriteLine("here's a master key... explore the rooms as you see fit. just don't get lost in the void now :)");

        //WORK:
        //put in player stats for battles
        //make list of (multiple?) battles to randomly generate in the combat room- actions will only be attack, defend, magic
        //allow recovery room to heal like healing action in battle system
        //game over when player health reaches 0 (allow them to start over? - reset game function?)
        //allow game to generate new room after done acting in current room and go through if statments again...? HOW!!!!

        //picking a single random element
        //using the built-in Random class to make life ez(?)
        var rooms = new List<string> { "Battle", "Rest", "Freedom" };

        //random object
        Random rnd = new Random();

        //get random index
        int index = rnd.Next(rooms.Count);

        //print random room 
        Console.WriteLine(rooms[index]);

        while (true)
        {
            if (rooms[index] == "Freedom")
            {
                Console.WriteLine("Do you wish to leave this trap? (Y/N)");
                string response = Console.ReadLine();
                if (response == "N" || response == "n")
                {
                    Console.WriteLine("yippeee... more CHAAAOOOSSS");
                    Console.WriteLine(rooms[index]);
                }
                if (response == "Y" || response == "y")
                {
                    Console.WriteLine("bye... coward...");
                    break;
                }
                else
                {
                    Console.WriteLine("um.... no clue what you said... TO THE VOID WITH YOU!");
                    Console.WriteLine("You died...");
                    break;
                }
            }
            if (rooms[index] == "Rest")
            {
                Console.WriteLine("Do you wish to recover your hp for the giving trials ahead? (Y/N)");
            }
            if (rooms[index] == "Battle")
            {
                Console.WriteLine("TIME FOR BATTLE");
            }
        }
    }
}