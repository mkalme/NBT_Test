using System;

namespace CustomWorldGenerator
{
    public class Biome
    {
        public string Namespace { get; set; }
        public int ID { get; set; }
        public Range TemperatureRange { get; set; }    
        public Range HumidityRange { get; set; }

        public Biome() {
            this.Namespace = "";
            this.ID = 0;
            this.TemperatureRange = new Range();
            this.HumidityRange = new Range();
        }
        public Biome(string nameSpace, int id, Range temperatureRange, Range humidityRange) {
            this.Namespace = nameSpace;
            this.ID = id;
            this.TemperatureRange = temperatureRange;
            this.HumidityRange = humidityRange;
        }
    }

    public class Range {
        public double Start { get; set; }
        public double End { get; set; }

        public Range() {
            this.Start = 0;
            this.End = 0;
        }
        public Range(int start, int end) {
            this.Start = start;
            this.End = end;
        }
    }
}