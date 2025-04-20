using System;
using System.Collections.Generic;
using System.Linq;

class Node
{
    public static int[] GoalState = { 1, 2, 3, 8, 0, 4, 7, 6, 5 };
    public int[] State;
    public int g;  // Gerçekleşen maliyet
    public int h;  // Heuristic maliyet
    public int f;  // Toplam maliyet (f = g + h)
    public Node Parent;

    public Node(int[] state, Node parent = null, int g = 0)
    {
        State = (int[])state.Clone();
        Parent = parent;
        this.g = g;
        CalculateHeuristic();
        f = g + h;
    }

    private void CalculateHeuristic()
    {
        h = 0;
        for (int i = 0; i < 9; i++)
        {
            if (State[i] == 0) continue;
            int goalIndex = Array.IndexOf(GoalState, State[i]);
            int rowDiff = Math.Abs(i / 3 - goalIndex / 3);
            int colDiff = Math.Abs(i % 3 - goalIndex % 3);
            h += rowDiff + colDiff;
        }
    }

    public static void PrintState(int[] state)
    {
        for (int i = 0; i < 9; i++)
        {
            Console.Write(state[i] == 0 ? "  " : state[i] + " ");
            if (i % 3 == 2) Console.WriteLine();
        }
        Console.WriteLine("----------");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter initial state (row by row, use 0 for empty space):");
        int[] initialState = ReadState();

        Console.WriteLine("Enter goal state (row by row, use 0 for empty space):");
        Node.GoalState = ReadState();

        SolvePuzzle(initialState);
    }

    static int[] ReadState()
    {
        int[] state = new int[9];
        for (int i = 0; i < 3; i++)
        {
            string input = Console.ReadLine();
            string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Fazla boşlukları temizler

            if (parts.Length != 3) // Eğer 3 sayı yoksa hata ver
            {
                Console.WriteLine("Geçersiz giriş! Lütfen 3 sayı girin.");
                i--; // Geçersiz satır girildiyse aynı satırı tekrar sor
                continue;
            }

            // Sayıları dönüştür
            int[] row = Array.ConvertAll(parts, int.Parse);
            Array.Copy(row, 0, state, i * 3, 3); // Satırı state dizisine ekle
        }
        return state;
    }

    static void SolvePuzzle(int[] initialState)
    {
        HashSet<string> visited = new HashSet<string>();
        List<Node> openList = new List<Node>();  // SortedSet yerine List kullandık
        Node startNode = new Node(initialState);
        openList.Add(startNode);

        int stepCount = 0; // Adım sayısını takip etmek için

        while (openList.Count > 0)
        {
            // Listeyi sıralayarak, f değeri en küçük olanı seçiyoruz
            openList.Sort((x, y) => x.f.CompareTo(y.f));
            Node current = openList[0];
            openList.RemoveAt(0);

            if (current.State.SequenceEqual(Node.GoalState))
            {
                PrintSolution(current);
                return;
            }

            visited.Add(string.Join(",", current.State));

            // Adım sayısını artır ve geçerli durumu yazdır
            stepCount++;
            Console.WriteLine($"Step {stepCount}:");
            Node.PrintState(current.State);

            // `g`, `h` ve `f` değerlerini yazdır
            Console.WriteLine($"g = {current.g}, h = {current.h}, f = {current.f}");

            foreach (Node neighbor in GenerateNeighbors(current))
            {
                if (!visited.Contains(string.Join(",", neighbor.State)))
                {
                    openList.Add(neighbor);
                }
            }
        }

        Console.WriteLine("No solution found.");
    }

    static List<Node> GenerateNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int zeroIndex = Array.IndexOf(node.State, 0);
        int[] moves = { -1, 1, -3, 3 }; // Left, Right, Up, Down

        foreach (int move in moves)
        {
            int newIndex = zeroIndex + move;
            if (newIndex >= 0 && newIndex < 9 &&
                !((zeroIndex % 3 == 2 && move == 1) || (zeroIndex % 3 == 0 && move == -1)))
            {
                int[] newState = (int[])node.State.Clone();
                (newState[zeroIndex], newState[newIndex]) = (newState[newIndex], newState[zeroIndex]);
                neighbors.Add(new Node(newState, node, node.g + 1));
            }
        }
        return neighbors;
    }

    static void PrintSolution(Node node)
    {
        Stack<Node> path = new Stack<Node>();
        while (node != null)
        {
            path.Push(node);
            node = node.Parent;
        }

        Console.WriteLine("Solution Steps:");
        while (path.Count > 0)
        {
            Node.PrintState(path.Pop().State);
        }
    }
}
