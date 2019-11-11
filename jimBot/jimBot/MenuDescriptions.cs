using System;
using System.Collections.Generic;
using System.Text;

namespace jimBot {
    public class MenuList {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }

        public MenuList() { }

        public MenuList(int id, string name, string description, string filepath) {
            Id = id;
            Name = name;
            Description = description;
            FilePath = filepath;
        }
    }

    public class Menu {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }

        public Menu() { }

        public Menu(int id, string name, string description, string price) {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
        }
    }
}
