
# Scalability via Sparsity in Stackelberg Security Games: An Augmented Decision Space Approach

This repository contains the official implementation of the **Augmented Decision Space Optimization (ADSO)** framework for **Stackelberg Security Games (SSGs)**.

The project demonstrates how explicitly encoding **sparsity** through a dual binary-real representation can significantly improve **scalability** and **convergence speed** in large-scale game-theoretic optimization problems.

-----

## 📄 About the Algorithm

ADSO (**Augmented Decision Space Optimization**) addresses the challenge of finding optimal **mixed strategies** for the **Leader** in SSGs where the solution space is vast but the optimal strategy is typically **sparse** (i.e., uses only a few pure strategies).

Key features of the implementation:

  * **Dual Encoding:** Strategies are represented by a tuple $(\hat{x}, \tilde{x})$, where $\hat{x}$ are **binary variables** (inclusion/exclusion) and $\tilde{x}$ are **real-valued variables** (probabilities).
  * **Hybrid Optimization:**
      * Binary variables are updated using a gradient-based probabilistic method (similar to **PBIL**).
      * Real variables are updated using **CMA-ES** (Covariance Matrix Adaptation Evolution Strategy).
  * **Danskin's Theorem Optimization:** For zero-sum games (e.g., **Warehouse Games**), the algorithm leverages **Danskin's theorem** to compute the Follower's best response only once per generation against the mean strategy, drastically reducing computational cost.

-----

## 📂 Repository Structure

The code is organized to separate the new ADSO implementation from the legacy evolutionary approaches.

  * `GeneticMultistepSG/`
      * `├── ADSO/`: Core ADSO implementation
          * `├── AdsoAlgorithm.cs`: Main optimization loop (**Binary + Real updates**)
          * `└── CmaEsOptimizer.cs`: **CMA-ES** implementation for real-valued part
      * `├── Struct/`: Game definitions and Graph structures
          * `├── Game.cs`: Base game classes
          * `├── Ggame.cs`: Warehouse/Search Game definitions
          * `└── FlipItGame.cs`: FlipIt Game definitions
      * `├── GAMES/`: Benchmark game definitions
      * `├── EASG/`: Legacy Evolutionary Algorithms (**EASG, CoEvoSG**)
          * `├── Population*.cs`: Old population management
          * `├── Chromosome*.cs`: Old chromosome encodings
          * `└── Coevolution*.cs`: Old evaluation logic
      * `├── Program.cs`: Entry point for the application
      * `└── GeneticMultistepSG.csproj`

-----

## 🚀 Running the Code

1.  **Configuration:**
    Open `Program.cs` to select the game mode and set parameters.

    ```csharp
    // in Program.cs
    public static int PopulationSize = 200; 
    public static int Generations = 500;

    static void Main(string[] args)
    {
        // Uncomment the desired benchmark
        MainGGame(args);       // Warehouse Games / Search Games
        // MainFlipItGame(args); // FlipIt Games
    }
    ```

2.  **Game Files:**
    The repository includes benchmark instances in the **`games/`** folder. Ensure `Program.cs` points to this directory (e.g., `games/smallbuilding-nontrivial.set` or `games/*.niflip`).

3.  **Build & Run:**
    Build the solution in **Release mode** for optimal performance and run the executable.

***Note:*** The contents of the `EASG` folder are provided for archival purposes and reproducibility of the baseline comparisons mentioned in the paper.

## Cite as

```
A. Żychowski, A. Gupta, Y-S. Ong, J. Mańdziuk. Scalability via Sparsity in Stackelberg Security Games: An Augmented Decision Space Approach,
ACM Transactions on Evolutionary Learning and Optimization. 2026.
```