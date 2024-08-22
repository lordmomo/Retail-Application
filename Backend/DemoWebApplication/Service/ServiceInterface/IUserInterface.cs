using DemoWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebApplication.Service.ServiceInterface
{
    public interface IUserInterface
    {
        //public void SaveUserData(Person user);
        //public Person? GetUserByUsername(string username);

        public List<PersonDto> GetAlUsers();
        public Task<string> getRoleofUser(string? id);
        public string GetUniqueFilename(string fileName);
        public PersonDto? GetUserByUsername(string username);
        public List<Item> getUserFavouriteItems(PersonDto user);
        //public string? GetUserFromToken(string token);
        public List<SalesHistoryResponse> GetUserTransactionHistory(string username);
        public string saveFavouriteItem(PersonDto user, SelectedItem favouriteItem);
        //bool ValidateToken(string token);
    }
}
