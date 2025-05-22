using System;

namespace WorldEditor
{
    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Property() {
            this.Name = "";
            this.Value = "";
        }
        public Property(string name, string value) {
            this.Name = name;
            this.Value = value;
        }
    }
}
