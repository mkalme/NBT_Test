using System;
using System.Collections.Generic;

namespace CustomWorldGenerator
{
    public class GenerationProfile
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Seed { get; set; }
        public PerlinNoise2DProfile PerlinNoiseProfile { get; set; }

        public GenerationProfile() {
            this.Width = 0;
            this.Height = 0;
            this.Seed = new Random().Next(int.MinValue, int.MaxValue);
            this.PerlinNoiseProfile = new PerlinNoise2DProfile();
            this.PerlinNoiseProfile.Seed = this.Seed;
        }
        public GenerationProfile(int width, int height, int seed) {
            this.Width = width;
            this.Height = height;
            this.Seed = seed;

            List<Octave> octaves = new List<Octave>();

            octaves.Add(new Octave(4, 3));
            octaves.Add(new Octave(8, 6));
            octaves.Add(new Octave(16, 12));
            octaves.Add(new Octave(32, 24));
            octaves.Add(new Octave(64, 96));

            this.PerlinNoiseProfile = new PerlinNoise2DProfile() {
                Width = width,
                Height = height,
                Seed = seed,
                Octaves = octaves
            };
        }
        public GenerationProfile(int width, int height, int seed, PerlinNoise2DProfile perlinNoiseProfile) {
            this.Width = width;
            this.Height = height;
            this.Seed = seed;
            this.PerlinNoiseProfile = perlinNoiseProfile;
        }
    }
}
