namespace VicemAPI.Models.Process
{
    public enum SystemPermissions
    {
        GetAllUser,
        GetAllEmployee, GetEmployeeById, CreateEmployee, EditEmployee, DeleteEmployee,
        GetAllRole, GetRoleById, CreateRole, EditRole, DeleteRole,
        AssignPermissionToRole, GetPermissionsForRole,
        GetRoleForUser, AssignRoleToUser
    }
}