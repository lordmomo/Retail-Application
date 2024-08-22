using Microsoft.AspNetCore.Mvc;
using DemoWebApplication.Models;
using DemoWebApplication.Utils;
using DemoWebApplication.Constants;
using DemoWebApplication.Service.ServiceInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using DemoWebApplication.Service.ServiceImplementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json.Linq;
using System.Web;
using AutoMapper;
using Microsoft.Build.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;


namespace DemoWebApplication.Controllers;

public class UserController : Controller
{
    public ApplicationDbContext db;

    private IUserInterface _userInterface;
    //private TokenServiceImpl _tokenServiceImpl;
    private readonly UserManager<Person> _userManager;
    private readonly SignInManager<Person> _signInManager;

    private IMapper mapper;

    public UserController(IUserInterface userInterface,
                            UserManager<Person> userManager,
                            //SignInManager<Person> signInManager, TokenServiceImpl tokenServiceImpl,
                            ApplicationDbContext applicationDbContext, 
                            IMapper mapper)
    {
        _userInterface = userInterface;
        this._userManager = userManager;
        //this._signInManager = signInManager;
        //this._tokenServiceImpl = tokenServiceImpl;
        this.db = applicationDbContext;
        this.mapper = mapper;
    }


    [HttpPost]
    [Route(Urls.registerApi)]
    public async Task<ApiResponse> RegisterUserForm([FromBody] RegisterUser user)
    {
        if (user == null)
            return new ApiResponse { success = false, message = Message.UserNotFound };

        //return Json(new { success = true, message = Message.UserNotFound });

        var dUser = mapper.Map<Person>(user);


        var result = await _userManager.CreateAsync(dUser, dUser.Password);
        var errors = new List<string>();

        var roleRsult = await _userManager.AddToRoleAsync(dUser, "User");
        if (!roleRsult.Succeeded)
        {
            errors.AddRange(result.Errors.Select(e => $"Failed to assign role User to user '{user.Username}': {e.Description}"));
        }

        if (result.Succeeded)
        {

            //return Json(new { success = true, message = Message.SuccessRegisteringUser });
            return new ApiResponse { success = true, message = Message.SuccessRegisteringUser };
        }
        else
        {
            return new ApiResponse { success = false, message = Message.ErrorRegisteringUser };

            //return Json(new { success = false, message = Message.ErrorRegisteringUser });
        }

    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin,Moderator")]
    [HttpGet("/users/{username}/profile-image")]
    public async Task<IActionResult> GetProfileImage(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if(user == null)
        {
            return NotFound(Message.UserNotFound);
        }
        var filePath = Path.Combine("wwwroot/images/profile-Images/", user.FilePath);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(Message.FileNotFound);
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "image/jpeg");
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin,Moderator")]
    [HttpPost]
    [Route("/users/{username}/add-profile-image")]
    public async Task<IActionResult> AddProfilePicture([FromForm] IFormFile file, string username)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(Message.NoFileUploadedMessage);
        }


        try
        {
            var userDto = _userInterface.GetUserByUsername(username);


        var user = await _userManager.FindByIdAsync(userDto.Id);

        if (user == null)
        {
            return NotFound(Message.UserNotFound);
        }

            var uniqueFilename = _userInterface.GetUniqueFilename(file.FileName);

            var filePath = Path.Combine("wwwroot/images/profile-Images/", uniqueFilename);


            using (var stream = System.IO.File.OpenWrite(filePath))
            {
                await file.CopyToAsync(stream);
            }

            user.FilePath = uniqueFilename;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {

                return Json(Message.ErrorUpdatingProfilePicture);

            }


            return Json(Message.ProfilePictureUpdatedMessage);
        }
        catch (Exception ex)
        {
            Log.Debug($"Error: {ex}");
            return StatusCode(500, Message.InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpGet]
    [Route(Urls.showUserDetailsApi)]
    public IActionResult ShowUserData(string username)
    {

        var user = _userInterface.GetUserByUsername(username);
        if (user != null)
        {
            return Json(user);
        }
        return StatusCode(404, Message.UserNotFound );
    }



    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin,Moderator")]
    [Route(Urls.editUserDetailsApi)]
    public IActionResult EditUserDetails([FromBody] EditUserDetailsDto userDetails, string username)
    {

        var checkUserDto = _userInterface.GetUserByUsername(username);

        if (checkUserDto == null)
        {
            return NotFound(Message.UserNotFound);
        } 

        var checkUser = mapper.Map<Person>(checkUserDto);

        

        //to be edited, put them in interface ---db
        var esvr = db.Users.FirstOrDefault(u => u.UserName == username);

        esvr.CustomerName = userDetails.CustomerName;
        esvr.Address = userDetails.Address;
        db.Update(esvr);
        db.SaveChanges();

        return Json(Message.UserDetailsUpdatedMessage);

    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin,Moderator")]
    [HttpPost]
    [Route(Urls.changeUserPasswordApi)]
    public async Task<IActionResult> EditPassword(string username, [FromBody] ChangePasswordDto changePasswordDto)
    {
        var checkUser = _userInterface.GetUserByUsername(username);
        if (checkUser == null)
        {
            return NotFound(Message.UserNotFound);
        }

        //to be edited, put them in interface ---db

        var esvr = db.Users.FirstOrDefault(u => u.UserName == username);
        if (esvr.Password == changePasswordDto.OldPassword)
        {
            //esvr.Password = changePasswordDto.NewPassword;
            var result = await _userManager.ChangePasswordAsync(esvr, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Json(Message.UserPasswordUpdatedMessage);

            }
            else
            {
                return Json(Message.FailedUserPasswordChangeMessage);
            }

        }
        else
        {
            return Json(Message.UserPasswordWrongMessage);
        }



    }

    //[AllowAnonymous]
    //[Route(Urls.logoutApi)]
    //public ActionResult Logout()
    //{
    //    HttpContext.Session.Clear();
        
    //    //await _signInManager.SignOutAsync();
    //    Response.Cookies.Delete("JwtToken");
    //    return RedirectToAction(DefaultValues.IndexView, DefaultValues.LoginController);
    //}



    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Moderator,Admin")]
    [HttpGet]
    [Route(Urls.viewAllUsersApi)]
    public ActionResult ViewAllUsers()
    {
        List<PersonDto> userList = _userInterface.GetAlUsers();
        return Json(userList);

    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin,Moderator")]
    [HttpGet]
    [Route(Urls.userTransactionHistoryApi)]
    public ActionResult UserTransaction(string username)
    {
        List<SalesHistoryResponse> transactioHistory = _userInterface.GetUserTransactionHistory(username);
        if (transactioHistory == null)
        {
            return Json(Message.NoUserTransactionMessage);
        }
        return Json(transactioHistory);
    }


}