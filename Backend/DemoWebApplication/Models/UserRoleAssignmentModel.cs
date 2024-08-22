namespace DemoWebApplication.Models
{
    public class UserRoleAssignmentModel
    {
        public List<string> UserIds { get; set; }
        public string RoleId { get; set; }
        public bool IsSelected { get; set; }
    }
}
