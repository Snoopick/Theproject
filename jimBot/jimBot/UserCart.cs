using System;
using System.Collections.Generic;
using System.Text;

namespace jimBot {
    class UserCart {
        public static Dictionary<int, string> userCart = new Dictionary<int, string>();

        public static void AddToCart(string userCartItem, int userId = 0) {
            Logger.Log("INFO", "Try add to cart");

            if (userId == 0) {
                Logger.Log("ERROR", "No user ID");
            }

            if (userCartItem.Equals(string.Empty)) {
                Logger.Log("ERROR", "No user cart data");
            }

            string existUserCart = "";

            String[] items = userCartItem.Split(';');
            string newString = "";

            newString = "ID:" + items[0] + ";COUNT:" + items[1] + ";";

            if (UserCart.userCart.ContainsKey(userId)) {
                existUserCart = UserCart.userCart[userId];
                existUserCart += newString;

                UserCart.userCart.Remove(userId);
                UserCart.userCart.Add(userId, existUserCart);
            } else {               
                UserCart.userCart.Add(userId, newString);
            }

            Logger.Log("INFO", "Items added to cart");
        }

        public static void DeleteFromCart(int userId = 0, int cartItem = 0) {
            Logger.Log("INFO", "Try deleting from cart");

            Dictionary<int, string> userCart = UserCart.GetCart();
            string newOrder = "";
            string[] userCartData = userCart[userId].Split(';');
            bool needAddCount = false;

            for (int i = 0; i < userCartData.Length; i++) {
                string[] item = userCartData[i].Split(':');

                if (item[0].Equals("ID") && Convert.ToInt32(item[1]) != cartItem) {
                    newOrder += userCartData[i] + ";";
                    needAddCount = true;
                } else if (needAddCount) {
                    newOrder += userCartData[i] + ";";
                    needAddCount = false;
                }
            }

            UserCart.updateUserCart(userId, newOrder);
        }

        public static void ClearCart(int userId = 0) {
            UserCart.updateUserCart(userId, "");
        }

        public static Dictionary<int, string> GetCart() {
            return UserCart.userCart;
        }

        public static void updateUserCart(int userId, string data) {
            Logger.Log("INFO", "Update cart");

            if (UserCart.userCart.ContainsKey(userId)) {
                UserCart.userCart.Remove(userId);
                UserCart.userCart.Add(userId, data);
            }
        }
    }
}
