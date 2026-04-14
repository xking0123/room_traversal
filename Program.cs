//expand?: data format, json- use for data storing. (expand game to be like... more profesisional dungeon crawler)
//chatgpt- ask for ideas to turn this into proper rougelike system? (ik ai=bad but... shut up...)

//make a game like battle system but have player bounce around between various kinds of rooms rather than battles
//have 1 room be a combat room, have 1 room be a room where you can heal, have 1 room where you can quit
using System.Text.Json;

public class gameState
{
    public Char player { get; set; }
    public List<Battle> Battles { get; set; }
}

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

    //making it so back to back battles wont share the same hp
    //making a new enemy object everytime a battle is generated
    public Char Clone()
    {
        return new Char(Name, MaxHealth, MaxMana, Attack, Defense);
    }
}

public class Battle
{
    public Char Enemy { get; set; }
    public string Name { get; set; }

    public Battle(string name, Char enemy)
    {
        Name = name;
        Enemy = enemy;
    }
}

public class StaticData
{
    public List<Room> Rooms { get; set; }
}

public class Room
{
    public string Description { get; set; } = "";
    public List<Choice> Choices { get; set; } = [];
}

public class Choice
{
    public string Description { get; set; } = "";
    public string FlavorText { get; set; } = "";

    public TransitionTarget? TransitionTarget { get; set; }
    public StatIncrease? StatIncrease { get; set; }
    public StatusEffect? StatusEffect { get; set; }
}

public class StatIncrease {}
public enum StatusEffect
{
    Poisoned,
    Burned,
    Blinded,
}

public enum TransitionTarget
{
    Battle,
    Escape,
}

public static partial class Program
{
    enum EnemyAction { Attack, Defend, Magic } //actions enemy can take
    public static void Main()
    {
        //making user for battles
        Char player = new Char("Wanderer", 50, 30, 20, 20);

        //make list of (multiple?) battles to randomly generate in the combat room- actions will only be attack, defend, magic
        List<Battle> battles = new List<Battle>
        {
            new Battle("Opponent 1: San the n00b", new Char("San the n00b", 15, 0, 5, 5)),
            new Battle("Opponent 2: Pan the casual", new Char("Pan the casual", 20, 20, 15, 15)),
            new Battle("Opponent 3: Tan the EXPERT", new Char("Tan the EXPERT", 40, 40, 25, 25)),
        };

        //converting player and player to json string
        string jsonString = JsonSerializer.Serialize(new gameState
        {
            player = player,
            Battles = battles
        }, new JsonSerializerOptions { WriteIndented = true });

        //making new json data file
        File.WriteAllText("data.json", jsonString);

        Console.Clear();
        Console.WriteLine("WELCOME TO THE VOID HOTEL... we hope you enjoy your stay!");
        Console.WriteLine("here's a master key... explore the rooms as you see fit. just don't get lost in the void now :)");

        //picking a single random element
        //using the built-in Random class to make life ez(?)
        var rooms = new List<string> { "Battle", "Rest", "Freedom" };

        //random object
        Random rnd = new Random(); //can be used multiple times

        while (true)
        {//put room generation inside while loop so that it can continuosly generate new room unless you leave hotel
            Console.Clear();

            //get random index (randomly generating a room)
            int index = rnd.Next(rooms.Count); //rnd used here

            //print random room 
            Console.WriteLine(rooms[index]);

            //get random index for battles (randomly generate like rooms)
            int battleIndex = rnd.Next(battles.Count); //rnd also used here

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
                    Console.WriteLine("You became lost in the void...");
                    break;
                }
            }
            //allow recovery room to heal like healing action in battle system
            if (rooms[index] == "Rest")
            {
                Console.WriteLine("Do you wish to recover your hp for the giving trials ahead? (Y/N)");
                string healResponse = Console.ReadLine();
                if (healResponse == "Y" || healResponse == "y")
                {
                    Console.WriteLine("Recover your strength for the battles ahead, brave one");
                    player.Health += 15;
                    player.Mana += 15;
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

                //making it so I dont reuse Char object if two battles are generated back to back
                Battle selectedBattle = battles[battleIndex];
                Battle freshBattle = new Battle(selectedBattle.Name, selectedBattle.Enemy.Clone());
                RunBattle(player, freshBattle);
                continue;
            }
        }
    }

    //player actions
    private static void ActionAttack(Char enemy, Char player)
    {
        Console.WriteLine("You chose to attack... GET EM'");
        int damage = player.Attack - (enemy.Defense + enemy.DefenseBuff) / 2;
        if (damage < 0)
            damage = 0;
        enemy.Health -= damage;
        if (enemy.Health < 0)
            enemy.Health = 0;
        enemy.DefenseBuff = 0;
        Console.WriteLine($"Dealt {damage} damage to {enemy.Name}. They have {enemy.Health} health left...");
        Console.WriteLine("press anything to go to next turn");
        Console.ReadKey();
    }

    private static void ActionDefend(Char player)
    {
        Console.WriteLine("You chose to defend... BOB & WEAVE");
        player.DefenseBuff += 10;
        Console.WriteLine("press anything to go to next turn");
        Console.ReadKey();
    }

    private static void ActionMagic(Char enemy, Char player)
    {
        if (player.Mana < 5)
        {
            Console.WriteLine("Not enough mana... pls try again later -_-");
            Console.WriteLine("press anything to go to next turn");
            Console.ReadKey();
            return;
        }
        Console.WriteLine("You chose to use magic... GET EM'");
        player.Mana -= 5;
        int damage = 2 * player.Attack - (enemy.Defense + enemy.DefenseBuff) / 2;
        if (damage < 0)
            damage = 0;
        enemy.Health -= damage;
        if (enemy.Health < 0)
            enemy.Health = 0;
        enemy.DefenseBuff = 0;
        Console.WriteLine($"Dealt {damage} damage to {enemy.Name}. They have {enemy.Health} health left...");
        Console.WriteLine("press anything to go to next turn");
        Console.ReadKey();
    }

    //enemy actions
    private static void EnemyAttack(Char enemy, Char player)
    {
        Console.WriteLine($"{enemy.Name} is coming to get you...");
        int damage = enemy.Attack - (player.Defense + player.DefenseBuff) / 2;
        if (damage < 0)
            damage = 0;
        player.Health -= damage;
        if (player.Health < 0)
            player.Health = 0;
        player.DefenseBuff = 0;
        Console.WriteLine($"{enemy.Name} hit you for {damage} damage... You have {player.Health} health left... SPIN BACK");
        Console.WriteLine("press anything to go to next turn");
        Console.ReadKey();
    }

    private static void EnemyDefend(Char enemy)
    {
        Console.WriteLine($"{enemy.Name} is bobing and weaving");
        enemy.DefenseBuff += 10;
        Console.WriteLine("press anything to go to next turn");
        Console.ReadKey();
    }

    private static void EnemyMagic(Char enemy, Char player)
    {
        if (enemy.Mana < 5)
        {
            EnemyAttack(enemy, player);
            return;
        }
        Console.WriteLine($"{enemy.Name} is hitting you with some dark arts...");
        enemy.Mana -= 5;
        int damage = 2 * enemy.Attack - (player.Defense + player.DefenseBuff) / 2;
        if (damage < 0)
            damage = 0;
        player.Health -= damage;
        if (player.Health < 0)
            player.Health = 0;
        player.DefenseBuff = 0;
        Console.WriteLine($"{enemy.Name} hit you for {damage} damage... You have {player.Health} health left... SPIN BACK");
        Console.WriteLine("press anything to go to next turn");
        Console.ReadKey();
    }

    static void EnemyTurn(Char player, Char enemy)
    {
        List<EnemyAction> possibleActions = new List<EnemyAction> { EnemyAction.Attack, EnemyAction.Defend }; //making list of possible actions for enemy
        if (enemy.MaxMana > 0) possibleActions.Add(EnemyAction.Magic);//adding magic to pool of options if enough mana

        //allow enemy to make random moves
        Random rnd = new Random();
        EnemyAction action = possibleActions[rnd.Next(possibleActions.Count)]; //better than static index going up...

        //saying what each action does (referencing their fuctions)
        switch (action)
        {
            case EnemyAction.Attack:
                EnemyAttack(enemy, player);
                break;
            case EnemyAction.Defend:
                EnemyDefend(enemy);
                break;
            case EnemyAction.Magic:
                EnemyMagic(enemy, player);
                break;
        }
    }

    //running actual battle
    static void RunBattle(Char player, Battle battle)
    {
        bool battleActive = true;
        while (battleActive && player.Health > 0)
        {
            Console.Clear();

            DisplayBattle(player, battle.Enemy);

            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)//player controls
            {
                case ConsoleKey.A:
                    ActionAttack(battle.Enemy, player);
                    break;
                case ConsoleKey.D:
                    ActionDefend(player);
                    break;
                case ConsoleKey.M:
                    ActionMagic(battle.Enemy, player);
                    break;

                default:
                    Console.WriteLine("ok magic man... whatever you say");
                    break;
            }

            //if enemy alive
            if (battle.Enemy.Health > 0)
            {
                EnemyTurn(player, battle.Enemy);
                if (player.Health <= 0)
                {
                    battleActive = false;
                }
            }

            //if enemy ded
            if (battle.Enemy.Health <= 0)
            {
                battleActive = false;
                Console.WriteLine("CONGRATULATIONS!");
            }

            if (player.Health <= 0)
            {
                resetGame();
            }
        }
    }

    //implementing display :)
    static void DisplayBattle(Char player, Char enemy)
    {
        int borderWidth = 24;

        Console.WriteLine("\n╔" + new string('═', borderWidth) + "╗");
        string enemyName = $"ENEMY: {enemy.Name}";
        Console.WriteLine("║" + enemyName.PadRight(borderWidth) + "║");
        DrawEnemyHealth(enemy);
        Console.WriteLine("╚" + new string('═', borderWidth) + "╝");
        Console.WriteLine("\n╔" + new string('═', borderWidth) + "╗");
        string playerInfo = $"{player.Name} " + "ATK: " + $"{player.Attack}" + " DEF: " + $"{player.Defense}";
        Console.WriteLine("║" + playerInfo.PadRight(borderWidth) + "║");
        DrawPlayerHealth(player);
        DrawPlayerMana(player);
        Console.WriteLine("╚" + new string('═', borderWidth) + "╝");
        Console.WriteLine("╔═════════╤═════════╗");
        Console.WriteLine("║Attack: A│Defend: D║");
        Console.WriteLine("╟─────────┴─────────╢");
        Console.WriteLine("║     Magic!: M     ║");
        Console.WriteLine("╚═══════════════════╝");
    }

    static void DrawPlayerHealth(Char player)
    {
        int filled = (int)(20.0 * player.Health / player.MaxHealth);
        int empty = 20 - filled;

        Console.Write("║HP ");
        Console.Write(new string('█', filled));
        Console.Write(new string('░', empty));
        Console.WriteLine(" ║");
    }

    static void DrawPlayerMana(Char player)
    {
        if (player.MaxMana == 0) return;
        int filled = (int)(20 * player.Mana / player.MaxMana);
        int empty = 20 - filled;

        Console.Write("║MP ");
        Console.Write(new string('▓', filled));
        Console.Write(new string('░', empty));
        Console.WriteLine(" ║");
    }

    static void DrawEnemyHealth(Char enemy)
    {
        int filled = (int)(20.0 * enemy.Health / enemy.MaxHealth);
        int empty = 20 - filled;

        Console.Write("║HP ");
        Console.Write(new string('█', filled));
        Console.Write(new string('░', empty));
        Console.WriteLine(" ║");
    }

    //game over when player health reaches 0 (allow them to start over? - reset game function?)
    static void resetGame()
    {
        Console.Clear();
        Console.WriteLine("welp.. looks like you died and were lost in the void...");
        Console.WriteLine("do you wish to go back??? (Y/N)");
        string endResponse = Console.ReadLine();
        if (endResponse == "Y" || endResponse == "y")
        {
            Console.WriteLine("May you experience good fortune on your journey back :)");
            Main();
        }
        else if (endResponse == "N" || endResponse == "n")
        {
            Console.WriteLine("oh... i see... have fun doing what you do then... bye bye");
        }
        else
        {
            Console.WriteLine("alright i'm sick and tired of your gibberish GO AWAY");
        }
    }
}