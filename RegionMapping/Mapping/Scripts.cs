using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using WorldEditor;

namespace RegionMapping {
    public class Scripts {
        //Get columns
        private static readonly int WaterID = ID.BlockIDReverse["minecraft:water"];
        public static int MaxHeight = 255;

        public static Column GetColumn(Chunk chunk, List<Section> sectionsInOrder, int x, int z){
            int xzIndex = z * 16 + x;

            for (int b = sectionsInOrder.Count - 1; b > -1; b--) {
                for (int y = 15; y > -1; y--) {
                    if (sectionsInOrder[b].YIndex * 16 + y > MaxHeight) {
                        continue;
                    }

                    short blockID = sectionsInOrder[b].BlockID[y * 256 + xzIndex];

                    if (blockID == WaterID) {
                        return GetWaterColumn(chunk, sectionsInOrder, x, z, b);
                    } else {
                        BlockColorType colorType = GetBlockColorType(blockID);

                        if (colorType != BlockColorType.None) {
                            return GetLandColumn(chunk, sectionsInOrder[b], blockID, colorType, x, y, z);
                        }
                    }
                }
            }

            return new Column(ColumnType.Region);
        }

        private static WaterColumn GetWaterColumn(Chunk chunk, List<Section> sections, int xp, int zp, int bIndex){
            WaterColumn column = new WaterColumn();
            int xzIndex = zp * 16 + xp;

            bool waterFound = false;
            bool baseFound = false;
            for (int b = bIndex; b > -1; b--) {
                for (int y = 15; y > -1; y--) {
                    int blockIndex = y * 256 + xzIndex;

                    short blockID = sections[b].BlockID[blockIndex];
                    int height = (sections[b].YIndex * 16) + y;

                    if (blockID == WaterID) {
                        if (!waterFound) {
                            column.WaterHeight = (byte)height;
                            column.WaterColor = GetBlockColor(
                                blockID,
                                sections[b].GetProperties((short)blockIndex),
                                (short)chunk.Biomes[xp, zp],
                                GetBlockColorType(blockID)).ToArgb();

                            waterFound = true;
                        }
                    } else if (waterFound) {
                        BlockColorType colorType = GetBlockColorType(blockID);
                        if (colorType != BlockColorType.None) {
                            if (!baseFound) {
                                column.HeightException = BlockProperties.ExceptionList.ContainsKey(blockID);
                                column.ShadowException = BlockProperties.ExcludedList.Contains(blockID);
                                column.BaseBlockHeight = (byte)((sections[b].YIndex * 16) + y - (column.HeightException ? BlockProperties.ExceptionList[blockID] : 0));
                                column.BaseBlockColor = GetBlockColor(
                                                    blockID,
                                                    sections[b].GetProperties((short)blockIndex),
                                                    (short)chunk.Biomes[xp, zp],
                                                    colorType).ToArgb();

                                if (BlockProperties.ExceptionUndefinedList.Contains(blockID)) {
                                    baseFound = true;
                                } else {
                                    //Break
                                    y = -1;
                                    b = -1;
                                }
                            } else {
                                if (!BlockProperties.ExceptionUndefinedList.Contains(blockID)) {
                                    column.BaseBlockHeight = (byte)((sections[b].YIndex * 16) + y - (column.HeightException ? BlockProperties.ExceptionList[blockID] : 0));
                                    column.UndefinedHeightException = true;

                                    //Break
                                    y = -1;
                                    b = -1;
                                }
                            }
                        }
                    }
                }
            }

            return column;
        }
        private static LandColumn GetLandColumn(Chunk chunk, Section section, short blockID, BlockColorType colorType, int x, int y, int z){
            bool exception = BlockProperties.ExceptionList.ContainsKey(blockID);
            bool excluded = BlockProperties.ExcludedList.Contains(blockID);
            int height = (section.YIndex * 16) + y - (exception ? BlockProperties.ExceptionList[blockID] : 0);
            Color blockColor = GetBlockColor(
                                blockID,
                                section.GetProperties(x, y, z),
                                (short)chunk.Biomes[x, z],
                                colorType);

            //Set column
            LandColumn column = new LandColumn() {
                Color = blockColor.ToArgb(),
                Exception = exception,
                Excluded = excluded,
                Height = (byte)height
            };

            return column;
        }

        private static BlockColorType GetBlockColorType(short blockID){
            if (blockID == 0) {
                return BlockColorType.None;
            }

            if (BlockColors.BlockColor.ContainsKey(blockID)) {
                return BlockColorType.Block;
            } else if (BlockColors.BiomeColorList.ContainsKey(blockID)) {
                return BlockColorType.Biome;
            } else if (BlockColors.PropertyColor.ContainsKey(blockID)) {
                return BlockColorType.Property;
            } else {
                return BlockColorType.None;
            }
        }
        protected static Color GetBlockColor(short id, List<Property> properties, short biome, BlockColorType colorType){
            Color color = new Color();

            if (colorType == BlockColorType.Block) {
                color = BlockColors.BlockColor[id];
            } else if (colorType == BlockColorType.Property) {
                if (properties.Count > 0) {
                    if (BlockColors.PropertyColor[id].ContainsKey(properties[0].Value)) {
                        color = BlockColors.PropertyColor[id][properties[0].Value];
                    } else {
                        color = BlockColors.DefaultPropertyColor[id];
                    }
                } else {
                    color = BlockColors.DefaultPropertyColor[id];
                }
            } else if (colorType == BlockColorType.Biome) {
                if (BlockColors.BiomeColors.ContainsKey(biome)) {
                    if (BlockColors.BiomeColors[biome].ContainsKey(id)) {
                        color = BlockColors.BiomeColors[biome][id];
                    } else {
                        color = BlockColors.BiomeColorList[id];
                    }
                } else {
                    color = BlockColors.BiomeColorList[id];
                }
            }

            return color;
        }

        //Universal
        protected static int Mod(int number, int mod){
            int result = number % mod;

            if (result < 0) {
                result += mod;
            }

            return result;
        }
    }

    public enum BlockColorType {
        Block,
        Property,
        Biome,
        None
    }
    public struct MapRange {
        public int XStart { get; set; }
        public int ZStart { get; set; }
        public int XEnd { get; set; }
        public int ZEnd { get; set; }

        public static MapRange GetRange(List<WorldEditor.Region> regions){
            if (regions.Count > 0) {
                IEnumerable<WorldEditor.Region> indexesOrderX = regions.OrderBy(o => o.XIndex);
                IEnumerable<WorldEditor.Region> indexesOrderZ = regions.OrderBy(o => o.ZIndex);

                int xStart = indexesOrderX.First().XIndex;
                int zStart = indexesOrderZ.First().ZIndex;
                int xEnd = indexesOrderX.Last().XIndex;
                int zEnd = indexesOrderZ.Last().ZIndex;

                return new MapRange() { XStart = xStart, ZStart = zStart, XEnd = xEnd, ZEnd = zEnd };
            } else {
                return new MapRange() { XStart = 0, ZStart = 0, XEnd = 0, ZEnd = 0 };
            }
        }
        public static MapRange GetRange(List<RegionIndex> regionIndexes){
            if (regionIndexes.Count > 0) {
                IEnumerable<RegionIndex> indexesOrderX = regionIndexes.OrderBy(o => o.X);
                IEnumerable<RegionIndex> indexesOrderZ = regionIndexes.OrderBy(o => o.Z);

                int xStart = indexesOrderX.First().X;
                int zStart = indexesOrderZ.First().Z;
                int xEnd = indexesOrderX.Last().X;
                int zEnd = indexesOrderZ.Last().Z;

                return new MapRange() { XStart = xStart, ZStart = zStart, XEnd = xEnd, ZEnd = zEnd };
            } else {
                return new MapRange() { XStart = 0, ZStart = 0, XEnd = 0, ZEnd = 0 };
            }
        }
    }
}
