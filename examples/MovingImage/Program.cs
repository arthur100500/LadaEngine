using System;
using System.IO;
using MovingImage;

namespace MovingImage;

    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var game = new Game();
                game.Run();
            }
            catch (Exception ex)
            {
                File.WriteAllText("Error.txt", ex.ToString());
                Console.WriteLine(ex.ToString());
            }
        }
    }
