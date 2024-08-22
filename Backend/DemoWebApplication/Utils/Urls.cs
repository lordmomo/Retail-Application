using System.Security.AccessControl;

namespace DemoWebApplication.Utils
{
    public static class Urls
    {
        public const string loginApi = "/login";

        public const string registerApi = "/register";

        public const string showUserDetailsApi = "users/{username}";

        public const string editUserDetailsApi = "/users/{username}/edit-details";

        public const string changeUserPasswordApi = "/users/{username}/change-password";

        public const string removeUserApi = "/users/{username}/remove-user";

        public const string viewAllUsersApi = "users/view-all-users";

        public const string userTransactionHistoryApi = "users/{username}/transaction-history";

        public const string logoutApi = "/users/logout";

        public const string shopApi = "/users/{username}/shop";

        public const string showAllItemsApi = "/users/{username}/shop/all-items-in-shop";

        public const string addItemsToCartApi = "/users/{username}/shop/add-item-in-cart";

        public const string showItemsInCartApi = "/users/{username}/shop/show-items-in-cart";

        public const string removeItemsFromCartApi = "/users/{username}/shop/remove-item-in-cart";

        public const string deductUserBalanceApi = "/users/{username}/shop/deduct-balance";

        public const string addItemsInShopApi = "/users/{username}/shop/add-items-in-shop";
        public const string removeItemsInShopApi = "/users/{username}/shop/remove-items-in-shop";
        public const string updateItemsInShopApi = "/users/{username}/shop/update-items-in-shop";
        public const string searchItemInShop = "/users/shop/item/{shopItem}";
        public const string saveFavouriteItem = "users/{username}/favourite-item";
        public const string getUserFavouriteItems = "users/{username}/get-favourite-item";


        public const string createRoleApi = "/roles/create-role";
        public const string listAllRolesApi = "/roles/get-all-roles";
        public const string editRoleApi = "/roles/edit-role";
        public const string deleteRoleApi = "/roles/delete-role/{roleId}";
        public const string assginRoleToUser = "/roles/assign-role";

    }
}
