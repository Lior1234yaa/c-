namespace Day3.Demo.Layouts;

public record Person(string Name, int Age, string City);

public static class SampleData
{
    public static List<Person> People() =>
    [
        new("Dana", 31, "Tel Aviv"),
        new("Yossi", 45, "Haifa"),
        new("Noa", 27, "Jerusalem"),
        new("Amir", 38, "Beer Sheva"),
    ];
}
