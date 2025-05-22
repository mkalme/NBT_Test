using System;
using System.IO;
using NBTEditorV2;
using WorldEditor;
using System.Collections.Generic;

namespace Schematics {
    public class Schematic {
        private short[,,] BlockID { get; set; }
        private List<Property>[,,] BlockProperties { get; set; }

        public Size Size { get; private set; }

        public Schematic(int width, int height, int length) {
            this.Size = new Size(width, height, length);
            this.BlockID = new short[width, height, length];
            this.BlockProperties = new List<Property>[width, height, length];
        }
        public Schematic(short[,,] blockID, List<Property>[,,] blockProperties) {
            this.Size = new Size(blockID.GetLength(0), blockID.GetLength(1), blockID.GetLength(2));
            this.BlockID = blockID;
            this.BlockProperties = blockProperties;
        }
        public Schematic(Block[,,] blocks) {
            this.Size = new Size(blocks.GetLength(0), blocks.GetLength(1), blocks.GetLength(2));

            for (int y = 0; y < blocks.GetLength(1); y++) {
                for (int z = 0; z < blocks.GetLength(2); z++) {
                    for (int x = 0; x < blocks.GetLength(0); x++) {
                        BlockID[x, y, z] = blocks[x, y, z].ID;
                        BlockProperties[x, y, z] = blocks[x, y, z].Properties;
                    }
                }
            }
        }

        public short[,,] GetBlockID() {
            return BlockID;
        }
        public List<Property>[,,] GetBlockProperties() {
            return BlockProperties;
        }

        public short GetID(int x, int y, int z) {
            return BlockID[x, y, z];
        }
        public List<Property> GetProperties(int x, int y, int z)
        {
            return BlockProperties[x, y, z];
        }

        public void SetBlock(int x, int y, int z, short id, List<Property> property) {
            BlockID[x, y, z] = id;
            BlockProperties[x, y, z] = property;
        }

        //Rotate
        public void Rotate(params SchematicsRotate[] rotationOptions) {
            Schematic schematic = Util.Rotate.RotateSchematic(BlockID, BlockProperties, rotationOptions);

            BlockID = schematic.GetBlockID();
            BlockProperties = schematic.GetBlockProperties();
            Size = schematic.Size;
        }

        //Extract
        public static Schematic Extract(World world, int x1, int y1, int z1, int x2, int y2, int z2) {
            Cords cords;
            Size size;

            Scripts.SetCordsAndSize(new Cords(x1, y1, z1), new Cords(x2, y2, z2), out cords, out size);

            return Util.Extract.ExtractSchematic(world, cords, size);
        }
        public static Schematic Extract(World world, int x, int y, int z, Size size) {
            Schematic schematic = Util.Extract.ExtractSchematic(world, new Cords(x, y, z), size);

            return schematic;
        }

        //Lay
        public void Lay(World world, int x, int y, int z) {
            Util.Lay.LaySchematic(world, this, x, y, z);
        }

        //Save & Load
        public static Schematic FromFile(string filePath){
            Schematic schematic = Load.FromFile(filePath);

            return schematic;
        }
        public void SaveToFile(string directoryPath, string name) {
            CompoundTag tag = Save.SaveToFile(this);

            byte[] bytes = CompoundTag.GetTagBytes(tag).ToArray();

            string filePath = directoryPath + @"\" + name + ".dat";

            File.WriteAllBytes(filePath, bytes);
        }
    }
}
