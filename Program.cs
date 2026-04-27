//expansion: add shops, add exp/gold to json, death = restart, randomized runs
//chatgpt: turn game into proper roguelike...

//data way w/json (making life easier)
//battle system pulled from ogProgram.cs so look there for battle logic...

using System.Text.Json;
var rooms = JsonSerializer.Deserialize<List<Room>>(File.ReadAllText("room.json"))!;

//leveling system
static void CheckLevelUp(Char player)
{
    int xpToNext = player.Level * 20;

    while (player.XP >= xpToNext)
    {
        player.XP -= xpToNext;
        player.Level++;

        player.MaxHealth += 10;
        player.Attack += 5;
        player.Defense += 3;

        player.Health = player.MaxHealth;

        Console.WriteLine($"LEVEL UP! You are now level {player.Level}!");
        xpToNext = player.Level * 20;
    }
}

//adding shops to game
// void RunShop(Char player)
// {
//     Console.WriteLine("Welcome to the shop!");

//     while (true)
//     {
//         Console.WriteLine($"Gold: {player.Gold}");
//         Console.WriteLine("1. +10 Max HP (10 gold)");
//         Console.WriteLine("2. +5 Attack (10 gold)");
//         Console.WriteLine("3. Leave");

//         var key = Console.ReadKey(true).Key;

//         if (key == ConsoleKey.D1 && player.Gold >= 10)
//         {
//             player.Gold -= 10;
//             player.MaxHealth += 10;
//             Console.WriteLine("Max HP increased!");
//         }
//         else if (key == ConsoleKey.D2 && player.Gold >= 10)
//         {
//             player.Gold -= 10;
//             player.Attack += 5;
//             Console.WriteLine("Attack increased!");
//         }
//         else if (key == ConsoleKey.D3)
//         {
//             break;
//         }
//     }
// }

Console.Clear();
Console.WriteLine($"There are {rooms.Count} rooms.");

//generating random rooms
var rand = new Random();
for (var n = rooms.Count - 1; n > 0; n--)
{
    var k = rand.Next(n + 1);
    var value = rooms[k];
    rooms[k] = rooms[n];
    rooms[n] = value;
}
Console.WriteLine($"There are now {rooms.Count} rooms.");

//implement player and put it outside of loop to make sure it 'persists' (think like og battle system in ogProgram)
Char player = new Char("Wanderer", 50, 30, 20, 20);

//converting JSON enemy data to actual Char to use for battles
//moved outside of loop to avoid recreating enemy with every instance of an enemy...?
Char ConvertToChar(Enemy enemy)
{
    return new Char(
        enemy.Name,
        enemy.MaxHealth,
        enemy.MaxMana,
        enemy.Attack,
        enemy.Defense
    );
}

//going through all the rooms one by one
foreach (var room in rooms)
{
    var effects = DoRoom(room);
    Console.WriteLine(effects.Flavor);

    //so player can actually heal and not get ragdolled throughout all the fights...
    if (effects.HealAmount > 0)
    {
        //adding a healing range rather than same amount every time (make it more... game like?)
        int missingHealth = player.MaxHealth - player.Health;
        int heal = Math.Min(effects.HealAmount, missingHealth);

        player.Health += heal;
        if (player.Health > player.MaxHealth)
            player.Health = player.MaxHealth;

        Console.WriteLine($"You healed for {heal}! Current HP: {player.Health}/{player.MaxHealth}");
    }

    //adding way for player to restore mana to continuously use magic attacks
    if (effects.ManaRestore > 0)
    {
        int manaRecovery = effects.ManaRestore;

        player.Mana += manaRecovery;
        if (player.Mana > player.MaxMana)
            player.Mana = player.MaxMana;
    }

    //actually implementing battle system for rooms with battles
    if (0 < effects.PossibleEnemies.Count)
    {
        var enemyIndex = rand.Next(0, effects.PossibleEnemies.Count);
        var enemyData = effects.PossibleEnemies[enemyIndex];

        Console.WriteLine($"You encounter {enemyData.Name}!");

        // convert JSON enemy into battle-ready character
        Char enemyChar = ConvertToChar(enemyData);

        // wrap it in Battle object
        Battle battle = new Battle(enemyData.Name, enemyChar);

        // run actual combat
        RunBattle(player, battle);

        // outcome
        if (player.Health > 0)
        {
            Console.WriteLine(enemyData.DeathFlavor);
            player.Gold += enemyData.GoldReward;
            player.XP += enemyData.XPReward;
            Console.WriteLine($"+{enemyData.GoldReward} gold, +{enemyData.XPReward} XP!");

            //calling level up after rewards after getting rewards
            CheckLevelUp(player);
        }
        else
        {
            Console.WriteLine(enemyData.KilledByFlavor);
            return;
        }
    }
}

//printing out everything relating to each individual room
Effects DoRoom(Room room)
{
    Console.WriteLine(room.Description + "\n");
    Console.WriteLine(room.Prompt + "\n");
    Console.WriteLine($" Y: {room.YesChoice.Prompt}");
    Console.WriteLine($" N: {room.NoChoice.Prompt}\n");
    while (true)
    {
        Console.Write("Your choice: ");
        var choice = Console.ReadKey().KeyChar;
        Console.WriteLine();
        switch (choice)
        {
            case 'y':
                Console.WriteLine(room.YesChoice.Flavor);
                return room.YesChoice.Effects;
            case 'n':
                Console.WriteLine(room.NoChoice.Flavor);
                return room.NoChoice.Effects;
        }
    }
}

//data stuffy
class Room
{
    public required string Description { get; set; }
    public required string Prompt { get; set; }
    public required Choice YesChoice { get; set; }
    public required Choice NoChoice { get; set; }
}
class Choice
{
    public required string Prompt { get; set; }
    public required string Flavor { get; set; }
    public required Effects Effects { get; set; }
}

//added in healing and mana restoring so playing can get stats back like in og battle system
class Effects
{
    public required string Flavor { get; set; }
    public List<Enemy> PossibleEnemies { get; set; } = [];

    public int HealAmount { get; set; } = 0;
    public int ManaRestore { get; set; } = 0;
    public bool IsShop { get; set; } = false;
}

//work: maybe try to add actual battling instead of just yes no for killing enemies...
//adding in mana, attack, and defense to implement actual battle system
class Enemy
{
    public required string Name { get; set; }
    public required int MaxHealth { get; set; }
    public required int MaxMana { get; set; }
    public required int Attack { get; set; }
    public required int Defense { get; set; }

    public required string DeathFlavor { get; set; }
    public required string KilledByFlavor { get; set; }
    public int GoldReward { get; set; }
    public int XPReward { get; set; }
}