using System;
using System.IO;

namespace Pong;

internal class Program
{
    private static void Main(string[] args)
    {
        var game = new Game(); 
        game.Run();
    }
}
