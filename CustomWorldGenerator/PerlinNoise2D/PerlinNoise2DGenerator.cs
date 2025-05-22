using System;

namespace CustomWorldGenerator
{
    public class PerlinNoise2DGenerator{
        private static Random Random = new Random();

        public static double[,] GenerateNoise(PerlinNoise2DProfile profile) {
            //Turn octaves to noise
            double[,] noiseMap = new double[profile.Width, profile.Height];
            for (int i = 0; i < profile.Octaves.Count; i++){
                double[,] noiseOctave = GetNoiseGrid(
                    profile.Width,
                    profile.Height,
                    profile.Octaves[i].Frequency,
                    profile.Octaves[i].Amplitude,
                    profile.Seed + i
                );

                for (int z = 0; z < noiseMap.GetLength(0); z++){
                    for (int x = 0; x < noiseMap.GetLength(1); x++){
                        noiseMap[x, z] += noiseOctave[x, z];
                    }
                }
            }

            return noiseMap;
        }
        public static double[,] GetNoiseGrid(int width, int height, double range, double amplitude, int seed) {
            Random = new Random(seed);
            GeneratePermutations();

            double[,] grid = new double[width, height];

            for (int y = 0; y < height; y++){
                for (int x = 0; x < width; x++){
                    double noise = Noise(
                        (x / (double)width * range),
                        (y / (double)height * range)
                    );
                    noise += 1;
                    noise = noise > 2 ? 2 : noise;
                    noise = noise < 0 ? 0 : noise;

                    noise /= amplitude;

                    grid[x, y] = noise;
                }
            }

            return grid;
        }

        private static double Noise(double x, double y){
            //double rectX = range / (p1.GetLength(0) - 1);
            //double rectY = range / (p1.GetLength(1) - 1);

            int xi = (int)Math.Floor(x) & 255;
            int yi = (int)Math.Floor(y) & 255;

            //int xi = (int)(x / rectX);
            //int yi = (int)(y / rectY);

            int g1 = p[xi, yi];
            int g2 = p[xi + 1, yi];
            int g3 = p[xi, yi + 1];
            int g4 = p[xi + 1, yi + 1];

            double xf = x - Math.Floor(x);
            double yf = y - Math.Floor(y);

            //double xf = x - xi * rectX;
            //double yf = y - yi * rectY;

            double d1 = Grad(g1, xf, yf);
            double d2 = Grad(g2, xf - 1, yf);
            double d3 = Grad(g3, xf, yf - 1);
            double d4 = Grad(g4, xf - 1, yf - 1);

            double u = Fade(xf);
            double v = Fade(yf);

            double x1Inter = Lerp(u, d1, d2);
            double x2Inter = Lerp(u, d3, d4);
            double yInter = Lerp(v, x1Inter, x2Inter);

            return yInter;
        }

        private static double Lerp(double amount, double left, double right) {
            return (1 - amount) * left + amount * right;
        }
        private static double Fade(double t) {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }
        private static double Grad(int hash, double x, double y) {
            switch (hash & 3) {
                case 0: return x + y;
                case 1: return -x + y;
                case 2: return x - y;
                case 3: return -x - y;
                default: return 0;
            }
        }

        private static int[,] p = new int[256, 256];
        private static void GeneratePermutations() {
            for (int y = 0; y < p.GetLength(1); y++){
                for (int x = 0; x < p.GetLength(0); x++){
                    p[x, y] = Random.Next(256);
                }
            }
        }
    }    
}
