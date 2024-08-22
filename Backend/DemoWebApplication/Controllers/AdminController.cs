using DemoWebApplication.Models;
using DemoWebApplication.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoWebApplication.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DemoWebApplication.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public class AdminController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<Person> _userManager;
    public AdminController(RoleManager<IdentityRole> roleManager, UserManager<Person> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpPost]
    [Route(Urls.createRoleApi)]
    public async Task<ApiResponse> CreateRole([FromBody] RoleDto role)
    {
        if (string.IsNullOrEmpty(role.rolename))
        {
            //return BadRequest(Message.RoleNameBadRequestMessage);
            return new ApiResponse { success = false, message = Message.RoleNameBadRequestMessage };

        }
        bool roleExists = await _roleManager.RoleExistsAsync(role.rolename);
        if (roleExists)
        {
            return new ApiResponse { success = false, message = Message.RoleAlreadyExists };
            //return Json(new { success = false, message = Message.RoleAlreadyExists });
        }
        else
        {
            IdentityRole indentityRole = new IdentityRole { Name = role.rolename };
            IdentityResult result = await _roleManager.CreateAsync(indentityRole);

            if (result.Succeeded)
            {
                return new ApiResponse { success = true, message = Message.SuccessCreatingingRoleMessage };

                //return Json(new { success = true, message = Message.SuccessCreatingingRoleMessage });

            }
            return new ApiResponse { success = false, message = Message.ErrorCreatingingRoleMessage };

            //return Json(new { success = false, message = Message.ErrorCreatingingRoleMessage });

        }

    }

    [HttpGet]
    [Route(Urls.listAllRolesApi)]
    public async Task<IActionResult> ListRoles()
    {
        List<IdentityRole> roles = await _roleManager.Roles.ToListAsync();
        return Json(roles);
    }


    [HttpPost]
    [Route(Urls.editRoleApi)]
    public async Task<ApiResponse> EditRole([FromBody] EditRoleDto model)
    {
        var role = await _roleManager.FindByIdAsync(model.Id);
        if (role == null)
        {
            return new ApiResponse { success = false, message = Message.RoleNotFoundMessage };

            //return NotFound(Message.RoleNotFoundMessage);
        }
        else
        {
            role.Name = model.Name;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return new ApiResponse { success = true, message = Message.SuccessEditingRoleMessage };

                //return Json(new { success = true, message = Message.SuccessEditingRoleMessage });
            }
            return new ApiResponse { success = false, message = Message.ErrorEditingRoleMessage };

            //return Json(new { success = false, message = Message.ErrorEditingRoleMessage });

        }

    }

    [HttpPost]
    [Route(Urls.deleteRoleApi)]
    public async Task<ApiResponse> DeleteRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return new ApiResponse { success = false, message = Message.RoleNotFoundMessage };

            //return NotFound(Message.RoleNotFoundMessage);
        }
        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            return new ApiResponse { success = true, message = Message.SuccessDeletingRoleMessage };

            //return Json(new { success = true, message = Message.SuccessDeletingRoleMessage});
        }
        return new ApiResponse { success = false, message = Message.ErrorDeletingRoleMessage };

        //return Json(new { success = false, message = Message.ErrorDeletingRoleMessage });
    }



    [HttpPost]
    [Route(Urls.assginRoleToUser)]
    public async Task<ApiResponse> AssignRoleToUser([FromBody] UserRoleAssignmentModel model)
    {
        var errors = new List<string>();

        var role = await _roleManager.FindByIdAsync(model.RoleId);
        if (role == null)
        {
            return new ApiResponse { success = false, message = Message.RoleNotFoundMessage };

            //return NotFound(Message.RoleNotFoundMessage);
        }

        var users = new List<Person>();
        foreach (var userId in model.UserIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                users.Add(user);
            }
            else
            {
                errors.Add($"User not found with id {userId}");
            }
        }

        foreach (var user in users)
        {

            var isInRole = await _userManager.IsInRoleAsync(user, role.Name);
            if (model.IsSelected && !isInRole)
            {
                var result = await _userManager.AddToRoleAsync(user, role.Name);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors.Select(e => $"Failed to assign role '{role.Name}' to user '{user.UserName}': {e.Description}"));
                }
                foreach(var checkRole in await _userManager.GetRolesAsync(user))
                {
                    if(checkRole != role.Name)
                    {
                        await _userManager.RemoveFromRoleAsync(user, checkRole);
                    }
                }
            }
            //else if (!model.IsSelected && isInRole)

            else if (model.IsSelected && isInRole)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors.Select(e => $"Failed to remove role '{role.Name}' from user '{user.UserName}': {e.Description}"));
                }
            }
        }

        if (errors.Any())
        {
            return new ApiResponse { success = false, message = Message.AssignRoleBadRequestMessage };

            //return BadRequest(errors);
        }

        return new ApiResponse { success = true, message =  Message.RoleAssignedSuccessfullyMessage};

        //return Ok();
    }

}
