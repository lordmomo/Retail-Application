
namespace DemoWebApplication.Constants
{
    public static class Message
    {
        public const string InsufficientBalance = "Insufficient balance. Transaction cannot be processed.";
        public const string ItemNotFound = "Item not found.";
        public const string CacheUpdatedAfterDataUpdate = "updated cache after data updated";
        public const string CacheUpdatedAfterDataRemoval = "updated cache after data removed";
        public const string CacheUpdatedAfterDataAdded = "updated cache after data added";
        public const string CacheCreatedForListofItems = "Cache for list of items created.";


        public const string ErrorWhileCacheUpdatedAfterDataUpdate = "updated cache after data updated";
        public const string ErrorWhileCacheUpdatedAfterDataRemoval = "updated cache after data removed";
        public const string ErrorWhileCacheUpdatedAfterDataAdded = "updated cache after data added";

        public const string ErrorWithAddingItemInDb = "Error adding item in database";


        public const string RoleAlreadyExists = "Role Already exists";
        public const string InvalidLoginAttempt = "Invalid Login Attempt";
        public const string ErrorUpdatingProfilePicture = "Error updating user profile picture.";
        public const string InternalServerError = "Internal server error.";
        public const string NoItemInCart = "No item added in cart";

        public const string FileNotFound = "File not found.";
        public const string NoFileUploadedMessage= "No file uploaded.";

        

        public const string UserNotFound = "User not found.";
        public const string SuccessRegisteringUser = "User registered successfully.";
        public const string ErrorRegisteringUser = "Error registering user";

        public const string SuccessCreatingingRoleMessage = "Role created successfully";
        public const string SuccessEditingRoleMessage = "Role edited successfully";
        public const string SuccessDeletingRoleMessage = "Role deletedd successfully";

        public const string ErrorCreatingingRoleMessage = "Error creating role";
        public const string ErrorEditingRoleMessage = "Error editing role";
        public const string ErrorDeletingRoleMessage = "Error deleting role";

        public const string RoleNotFoundMessage = "RoleName not found";
        public const string RoleNameBadRequestMessage = "RoleName cannot be null or empty.";

        
        
        public const string InvalidCredentialsMessage = "Invalid username or password.";

        public const string FailedToDeduceBalanceMessage = "Failed to deduce user balance!!!";
        public const string SuccessfulDeduceBalanceMessage = "Successfully deduced user balance";



        public const string ItemAddedToShop = "Item added in shop";
        public const string ErrorAddingItemToShop = "Error adding item in shop";

        public const string ItemEditedOfShop = "Item edited of shop";
        public const string ErrorEditingItemOfShop = "Error editing item of shop";

        public const string ItemDeletedFromShop = "Item deleted from shop";
        public const string ErrorDeletingItemFromShop = "Error deleting item from shop";

        public const string ProfilePictureUpdatedMessage = "Profile picture updated successfully";
        public const string UserDetailsUpdatedMessage = "User details updated successfully";

        public const string UserPasswordUpdatedMessage = "User password updated successfully";
        public const string FailedUserPasswordChangeMessage = "Failed to change user password";
        public const string UserPasswordWrongMessage = "User password is wrong";


        public const string NoUserTransactionMessage = "No transaction history present";
        public const string ItemAlreadyInFavourites = "Product already exists in favourites.";
        public const string ItemAddedInFavourites = "Product added to favourites.";

        public const string AssignRoleBadRequestMessage = "Bad request while assigning role to user";

        public const string RoleAssignedSuccessfullyMessage = "Successfully assigned role to user";
    }
}
