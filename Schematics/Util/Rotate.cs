using System;
using WorldEditor;
using System.Collections.Generic;

namespace Schematics.Util {
    class Rotate {
        public static Schematic RotateSchematic(short[,,] blockID, List<Property>[,,] properties, params SchematicsRotate[] rotationOptions) {
            Schematic schematic = new Schematic(blockID, properties);
            
            for (int i = 0; i < rotationOptions.Length; i++) {
                blockID = schematic.GetBlockID();
                properties = schematic.GetBlockProperties();

                switch (rotationOptions[i]) {
                    case SchematicsRotate.RotateX90:
                        schematic = RotateOnXAxis90(blockID, properties);
                        break;
                    case SchematicsRotate.RotateX180:
                        schematic = RotateOnXAxis180(blockID, properties);
                        break;
                    case SchematicsRotate.RotateX270:
                        schematic = RotateOnXAxis270(blockID, properties);
                        break;
                    case SchematicsRotate.RotateY90:
                        schematic = RotateOnYAxis90(blockID, properties);
                        break;
                    case SchematicsRotate.RotateY180:
                        schematic = RotateOnYAxis180(blockID, properties);
                        break;
                    case SchematicsRotate.RotateY270:
                        schematic = RotateOnYAxis270(blockID, properties);
                        break;
                    case SchematicsRotate.RotateZ90:
                        schematic = RotateOnZAxis90(blockID, properties);
                        break;
                    case SchematicsRotate.RotateZ180:
                        schematic = RotateOnZAxis180(blockID, properties);
                        break;
                    case SchematicsRotate.RotateZ270:
                        schematic = RotateOnZAxis270(blockID, properties);
                        break;
                }
            }
            
            return schematic;
        }
        
        //Rotate on X - Axis
        private static Schematic RotateOnXAxis90(short[,,] blockID, List<Property>[,,] properties) {
            var newBlockID = new short[blockID.GetLength(0), blockID.GetLength(2), blockID.GetLength(1)];
            var newProperties = new List<Property>[properties.GetLength(0), properties.GetLength(2), properties.GetLength(1)];

            int newHeight = newBlockID.GetLength(1);
            for (int y = 0; y < newHeight; y++) {
                for (int z = 0; z < newBlockID.GetLength(2); z++) {
                    for (int x = 0; x < newBlockID.GetLength(0); x++) {
                        int newY = z;
                        int newZ = newHeight - y - 1;

                        newBlockID[x, y, z] = blockID[x, newY, newZ];
                        newProperties[x, y, z] = properties[x, newY, newZ];
                    }
                }
            }

            return new Schematic(newBlockID, newProperties);
        }
        private static Schematic RotateOnXAxis180(short[,,] blockID, List<Property>[,,] properties) {
            var newBlockID = new short[blockID.GetLength(0), blockID.GetLength(1), blockID.GetLength(2)];
            var newProperties = new List<Property>[properties.GetLength(0), properties.GetLength(1), properties.GetLength(2)];

            int newHeight = newBlockID.GetLength(1);
            int newLength = newBlockID.GetLength(2);
            for (int y = 0; y < newHeight; y++) {
                for (int z = 0; z < newLength; z++) {
                    for (int x = 0; x < newBlockID.GetLength(0); x++) {
                        int newY = newHeight - y - 1;
                        int newZ = newLength - z - 1;

                        newBlockID[x, y, z] = blockID[x, newY, newZ];
                        newProperties[x, y, z] = properties[x, newY, newZ];
                    }
                }
            }

            return new Schematic(newBlockID, newProperties);
        }
        private static Schematic RotateOnXAxis270(short[,,] blockID, List<Property>[,,] properties)
        {
            var newBlockID = new short[blockID.GetLength(0), blockID.GetLength(2), blockID.GetLength(1)];
            var newProperties = new List<Property>[properties.GetLength(0), properties.GetLength(2), properties.GetLength(1)];

            int newLength = newBlockID.GetLength(2);
            for (int y = 0; y < newBlockID.GetLength(1); y++) {
                for (int z = 0; z < newLength; z++) {
                    for (int x = 0; x < newBlockID.GetLength(0); x++) {
                        int newY = newLength - z - 1;
                        int newZ = y;

                        newBlockID[x, y, z] = blockID[x, newY, newZ];
                        newProperties[x, y, z] = properties[x, newY, newZ];
                    }
                }
            }

            return new Schematic(newBlockID, newProperties);
        }

        //Rotate on Y - Axis
        private static Schematic RotateOnYAxis90(short[,,] blockID, List<Property>[,,] properties)
        {
            var newBlockID = new short[blockID.GetLength(2), blockID.GetLength(1), blockID.GetLength(0)];
            var newProperties = new List<Property>[properties.GetLength(2), properties.GetLength(1), properties.GetLength(0)];

            int newWidth = newBlockID.GetLength(0);
            for (int y = 0; y < newBlockID.GetLength(1); y++) {
                for (int z = 0; z < newBlockID.GetLength(2); z++) {
                    for (int x = 0; x < newWidth; x++) {
                        int newX = z;
                        int newZ = newWidth - x - 1;

                        newBlockID[x, y, z] = blockID[newX, y, newZ];
                        newProperties[x, y, z] = properties[newX, y, newZ];
                    }
                }
            }

            return new Schematic(newBlockID, newProperties);
        }
        private static Schematic RotateOnYAxis180(short[,,] blockID, List<Property>[,,] properties)
        {
            var newBlockID = new short[blockID.GetLength(0), blockID.GetLength(1), blockID.GetLength(2)];
            var newProperties = new List<Property>[properties.GetLength(0), properties.GetLength(1), properties.GetLength(2)];

            int newLength = newBlockID.GetLength(2);
            int newWidth = newBlockID.GetLength(0);
            for (int y = 0; y < newBlockID.GetLength(1); y++) {
                for (int z = 0; z < newLength; z++) {
                    for (int x = 0; x < newWidth; x++) {
                        int newX = newWidth - x - 1;
                        int newZ = newLength - z - 1;

                        newBlockID[x, y, z] = blockID[newX, y, newZ];
                        newProperties[x, y, z] = properties[newX, y, newZ];
                    }
                }
            }

            return new Schematic(newBlockID, newProperties);
        }
        private static Schematic RotateOnYAxis270(short[,,] blockID, List<Property>[,,] properties)
        {
            var newBlockID = new short[blockID.GetLength(2), blockID.GetLength(1), blockID.GetLength(0)];
            var newProperties = new List<Property>[properties.GetLength(2), properties.GetLength(1), properties.GetLength(0)];

            int newLength = newBlockID.GetLength(2);
            for (int y = 0; y < newBlockID.GetLength(1); y++) {
                for (int z = 0; z < newLength; z++) {
                    for (int x = 0; x < newBlockID.GetLength(0); x++) {
                        int newX = newLength - z - 1;
                        int newZ = x;

                        newBlockID[x, y, z] = blockID[newX, y, newZ];
                        newProperties[x, y, z] = properties[newX, y, newZ];
                    }
                }
            }

            return new Schematic(newBlockID, newProperties);
        }

        //Rotate on Z - Axis
        private static Schematic RotateOnZAxis90(short[,,] blockID, List<Property>[,,] properties)
        {
            var newBlockID = new short[blockID.GetLength(1), blockID.GetLength(0), blockID.GetLength(2)];
            var newProperties = new List<Property>[properties.GetLength(1), properties.GetLength(0), properties.GetLength(2)];

            int newWidth = newBlockID.GetLength(0);
            for (int y = 0; y < newBlockID.GetLength(1); y++) {
                for (int z = 0; z < newBlockID.GetLength(2); z++) {
                    for (int x = 0; x < newWidth; x++) {
                        int newX = y;
                        int newY = newWidth - x - 1;

                        newBlockID[x, y, z] = blockID[newX, newY, z];
                        newProperties[x, y, z] = properties[newX, newY, z];
                    }
                }
            }

            return new Schematic(newBlockID, newProperties);
        }
        private static Schematic RotateOnZAxis180(short[,,] blockID, List<Property>[,,] properties)
        {
            var newBlockID = new short[blockID.GetLength(0), blockID.GetLength(1), blockID.GetLength(2)];
            var newProperties = new List<Property>[properties.GetLength(0), properties.GetLength(1), properties.GetLength(2)];

            int newWidth = newBlockID.GetLength(0);
            int newHeight = newBlockID.GetLength(1);
            for (int y = 0; y < newHeight; y++) {
                for (int z = 0; z < newBlockID.GetLength(2); z++) {
                    for (int x = 0; x < newWidth; x++) {
                        int newX = newWidth - x - 1;
                        int newY = newHeight - y - 1;

                        newBlockID[x, y, z] = blockID[newX, newY, z];
                        newProperties[x, y, z] = properties[newX, newY, z];
                    }
                }
            }

            return new Schematic(newBlockID, newProperties);
        }
        private static Schematic RotateOnZAxis270(short[,,] blockID, List<Property>[,,] properties)
        {
            var newBlockID = new short[blockID.GetLength(1), blockID.GetLength(0), blockID.GetLength(2)];
            var newProperties = new List<Property>[properties.GetLength(1), properties.GetLength(0), properties.GetLength(2)];

            int newWidth = newBlockID.GetLength(0);
            int newHeight = newBlockID.GetLength(1);
            for (int y = 0; y < newHeight; y++) {
                for (int z = 0; z < newBlockID.GetLength(2); z++) {
                    for (int x = 0; x < newWidth; x++) {
                        int newX = newHeight - y - 1;
                        int newY = x;

                        newBlockID[x, y, z] = blockID[newX, newY, z];
                        newProperties[x, y, z] = properties[newX, newY, z];
                    }
                }
            }

            return new Schematic(newBlockID, newProperties);
        }
    }
}
