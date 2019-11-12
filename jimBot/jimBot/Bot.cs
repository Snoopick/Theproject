using System;
using System.Collections.Generic;
using System.Threading;

namespace jimBot {
    public class Bot {
        MenuList[] menuData;
        Menu[] menu;
        int userId;
        int menuNum = 0;
        int menuCount = 0;
        Dictionary<int, string[]> registeredUsers = new Dictionary<int, string[]> { };
        Dictionary<int, string> menuList = new Dictionary<int, string> { };
        Dictionary<int, string> menuItemData = new Dictionary<int, string> { };

        public delegate void EmailsEvents();

        public event EmailsEvents SubscribeUser;

        public Bot() {
            Logger.Log("INFO", "Get random user id");
            this.userId = Helpers.GetRandom();
            Logger.Log("INFO", "user id is " + this.userId);
            this.menuData = this.GetMenuList();
        }

        public void Run () {
            Logger.Log("INFO", "Print menu");
            this.printMenu();

            if (!this.menuList[this.menuNum].Equals(string.Empty)) {
                int action = 0;

                while (action != 1 || action != 4) {
                    Helpers.Write(CRUD.GetActionMenu());
                    Logger.Log("INFO", "Get user choise");
                    action = Convert.ToInt32(Helpers.GetInput());
                    Logger.Log("INFO", "User choised " + action);

                    switch (action) {
                        case 1:
                            this.printMenuList();
                            break;
                        case 2:
                            this.GetUserOrder();
                            break;
                        case 3:
                            this.DeleteFromOrder();
                            break;
                        case 4:
                            this.Ordering();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public MenuList[] GetMenuList() {
            Logger.Log("INFO", "Get menu");
            MenuList[] data = CRUD.GetMenuList("menu.xml");

            return data;
        }

        public Menu[] GetMenu(string menu = "") {
            Menu[] data = CRUD.GetMenu(menu);

            return data;
        }

        public void printMenu () {
            Helpers.Write("Выберите меню:");

            foreach (MenuList menu in this.menuData) {
                Helpers.Write($"{menu.Id}. {menu.Name}");
                if (!this.menuList.ContainsKey(menu.Id)) {
                    this.menuList.Add(menu.Id, menu.FilePath);
                    this.menuCount++;
                }
            }

            while (this.menuNum == 0 || this.menuNum.Equals(string.Empty)) {
                string enteredMenu = Helpers.GetInput().ToString();

                try {
                    for (int i = 0; i <= this.menuCount; i++) {
                        if (Convert.ToInt32(enteredMenu) == i) {
                            this.menuNum = i;
                        }
                    }
                } catch (Exception e) {
                    Logger.Log("ERROR", "Entered not num " + e.Message);
                    throw new Exception(e.Message);
                }
            }
        }

        public void printMenuList () {
            Logger.Log("INFO", "Menu is printed");
            Helpers.Write(this.menuList[this.menuNum]);

            this.menu = this.GetMenu(this.menuList[this.menuNum]);

            Helpers.Write("Выберите из меню:");
            foreach (Menu menu in this.menu) {
                Helpers.Write($"{menu.Id}. {menu.Name}\r\nОписание: {menu.Description}\r\nЦена: {menu.Price}");
                if (!this.menuItemData.ContainsKey(menu.Id)) {
                    this.menuItemData.Add(menu.Id, $"{menu.Id}. {menu.Name} Цена: {menu.Price}");
                    this.menuCount++;
                }
            }

            Helpers.Write("0. Выход");

            this.getUserMenuChoise();
        }

        public void getUserMenuChoise() {
            Logger.Log("INFO", "Order Generate");
            int menuItemId = 1;
            int menuItemCount = 0;

            do {
                Helpers.Write("Введите номер позиции или введите 0 для выхода:");
                menuItemId = Convert.ToInt32(Helpers.GetInput());
                Logger.Log("INFO", "Getted item from menu " + menuItemId);

                #region add item in cart
                if (menuItemId != 0) {
                    Helpers.Write("Укажите кол-во:");
                    menuItemCount = Convert.ToInt32(Helpers.GetInput());
                    Logger.Log("INFO", "Getted item count from menu " + menuItemCount);

                    string cartItem = menuItemId + ";" + menuItemCount;

                    UserCart.AddToCart(cartItem, this.userId);
                }
                #endregion

            } while (menuItemId != 0);
        }

        public void GetUserOrder() {
            Logger.Log("INFO", "Get user order");

            Dictionary<int, string>  userCart = UserCart.GetCart();
            string order = "";

            if (userCart.ContainsKey(this.userId)) {
                string[] userCartData = userCart[this.userId].Split(';');

                for (int i = 0; i < userCartData.Length; i++) {
                    string[] item = userCartData[i].Split(':');

                    if (item[0].Equals("ID")) {
                        order += this.menuItemData[Convert.ToInt32(item[1])] + "\r\n";
                    }
                }

                Helpers.Write(order);
            } else {
                Helpers.Write("Ваша корзина пуста");
            }
        }

        public void DeleteFromOrder () {
            Logger.Log("INFO", "Delete item from cart");

            Helpers.Write("Введите id, который нужно удалить");
            this.GetUserOrder();
            int deleteId = Convert.ToInt32(Helpers.GetInput());
            Logger.Log("INFO", "Delete " + deleteId + "position");

            UserCart.DeleteFromCart(this.userId, deleteId);
        }

        public void Ordering() {
            Logger.Log("INFO", "Generate orger");

            string userName;
            string userSurname;
            string userLastname;
            string userAddress;
            string userEmail;

            Helpers.Write("Оформление заказа:");
            Helpers.Write("Введите фамилию:");
            userLastname = Convert.ToString(Helpers.GetInput());

            Helpers.Write("Введите имя:");
            userName = Convert.ToString(Helpers.GetInput());

            Helpers.Write("Введите отчество:");
            userSurname = Convert.ToString(Helpers.GetInput());

            Helpers.Write("Введите адрес для доставки:");
            userAddress = Convert.ToString(Helpers.GetInput());

            Helpers.Write("Введите почту:");
            userEmail = Convert.ToString(Helpers.GetInput());

            string body = $"Уважаемый {userLastname} {userName} {userSurname}<br/>\r\n" +
                $"Вы оформили заказ у JimBot, состав заказа:<br/>\r\n";
            this.GetUserOrder();

            string[] userData = new string[6] { userLastname, userName, userSurname, userAddress, userEmail, body };
            this.registeredUsers.Add(this.userId, userData);

            Helpers.SendEmailAsync(userEmail, "Новый заказа", body).GetAwaiter();

            this.SubscribeUser = this.Subscribe;

            UserCart.ClearCart(this.userId);
            Helpers.Write("Ваш заказ оформлен, с вами свяжутся для уточнения");
        }

        public void Subscribe() {
            Thread.Sleep(1000);
            Helpers.SendEmailAsync(this.registeredUsers[this.userId][4], "Заказ скомплектован", this.registeredUsers[this.userId][5]).GetAwaiter();
            Thread.Sleep(1000);
            Helpers.SendEmailAsync(this.registeredUsers[this.userId][4], "Заказ доставлен курьером", this.registeredUsers[this.userId][5]).GetAwaiter();
            Thread.Sleep(1000);
            Helpers.SendEmailAsync(this.registeredUsers[this.userId][4], "Заказ оплачен", this.registeredUsers[this.userId][5]).GetAwaiter();
        }
    }
}
