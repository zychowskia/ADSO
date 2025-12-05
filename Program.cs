using GeneticMultistepCoevoSG.Struct;
using GeneticMultistepSG.ADSO;
using GeneticMultistepSG.Struct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace GeneticMultistepSG
{
    public class Program
    {
        // Global Random instance for reproducibility (Seed 7 as per original)
        public static Random rand = new Random(7);

        // Global Game Definition accessible by static contexts
        public static Game gameDefinition;

        // Logging Writers
        public static StreamWriter attackerWriter;
        public static string currentGame;
        public static int currentIt;

        // ADSO Configuration
        public static int PopulationSize = 200; // Fixed at 200 per paper
        public static int MaxEvaluations = 100000; // 10^5 evaluations
        public static int Generations = 500; // Approximate generations equivalent

        static void Main(string[] args)
        {
            // Uncomment the mode you wish to run based on the paper's benchmarks
            
            // 1. Run Warehouse Games (Zero-Sum, supports Danskin's optimization)
            MainGGame(args);

            // 2. Run FlipIt Games (Non-Zero-Sum)
            // MainFlipItGame(args);
        }

        static void MainGGame(string[] args)
        {
            // Initialize Logging
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-FFF");
            attackerWriter = new StreamWriter($@"resultsAttacker_{timeStamp}.csv");
            StreamWriter writer = new StreamWriter($@"results_{timeStamp}.csv");

            // CSV Header
            writer.WriteLine("game;algorithm;bestPayoff;totalTimeSeconds");

            // Load Game List (Warehouse Games / Search Games)
            // Ensure this path points to your valid .set file or directory
            string[] games;
            try 
            {
                games = File.ReadAllLines(@"path-to-file\smallbuilding-nontrivial.set");
            }
            catch
            {
                Console.WriteLine("Could not load .set file. defaulting to empty list.");
                games = new string[0];
            }

            foreach (string gameName in games)
            {
                string filePath = $@"path-to-file\{gameName}";
                if (!File.Exists(filePath)) 
                {
                    // Try local directory if absolute path fails
                    filePath = gameName;
                    if (!File.Exists(filePath)) continue;
                }

                currentGame = gameName;
                Console.WriteLine($"\n--- Processing {gameName} ---");

                // 1. Load Game Definition
                gameDefinition = LoadGGameDefinition(filePath);

                // 2. Initialize ADSO Algorithm
                //    - Encodes Augmented Decision Space (Binary + Real)
                //    - Configures CMA-ES
                //    - Sets up Danskin's theorem logic for Zero-Sum games
                AdsoAlgorithm adso = new AdsoAlgorithm(gameDefinition, PopulationSize);

                // 3. Run Optimization
                DateTime startTime = DateTime.Now;
                
                // Run for fixed generations or until convergence
                // In paper: "max 10^5 evaluations" -> With pop 200, that's 500 generations.
                adso.Run(Generations); 

                double totalTime = DateTime.Now.Subtract(startTime).TotalSeconds;

                // 4. Log Final Results
                // Note: Accessing internal best payoff would require exposing it from AdsoAlgorithm
                // For this driver, we assume AdsoAlgorithm logs the detailed progression.
                // We log a completion summary here.
                
                Console.WriteLine($"Finished {gameName} in {totalTime:F2}s");
                writer.WriteLine($"{gameName};ADSO;SEE_DETAILED_LOG;{totalTime}");
                writer.Flush();
                attackerWriter.Flush();
            }

            writer.Close();
            attackerWriter.Close();
        }

        static void MainFlipItGame(string[] args)
        {
            // Initialize Logging
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-FFF");
            attackerWriter = new StreamWriter($@"resultsAttacker_FlipIt_{timeStamp}.csv");
            StreamWriter writer = new StreamWriter($@"results_FlipIt_{timeStamp}.csv");

            writer.WriteLine("game;algorithm;bestPayoff;totalTimeSeconds");

            // Load all .niflip games from directory
            string[] games = Directory.GetFiles(@"path-to-file\games", "*.niflip")
                                      .Select(x => Path.GetFileName(x))
                                      .ToArray();

            foreach (string gameName in games)
            {
                string filePath = $@"path-to-file\games\{gameName}";
                if (!File.Exists(filePath)) continue;

                currentGame = gameName;
                Console.WriteLine($"\n--- Processing FlipIt: {gameName} ---");

                // 1. Load Game Definition
                gameDefinition = LoadFlipItGameDefinition(filePath);

                // 2. Initialize ADSO Algorithm
                AdsoAlgorithm adso = new AdsoAlgorithm(gameDefinition, PopulationSize);

                // 3. Run Optimization
                DateTime startTime = DateTime.Now;
                adso.Run(Generations);
                double totalTime = DateTime.Now.Subtract(startTime).TotalSeconds;

                Console.WriteLine($"Finished {gameName} in {totalTime:F2}s");
                writer.WriteLine($"{gameName};ADSO;SEE_DETAILED_LOG;{totalTime}");
                
                writer.Flush();
                attackerWriter.Flush();
            }

            writer.Close();
            attackerWriter.Close();
        }

        // ==========================================
        //         Game Loading Helpers
        // ==========================================

        static Ggame LoadGGameDefinition(string filePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                Ggame game = (new JavaScriptSerializer().Deserialize(jsonContent, typeof(Ggame))) as Ggame;
                
                if (game == null) throw new Exception("Deserialization returned null.");

                game.graphConfig.MakeAdjacencyList();
                game.ComputeAttackerStrategies();
                return game;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading GGame {filePath}: {ex.Message}");
                return new Ggame(); // Return empty to avoid crash, loop will likely fail next step
            }
        }

        static FlipItGame LoadFlipItGameDefinition(string filePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                FlipItGame game = (new JavaScriptSerializer().Deserialize(jsonContent, typeof(FlipItGame))) as FlipItGame;
                
                if (game == null) throw new Exception("Deserialization returned null.");

                game.graph.MakeAdjacencyList();
                game.ComputeAttackerStrategies();
                return game;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading FlipItGame {filePath}: {ex.Message}");
                return new FlipItGame();
            }
        }
    }
}