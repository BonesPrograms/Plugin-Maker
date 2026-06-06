using BonesPluginMaker;

Console.WriteLine("Enter the name of your plugin!");

PluginMaker maker = new();
maker.Make();
Console.WriteLine("Press any key to exit.");
Console.ReadKey();