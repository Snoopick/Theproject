using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace jimBot {
    class CRUD {
        private static Dictionary<string, string> dataArray = new Dictionary<string, string> { };
        public static string DBPath = @"..\\..\\..\\data\\";

        public static MenuList[] GetMenuList(string dataFile = "") {
            if (dataFile.Equals(string.Empty)) {
                Logger.Log("ERROR", "No file data");
            }

            MenuList[] data = new MenuList[] { };

            var serializer = new XmlSerializer(typeof(MenuList[]));
            using (var reader = new XmlTextReader(CRUD.DBPath + "menu.xml")) {
                data = (MenuList[])serializer.Deserialize(reader);
            }

            return data;
        }

        public static Menu[] GetMenu(string dataFile = "") {
            if (dataFile.Equals(string.Empty)) {
                Logger.Log("ERROR", "No file data");
            }

            Menu[] data = new Menu[] { };

            var serializer = new XmlSerializer(typeof(Menu[]));
            using (var reader = new XmlTextReader(CRUD.DBPath + dataFile)) {
                data = (Menu[])serializer.Deserialize(reader);
            }

            return data;
        }

        public static string GetActionMenu () {
            string actionMenu = "1. Выбрать меню\r\n" +
                "2. Посмотреть заказ\r\n" +
                "3. Удалить позицию из заказа\r\n" +
                "4. Оформить заказа";

            return actionMenu;
        }
    }
}
