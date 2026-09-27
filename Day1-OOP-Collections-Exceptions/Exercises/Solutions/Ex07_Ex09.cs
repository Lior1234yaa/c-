namespace Day1.Exercises;

public static class Ex07
{
    public static void Run()
    {
        const string sentence = "the quick brown fox jumps over the lazy dog The DOG sleeps";
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var word in sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            counts[word] = counts.GetValueOrDefault(word) + 1;

        foreach (var (word, count) in counts.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key))
            Console.WriteLine($"{word,-8} {count}");
    }
}

public static class Ex08
{
    class MyStack<T> where T : notnull
    {
        private T[] _items = new T[4];
        public int Count { get; private set; }
        public bool IsEmpty => Count == 0;

        public void Push(T item)
        {
            if (Count == _items.Length) Array.Resize(ref _items, _items.Length * 2);
            _items[Count++] = item;
        }

        public T Pop()
        {
            if (IsEmpty) throw new InvalidOperationException("stack is empty");
            var item = _items[--Count];
            _items[Count] = default!;   // לא להחזיק הפניה מיותרת
            return item;
        }

        public T Peek() => IsEmpty ? throw new InvalidOperationException("stack is empty") : _items[Count - 1];
    }

    public static void Run()
    {
        var ints = new MyStack<int>();
        for (int i = 1; i <= 10; i++) ints.Push(i);   // גדל מעבר ל-4
        Console.WriteLine($"count={ints.Count}, peek={ints.Peek()}, pop={ints.Pop()}, pop={ints.Pop()}, count={ints.Count}");

        var words = new MyStack<string>();
        words.Push("a"); words.Push("b");
        Console.WriteLine($"{words.Pop()} {words.Pop()} empty={words.IsEmpty}");
        try { words.Pop(); } catch (InvalidOperationException ex) { Console.WriteLine($"error: {ex.Message}"); }
    }
}

public static class Ex09
{
    public static void Run()
    {
        // (א) ייחודיים → HashSet<T>: בלי כפילויות, בדיקת שייכות O(1)
        int[] visits = [17, 3, 17, 42, 3, 8, 17];
        var unique = new HashSet<int>(visits);
        Console.WriteLine($"(a) unique visitors: {unique.Count} of {visits.Length}");

        // (ב) תור הדפסה → Queue<T>: FIFO
        var printQueue = new Queue<string>(["report.pdf", "photo.png", "letter.docx"]);
        Console.WriteLine($"(b) printing {printQueue.Dequeue()}, next is {printQueue.Peek()}");

        // (ג) Undo → Stack<T>: LIFO — הפעולה האחרונה מתבטלת ראשונה
        var undo = new Stack<string>();
        undo.Push("type 'a'"); undo.Push("type 'b'"); undo.Push("bold");
        Console.WriteLine($"(c) undo: {undo.Pop()}, then {undo.Pop()}");

        // (ד) חיפוש לפי מפתח → Dictionary<K,V>: O(1) במקום סריקה
        var students = new Dictionary<string, string> { ["123"] = "Dana", ["456"] = "Yossi" };
        Console.WriteLine($"(d) student 456 = {students["456"]}, has 999? {students.ContainsKey("999")}");

        // (ה) רשימה פשוטה עם סדר → List<T>
        var grades = new List<int> { 88, 92, 75 };
        grades.Add(100);
        Console.WriteLine($"(e) average of {grades.Count} grades = {grades.Average():F1}");
    }
}
