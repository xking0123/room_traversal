//data way w/json (making life easier)

using System.Text.Json;
var rooms = JsonSerializer.Deserialize<List<Room>>(File.ReadAllText("room.json"))!;
Console.WriteLine($"There are {rooms.Count} rooms.");
var rand = new Random();
for (var n = rooms.Count - 1; n > 0; n--)
{
    var k = rand.Next(n + 1);
    var value = rooms[k];
    rooms[k] = rooms[n];
    rooms[n] = value;
}
Console.WriteLine($"There are now {rooms.Count} rooms.");
foreach (var room in rooms)
{
    var effects = DoRoom(room);
    Console.WriteLine(effects.Flavor);
    if (0 < effects.PossibleEnemies.Count)
    {
        var enemyIndex = rand.Next(0, effects.PossibleEnemies.Count);
        var enemy = effects.PossibleEnemies[enemyIndex];
        Console.WriteLine($"You face off against {enemy.Name}!");
        Console.WriteLine("Do you kill them??");
        var exit = false;
        while (!exit)
        {
            Console.Write("Your choice: ");
            var choice = Console.ReadKey().KeyChar;
            Console.WriteLine();
            switch (choice)
            {
                case 'y':
                    Console.WriteLine(enemy.DeathFlavor);
                    exit = true;
                    break;
                case 'n':
                    Console.WriteLine(enemy.KilledByFlavor);
                    return;
            }
        }
    }
}
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
class Effects
{
    public required string Flavor { get; set; }
    public List<Enemy> PossibleEnemies { get; set; } = [];
}
class Enemy
{
    public required string Name { get; set; }
    public required int InitialHealth { get; set; }
    public required string DeathFlavor { get; set; }
    public required string KilledByFlavor { get; set; }
}

//work: maybe try to add actual battling instead of just yes no for killing enemies...