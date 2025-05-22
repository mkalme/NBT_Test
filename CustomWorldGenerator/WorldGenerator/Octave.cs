using System;

namespace CustomWorldGenerator
{
    public class Octave
    {
        public double Frequency { get; set; }
        public double Amplitude { get; set; }

        public Octave() {
            this.Frequency = 1;
            this.Amplitude = 1;
        }
        public Octave(double frequency, double amplitude) {
            this.Frequency = frequency;
            this.Amplitude = amplitude;
        }
    }
}
