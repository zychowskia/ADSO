using System;
using System.Linq;

namespace GeneticMultistepSG.Adso
{
    // Minimal compliant implementation of CMA-ES for ADSO real-valued optimization
    public class CmaEsOptimizer
    {
        private int N; // Dimension
        private int Lambda; // Population size
        private int Mu; // Parents size

        // Strategy parameters
        public double[] Mean { get; private set; }
        private double[] Weights;
        private double Mueff;
        private double Sigma;
        private double[,] C; // Covariance matrix
        private double[] P_c; // Evolution path for C
        private double[] P_sigma; // Evolution path for sigma
        private double[,] B; // Eigenvectors
        private double[] D; // Eigenvalues

        // Learning rates
        private double C_c;
        private double C_1;
        private double C_mu;
        private double C_sigma;
        private double D_sigma;
        private double ChiN;

        private Random Rand;
        private int EigensystemUpToDate = 0;

        public CmaEsOptimizer(int dimension, int populationSize, double initialSigma, Random rand)
        {
            N = dimension;
            Lambda = populationSize;
            Rand = rand;
            Mu = (int)Math.Floor(Lambda / 2.0);
            
            // Initialization
            Mean = new double[N];
            for (int i = 0; i < N; i++) Mean[i] = 0.5; // Start centered

            Sigma = initialSigma;

            // Weights
            Weights = new double[Mu];
            double sumW = 0;
            for (int i = 0; i < Mu; i++)
            {
                Weights[i] = Math.Log(Mu + 0.5) - Math.Log(i + 1);
                sumW += Weights[i];
            }
            for (int i = 0; i < Mu; i++) Weights[i] /= sumW;

            Mueff = 0;
            double sumSqr = 0;
            for (int i = 0; i < Mu; i++) sumSqr += Weights[i] * Weights[i];
            Mueff = 1 / sumSqr;

            // Strategy Parameters
            C_c = (4 + Mueff / N) / (N + 4 + 2 * Mueff / N);
            C_sigma = (Mueff + 2) / (N + Mueff + 5);
            C_1 = 2 / ((N + 1.3) * (N + 1.3) + Mueff);
            C_mu = Math.Min(1 - C_1, 2 * (Mueff - 2 + 1 / Mueff) / ((N + 2) * (N + 2) + Mueff));
            D_sigma = 1 + 2 * Math.Max(0, Math.Sqrt((Mueff - 1) / (N + 1)) - 1) + C_sigma;
            ChiN = Math.Sqrt(N) * (1 - 1 / (4.0 * N) + 1 / (21.0 * N * N));

            // Matrices
            C = new double[N, N];
            B = new double[N, N];
            D = new double[N];
            for (int i = 0; i < N; i++)
            {
                C[i, i] = 1.0;
                B[i, i] = 1.0;
                D[i] = 1.0;
            }

            P_c = new double[N];
            P_sigma = new double[N];
        }

        public double[][] SamplePopulation()
        {
            if (EigensystemUpToDate > 0) UpdateEigensystem();

            double[][] offspring = new double[Lambda][];
            for (int k = 0; k < Lambda; k++)
            {
                offspring[k] = new double[N];
                double[] z = new double[N];
                for (int i = 0; i < N; i++) z[i] = GaussianRandom();

                // y = B * D * z
                double[] y = MultiplyBD(z);

                // x = m + sigma * y
                for (int i = 0; i < N; i++)
                {
                    offspring[k][i] = Mean[i] + Sigma * y[i];
                }
            }
            return offspring;
        }

        public void UpdateDistribution(double[][] population, double[] fitnessValues)
        {
            // Sort by fitness (descending because we maximize payoff)
            var sortedIndices = Enumerable.Range(0, Lambda)
                .OrderByDescending(i => fitnessValues[i])
                .ToArray();

            // Recombine Mean
            double[] oldMean = (double[])Mean.Clone();
            for (int i = 0; i < N; i++) Mean[i] = 0;
            
            for (int i = 0; i < Mu; i++)
            {
                double[] candidate = population[sortedIndices[i]];
                for (int j = 0; j < N; j++)
                {
                    Mean[j] += Weights[i] * candidate[j];
                }
            }

            // Update Evolution Paths
            // y_w = (m_new - m_old) / sigma
            double[] y_w = new double[N];
            for (int i = 0; i < N; i++) y_w[i] = (Mean[i] - oldMean[i]) / Sigma;

            // C^-1/2 * y_w (approximation)
            double[] invBDy_w = MultiplyInvBD(y_w);

            // P_sigma
            double psLen = 0;
            for(int i=0; i<N; i++)
            {
                P_sigma[i] = (1 - C_sigma) * P_sigma[i] + Math.Sqrt(C_sigma * (2 - C_sigma) * Mueff) * invBDy_w[i];
                psLen += P_sigma[i] * P_sigma[i];
            }
            psLen = Math.Sqrt(psLen);

            // P_c
            double h_sigma = (psLen / Math.Sqrt(1 - Math.Pow(1 - C_sigma, 2 * (EigensystemUpToDate + 1))) / ChiN < 1.4 + 2.0 / (N + 1)) ? 1 : 0;
            
            for (int i = 0; i < N; i++)
            {
                P_c[i] = (1 - C_c) * P_c[i] + h_sigma * Math.Sqrt(C_c * (2 - C_c) * Mueff) * y_w[i];
            }

            // Update C
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    double sumRankOne = P_c[i] * P_c[j];
                    double sumRankMu = 0;
                    for (int k = 0; k < Mu; k++)
                    {
                        int idx = sortedIndices[k];
                        // y_k = (x_k - m_old) / sigma
                        double y_k_i = (population[idx][i] - oldMean[i]) / Sigma;
                        double y_k_j = (population[idx][j] - oldMean[j]) / Sigma;
                        sumRankMu += Weights[k] * y_k_i * y_k_j;
                    }

                    C[i, j] = (1 - C_1 - C_mu) * C[i, j] 
                              + C_1 * (sumRankOne + (1-h_sigma) * C_c * (2-C_c) * C[i, j])
                              + C_mu * sumRankMu;
                }
            }

            // Update Sigma
            Sigma *= Math.Exp((C_sigma / D_sigma) * (psLen / ChiN - 1));

            EigensystemUpToDate++;
        }

        private void UpdateEigensystem()
        {
            for(int i=0; i<N; i++) 
            {
                D[i] = Math.Sqrt(Math.Max(0, C[i,i]));
                for(int j=0; j<N; j++) B[i,j] = (i==j) ? 1 : 0;
            }
            EigensystemUpToDate = 0;
        }

        private double[] MultiplyBD(double[] z)
        {
            double[] res = new double[N];
            for (int i = 0; i < N; i++)
            {
                double sum = 0;
                for (int j = 0; j < N; j++) sum += B[i, j] * D[j] * z[j];
                res[i] = sum;
            }
            return res;
        }
        
        private double[] MultiplyInvBD(double[] v)
        {
            // B * D^-1 * B^T * v (approx since we kept B identity in snippet)
            double[] res = new double[N];
            for (int i = 0; i < N; i++) res[i] = v[i] / (D[i] + 1e-10);
            return res;
        }

        private double GaussianRandom()
        {
            double u1 = 1.0 - Rand.NextDouble();
            double u2 = 1.0 - Rand.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        }
    }
}