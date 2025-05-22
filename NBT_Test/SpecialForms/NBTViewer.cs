using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NBTEditorV2;

namespace NBT_Test
{
    public partial class NBTViewer : Form
    {
        private CompoundTag Compound { get; set; }

        public NBTViewer(CompoundTag compound)
        {
            InitializeComponent();

            this.Compound = compound;
        }

        private void NBTViewer_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = GetTagCompound(Compound, false);
        }

        private string GetTagCompound(CompoundTag tag, bool inList){
            string text = "";

            int count = tag.GetSize();

            string textTemp = "";
            for (int i = 0; i < tag.GetSize(); i++){
                Tag tag1 = tag.TagAt(i);

                if (tag1.Type == TagType.Compound){
                    textTemp += "\n\t" + GetTagCompound((CompoundTag)tag1.Item, false).Replace("\n", "\n\t");
                }else if (tag1.Type == TagType.List){
                    textTemp += "\n\t" + GetTagList((ListTag)tag1.Item, false).Replace("\n", "\n\t");
                }else if (tag1.Type == TagType.ByteArray){
                    textTemp += "\n\t" + GetTagByteArray((ByteArrayTag)tag1.Item, false);
                }else if (tag1.Type == TagType.LongArray){
                    textTemp += "\n\t" + GetTagLongArray((LongArrayTag)tag1.Item, false);
                }else if (tag1.Type == TagType.IntArray){
                    textTemp += "\n\t" + GetTagIntArray((IntArrayTag)tag1.Item, false);
                }else if (tag1.Type == TagType.String){
                    textTemp += "\n\t" + GetTagString((StringTag)tag1.Item, false);
                }else if (tag1.Type == TagType.Double){
                    textTemp += "\n\t" + GetTagDouble((DoubleTag)tag1.Item, false);
                }else if (tag1.Type == TagType.Short){
                    textTemp += "\n\t" + GetTagShort((ShortTag)tag1.Item, false);
                }else if (tag1.Type == TagType.Float){
                    textTemp += "\n\t" + GetTagFloat((FloatTag)tag1.Item, false);
                }else if (tag1.Type == TagType.Byte){
                    textTemp += "\n\t" + GetTagByte((ByteTag)tag1.Item, false);
                }else if (tag1.Type == TagType.Long){
                    textTemp += "\n\t" + GetTagLong((LongTag)tag1.Item, false);
                }else if (tag1.Type == TagType.Int){
                    textTemp += "\n\t" + GetTagInt((IntTag)tag1.Item, false);
                }
                //} else if (tag1.TagType == TagTypes.TAG_END) {
                //    textTemp += "\n\t" + getTagEND(tag1);
                //    count--;
                //}
            }

            text += "Tag_Compound(" + GetTagName(tag.Name, inList) + "): " + count + " " + (count == 1 ? "entry" : "entries") + "\n{";
            text += textTemp + "\n}";

            return text;
        }
        private string GetTagList(ListTag tag, bool inList){
            string text = "";

            text += "Tag_List(" + GetTagName(tag.Name, inList) + "): " + tag.Payload.Elements.Count + " " + (tag.Payload.Elements.Count == 1 ? "entry" : "entries") + "\n{";

            for (int i = 0; i < tag.Payload.Elements.Count; i++){
                object item = tag.Payload.Elements[i];

                if (tag.Payload.Type == TagType.Compound){
                    text += "\n\t" + GetTagCompound(new CompoundTag("", (List<Tag>)item), true).Replace("\t", "\t\t");
                }else if (tag.Payload.Type == TagType.List){
                    text += "\n\t" + GetTagList(new ListTag("", (ListPayload)item), true).Replace("\t", "\t\t");
                }else if (tag.Payload.Type == TagType.ByteArray){
                    text += "\n\t" + GetTagByteArray(new ByteArrayTag("", (byte[])item), true);
                }else if (tag.Payload.Type == TagType.LongArray){
                    text += "\n\t" + GetTagLongArray(new LongArrayTag("", (long[])item), true);
                }else if (tag.Payload.Type == TagType.IntArray){
                    text += "\n\t" + GetTagIntArray(new IntArrayTag("", (int[])item), true);
                }else if (tag.Payload.Type == TagType.String){
                    text += "\n\t" + GetTagString(new StringTag("", (string)item), true);
                }else if (tag.Payload.Type == TagType.Double){
                    text += "\n\t" + GetTagDouble(new DoubleTag("", (double)item), true);
                }else if (tag.Payload.Type == TagType.Short){
                    text += "\n\t" + GetTagShort(new ShortTag("", (short)item), true);
                }else if (tag.Payload.Type == TagType.Float){
                    text += "\n\t" + GetTagFloat(new FloatTag("", (float)item), true);
                }else if (tag.Payload.Type == TagType.Byte){
                    text += "\n\t" + GetTagByte(new ByteTag("", (byte)item), true);
                }else if (tag.Payload.Type == TagType.Long){
                    text += "\n\t" + GetTagLong(new LongTag("", (long)item), true);
                }else if (tag.Payload.Type == TagType.Int){
                    text += "\n\t" + GetTagInt(new IntTag("", (int)item), true);
                }//else if (tag.ListType == TagType.TAG_END){
                //    text += "\n\t" + getTagEND(tag1);
                //}
            }

            text += "\n}";

            return text;
        }
        private string GetTagByteArray(ByteArrayTag tag, bool inList){
            string text = "";

            text += "TAG_Byte_Array(" + GetTagName(tag.Name, inList) + "): " + "[" + tag.Array.Length + " bytes]";

            return text;
        }
        private string GetTagLongArray(LongArrayTag tag, bool inList){
            string text = "";

            text += "TAG_Long_Array(" + GetTagName(tag.Name, inList) + "): " + "[" + tag.Array.Length + " singles]";

            return text;
        }
        private string GetTagIntArray(IntArrayTag tag, bool inList){
            string text = "";

            text += "TAG_Int_Array(" + GetTagName(tag.Name, inList) + "): " + "[" + tag.Array.Length + " integers]";

            return text;
        }
        private string GetTagString(StringTag tag, bool inList){
            string text = "";

            text += "Tag_String(" + GetTagName(tag.Name, inList) + "): " + "\'" + tag.Text + "\'";

            return text;
        }
        private string GetTagDouble(DoubleTag tag, bool inList){
            string text = "";

            text += "Tag_Double(" + GetTagName(tag.Name, inList) + "): " + tag.Value.ToString().Replace(",", ".");

            return text;
        }
        private string GetTagShort(ShortTag tag, bool inList){
            string text = "";

            text += "Tag_Short(" + GetTagName(tag.Name, inList) + "): " + tag.Value;

            return text;
        }
        private string GetTagFloat(FloatTag tag, bool inList){
            string text = "";

            text += "Tag_Float(" + GetTagName(tag.Name, inList) + "): " + tag.Value.ToString().Replace(",", ".");

            return text;
        }
        private string GetTagByte(ByteTag tag, bool inList){
            string text = "";

            text += "Tag_Byte(" + GetTagName(tag.Name, inList) + "): " + tag.Value.ToString();

            return text;
        }
        private string GetTagLong(LongTag tag, bool inList){
            string text = "";

            text += "Tag_Long(" + GetTagName(tag.Name, inList) + "): " + tag.Value + "L";

            return text;
        }
        private string GetTagInt(IntTag tag, bool inList){
            string text = "";

            text += "Tag_Int(" + GetTagName(tag.Name, inList) + "): " + tag.Value;

            return text;
        }

        private string GetTagName(string name, bool inList){
            if (inList){
                return "None";
            }else{
                return name;
            }
        }
    }
}
