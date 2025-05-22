using System;
using System.Collections.Generic;

namespace CustomWorldGenerator
{
    public class PerlinNoise2DProfile
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public List<Octave> Octaves { get; set; }
        public int Seed { get; set; }

        public PerlinNoise2DProfile() {
            this.Width = 0;
            this.Height = 0;
            this.Octaves = new List<Octave>();
            this.Seed = new Random().Next(int.MinValue, int.MaxValue);
        }
        public PerlinNoise2DProfile(int width, int height, List<Octave> octaves, int seed) {
            this.Width = width;
            this.Height = height;
            this.Octaves = octaves;
            this.Seed = seed;
        }
    }
}
