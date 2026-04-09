//make a game like battle system but have player bounce around between various kinds of rooms rather than battles
//have 1 room be a combat room, have 1 room be a room where you can heal, have 1 room where you can quit

//expand: data format, json- use for data storing. (expand game to be like... more profesisional dungeon crawler)
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;

//put in player stats for battles
public class Char
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Mana { get; set; }
    public int MaxMana { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int DefenseBuff { get; set; } = 0;

    public Char(string name, int maxHealth, int maxMana, int attack, int defense)
    {
        Name = name;
        Health = maxHealth;
        MaxHealth = maxHealth;
        Mana = maxMana;
        MaxMana = maxMana;
        Attack = attack;
        Defense = defense;
    }
}

public class Battle
{
    public Char Enemy{get; set;}
    public string Name{get; set;}

    public Battle(string name, Char enemy)
    {
        Name = name;
        Enemy = enemy;
    }
}

public static partial class Program
{
    enum EnemyAction{Attack, Defend}
    public static void Main()
    {
        //making user for battles
        Char player = new Char("Void Explorer", 50, 30, 20, 20);

        //make list of (multiple?) battles to randomly generate in the combat room- actions will only be attack, defend, magic
        //randomly generate like rooms
        List<Battle> battles = new List<Battle>
        {
            new Battle("Opponent 1: San the n00b", new Char("San", 15, 0, 5, 5)),
            new Battle("Opponent 2: X KING", new Char("X KING", 50, 30, 20, 20)),
            new Battle("Opponent 3: BIG TERRY", new Char("BIG TERRY", 40, 40, 25, 25)),
            new Battle("Opponent 4: GOD???!", new Char("GOD???!", 100, 100, 50, 50))
        };

        Random battleRND = new Random();

        Console.Clear();
        Console.WriteLine("WELCOME TO THE VOID HOTEL... we hope you enjoy your stay!");
        Console.WriteLine("here's a master key... explore the rooms as you see fit. just don't get lost in the void now :)");

        //WORK:
        //allow recovery room to heal like healing action in battle system
        //game over when player health reaches 0 (allow them to start over? - reset game function?)
        //allow game to generate new room after done acting in current room and go through if statments again...? HOW!!!!

        //picking a single random element
        //using the built-in Random class to make life ez(?)
        var rooms = new List<string> { "Battle", "Rest", "Freedom" };

        //random object
        Random rnd = new Random();

        while (true)
        {//put room generation inside while loop so that it can continuosly generate new room
            Console.Clear();

            //get random index
            int index = rnd.Next(rooms.Count);

            //print random room 
            Console.WriteLine(rooms[index]);

            //get random index for battles
            int battleIndex = battleRND.Next(battles.Count);

            if (rooms[index] == "Freedom")
            {
                Console.WriteLine("Do you wish to leave this trap? (Y/N)");
                string response = Console.ReadLine();
                if (response == "N" || response == "n")
                {
                    Console.WriteLine("yippeee... more CHAAAOOOSSS");
                    continue; //goes back to start of loop (generates new room)
                }
                else if (response == "Y" || response == "y")
                {
                    Console.WriteLine("bye... coward...");
                    break; //breaks out of loop, ends game
                }
                else
                {
                    Console.WriteLine("um.... no clue what you said... im scared so TO THE VOID WITH YOU!");
                    Console.WriteLine("You died...");
                    break;
                }
            }
            if (rooms[index] == "Rest")
            {
                Console.WriteLine("Do you wish to recover your hp for the giving trials ahead? (Y/N)");
                string healResponse = Console.ReadLine();
                if (healResponse == "Y" || healResponse == "y")
                {
                    Console.WriteLine("Recover your strength for the battles ahead, brave one");
                    player.Health += 10;
                    player.Mana += 10;
                    if (player.Health > player.MaxHealth) player.Health = player.MaxHealth;
                    if (player.Mana > player.MaxMana) player.Mana = player.MaxMana;
                    continue;
                }
                else if (healResponse == "N" || healResponse == "n")
                {
                    Console.WriteLine("ok then... good luck out there :)");
                    continue;
                }
                else
                {
                    Console.WriteLine("you speaking gibberish... OUT OF MY SIGHT");
                    continue;
                }
            }
            if (rooms[index] == "Battle")
            {
                Console.WriteLine("TIME FOR BATTLE");

                //pick 1 battle out of the list
                Console.WriteLine(battleIndex);
                RunBattle(player, battleIndex); //commence confrontation...
            }
        }
    }

    //player actions

    //enemy actions
    private static void EnemyAttack(Char enemy, Char player)
    {
        Console.WriteLine($"{enemy.Name} attacks");
    }

    // static void EnemyTurn(Char player, Char enemy)
    // {
    //     List<EnemyAction> possibleActions = new List<EnemyAction> {EnemyAction.Attack, EnemyAction.Defend};
    //     if(enemy.MaxMana > 0) possibleActions.Add()
    // }

    static void RunBattle(Char player, int battleIndex) //had to take int battleIndex rather than the Battle from the list itself (it work?)
    {
        bool battleActive = true;
        while(battleActive && player.Health > 0)
        {
            Console.Clear();

            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)//controls
            {
            
            }
        }
    }

    //try to implement display?
}