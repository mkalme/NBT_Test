using System;
using System.Collections.Generic;

namespace RegionMapping {
    public class Direction {
        public static readonly Direction Land_Top = new Direction(6, 30);
        public static readonly Direction Land_Left = new Direction(6, 30);
        public static readonly Direction Land_Bottom = new Direction(3, 15);
        public static readonly Direction Land_Right = new Direction(3, 7);

        public static readonly Direction Water_Top = new Direction(4, 25);
        public static readonly Direction Water_Left = new Direction(4, 25);
        public static readonly Direction Water_Bottom = new Direction(3, 15);
        public static readonly Direction Water_Right = new Direction(3, 7);

        public int MaxHeight { get; set; }
        public int MaxColor { get; set; }
        public int ColorDifference { get; set; }

        public Direction(int maxHeight, int colorDifference) {
            this.MaxHeight = maxHeight;
            this.MaxColor = maxHeight * colorDifference;
            this.ColorDifference = colorDifference;
        }
        public Direction(int maxHeight, int colorDifference, int maxColor){
            this.MaxHeight = maxHeight;
            this.MaxColor = maxColor;
            this.ColorDifference = colorDifference;
        }
    }

    public class DirectionProfile {
        public Dictionary<Directions, Direction> Land = new Dictionary<Directions, Direction>() {
            {Directions.Top, Direction.Land_Top},
            {Directions.Left, Direction.Land_Left},
            {Directions.Bottom, Direction.Land_Bottom},
            {Directions.Right, Direction.Land_Right}
        };
        public Dictionary<Directions, Direction> Water = new Dictionary<Directions, Direction>() {
            {Directions.Top, Direction.Water_Top},
            {Directions.Left, Direction.Water_Left},
            {Directions.Bottom, Direction.Water_Bottom},
            {Directions.Right, Direction.Water_Right}
        };

        public static readonly DirectionProfile Heavy = new DirectionProfile() {
            Land = new Dictionary<Directions, Direction>() {
                {Directions.Top, new Direction(4, 40)},
                {Directions.Left, new Direction(4, 40)},
                {Directions.Bottom, new Direction(2, 20)},
                {Directions.Right, new Direction(2, 9)}
            }
        };
        public static readonly DirectionProfile Medium = new DirectionProfile();
        public static readonly DirectionProfile Light = new DirectionProfile() {
            Land = new Dictionary<Directions, Direction>() {
                {Directions.Top, new Direction(6, 15)},
                {Directions.Left, new Direction(6, 15)},
                {Directions.Bottom, new Direction(3, 7)},
                {Directions.Right, new Direction(3, 3)}
            }
        };
    }
    public enum Directions {
        Top,
        Right,
        Left,
        Bottom,
        None
    }
}
