namespace Day1.Exercises;

public static class Ex05
{
    interface IElectric { int BatteryKwh { get; } }

    abstract class Vehicle(string name)
    {
        public string Name { get; } = name;
        public abstract int Wheels { get; }
        public virtual string Describe() => $"{Name}: {Wheels} wheels";
    }

    class Car(string name) : Vehicle(name) { public override int Wheels => 4; }
    class Motorcycle(string name) : Vehicle(name) { public override int Wheels => 2; }
    class Truck(string name, int loadCapacity) : Vehicle(name)
    {
        public override int Wheels => 6;
        public override string Describe() => base.Describe() + $", carries {loadCapacity} kg";
    }
    class ElectricCar(string name, int batteryKwh) : Car(name), IElectric
    {
        public int BatteryKwh => batteryKwh;
        public override string Describe() => base.Describe() + $", {BatteryKwh} kWh battery";
    }

    public static void Run()
    {
        List<Vehicle> fleet = [new Car("Corolla"), new Motorcycle("Vespa"), new Truck("Actros", 18_000), new ElectricCar("Model 3", 75)];
        foreach (var v in fleet) Console.WriteLine(v.Describe());
        Console.WriteLine("Electric only:");
        foreach (var e in fleet.OfType<IElectric>()) Console.WriteLine($"  {((Vehicle)e).Name} — {e.BatteryKwh} kWh");
    }
}

public static class Ex06
{
    static string Classify(object? o) => o switch
    {
        null => "null",
        int i when i < 0 => $"negative int {i}",
        int i => $"positive int {i}",
        "" => "empty string",
        string s => $"string \"{s}\"",
        double d => $"double {d}",
        int[] { Length: 0 } => "empty int array",
        int[] arr => $"int array with {arr.Length} items, first={arr[0]}",
        _ => $"something else ({o.GetType().Name})",
    };

    public static void Run()
    {
        object?[] samples = [-3, 7, "", "hi", 2.5, Array.Empty<int>(), new[] { 1, 2 }, null, DateTime.Now];
        foreach (var s in samples) Console.WriteLine(Classify(s));
    }
}
