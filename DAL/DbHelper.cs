using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;


//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Data;

public class DbHelper
{
    public string ConnectionString { get; }

    public DbHelper(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("dbsc");
    }

    public DataTable GetDataTable(SqlCommand cmd)
    {
        using SqlConnection con = new SqlConnection(ConnectionString);
        cmd.Connection = con;

        using SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        return dt;
    }
    public List<Department> GetDepartments()
    {
        List<Department> list = new List<Department>();

        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            string query = "SELECT DepartmentId, DepartmentName FROM Departments";
            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new Department
                {
                    DepartmentId = Convert.ToInt32(rdr["DepartmentId"]),
                    DepartmentName = rdr["DepartmentName"].ToString()
                });
            }
        }

        return list;
    }
    public List<Employee> GetEmployees()
    {
        List<Employee> list = new List<Employee>();

        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            string query = "SELECT * FROM Employees";
            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new Employee
                {
                    EmployeeId = Convert.ToInt32(rdr["EmployeeId"]),
                    FirstName = rdr["FirstName"].ToString(),
                    LastName = rdr["LastName"].ToString(),
                    Email = rdr["Email"].ToString(),
                    Phone = rdr["Phone"] as string,
                    AadhaarNumber = rdr["AadhaarNumber"].ToString(),
                    PanNumber = rdr["PanNumber"].ToString(),
                    Gender = rdr["Gender"] as string,
                    PhotoPath = rdr["PhotoPath"] as string,
                    DepartmentId = Convert.ToInt32(rdr["DepartmentId"])
                });
            }
        }

        return list;
    }

    public List<UserListVM> GetUsers()
    {
        List<UserListVM> list = new List<UserListVM>();

        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            string query = @"
        SELECT 
    u.UserId,e.FirstName,u.Username,r.RoleName,u.IsActive
 
FROM Users u
LEFT JOIN Employees e ON u.EmployeeId = e.EmployeeId
LEFT JOIN Roles r ON u.RoleId = r.RoleId";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new UserListVM
                {
                    UserId = Convert.ToInt32(rdr["UserId"]),
                    Username = rdr["Username"].ToString(),
                    RoleName = rdr["RoleName"].ToString(),
                    IsActive = Convert.ToBoolean(rdr["IsActive"]),
                    FirstName = rdr["FirstName"].ToString(),
                   
                });
            }
        }

        return list;
    }
    public List<SelectListItem> GetRoles()
    {
        List<SelectListItem> list = new List<SelectListItem>();

        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            string q = "SELECT RoleId, RoleName FROM Roles";

            SqlCommand cmd = new SqlCommand(q, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = rdr["RoleId"].ToString(),
                    Text = rdr["RoleName"].ToString()
                });
            }
        }

        return list;
    }

    public List<SelectListItem> Employees()
    {
        List<SelectListItem> list = new List<SelectListItem>();

        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            string q = "SELECT EmployeeId, FirstName + ' ' + LastName AS Name FROM Employees where EmployeeId not in (select EmployeeId from users)";

            SqlCommand cmd = new SqlCommand(q, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = rdr["EmployeeId"].ToString(),
                    Text = rdr["Name"].ToString()
                });
            }
        }

        return list;
    }


    public List<Employee> GetEmployeesWithDepartment()
    {
        List<Employee> list = new List<Employee>();

        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            string query = @"SELECT e.EmployeeId, e.FirstName, e.LastName, e.Email,e.Phone,
                                e.DepartmentId, e.PhotoPath, d.DepartmentName,e.DateOfBirth
                         FROM Employees e
                         INNER JOIN Departments d 
                         ON e.DepartmentId = d.DepartmentId";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new Employee
                {
                    EmployeeId = Convert.ToInt32(rdr["EmployeeId"]),
                    Phone = rdr["Phone"].ToString(),
                    FirstName = rdr["FirstName"].ToString(),
                    LastName = rdr["LastName"].ToString(),
                    Email = rdr["Email"].ToString(),
                    DateOfBirth = rdr["DateOfBirth"] == DBNull.Value
    ? null
    : DateOnly.FromDateTime(Convert.ToDateTime(rdr["DateOfBirth"])),

                    DepartmentId = Convert.ToInt32(rdr["DepartmentId"]),
                    PhotoPath = rdr["PhotoPath"] as string,

                    // 👇 this is the important part
                    Department = new Department
                    {
                        DepartmentName = rdr["DepartmentName"].ToString()
                    }
                });
            }
        }

        return list;
    }
    public Employee GetEmployeeById(int id)
    {
        Employee emp = new Employee();

        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            string query = "SELECT * FROM Employees WHERE EmployeeId = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                emp.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                emp.FirstName = dr["FirstName"].ToString();
                emp.LastName = dr["LastName"].ToString();
                emp.Email = dr["Email"].ToString();
                emp.Phone = dr["Phone"].ToString();
                emp.AadhaarNumber = dr["AadhaarNumber"].ToString();
                if (dr["DateOfBirth"] != DBNull.Value)
                {
                    emp.DateOfBirth = DateOnly.FromDateTime(
                        Convert.ToDateTime(dr["DateOfBirth"])
                    );
                }

                emp.PanNumber = dr["PanNumber"].ToString();
                emp.Gender = dr["Gender"].ToString();
                emp.PhotoPath = dr["PhotoPath"].ToString();
                emp.DepartmentId = Convert.ToInt32(dr["DepartmentId"]);
            }
        }

        return emp;
    }
    public void AddUser(NewUserVM user)
    {
        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            string query = @"INSERT INTO Users(EmployeeId,Username,passwordhash,RoleId,IsActive)
                         VALUES(@EmployeeId,@Username,'test',@RoleId,@IsActive)";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@EmployeeId", user.EmployeeId);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);

            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public NewUserVM GetUserById(int id)
    {
        NewUserVM user = new NewUserVM();

        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            string q = "SELECT * FROM Users WHERE UserId=@id";
            SqlCommand cmd = new SqlCommand(q, con);
            cmd.Parameters.AddWithValue("@id", id);

            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                user.EmployeeId = Convert.ToInt32(rdr["EmployeeId"]);
                user.Username = rdr["Username"].ToString();
                user.RoleId = Convert.ToInt32(rdr["RoleId"]);
                user.IsActive = Convert.ToBoolean(rdr["IsActive"]);
            }
        }
        return user;
    }
    public void UpdateUser(NewUserVM user, int id)
    {
        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            string q = @"UPDATE Users 
                     SET 
                         Username=@Username,
                         RoleId=@RoleId,
                         IsActive=@IsActive
                     WHERE UserId=@id";

            SqlCommand cmd = new SqlCommand(q, con);

            //cmd.Parameters.AddWithValue("@EmployeeId", user.EmployeeId);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            cmd.Parameters.AddWithValue("@id", id);

            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
    public void DeleteUser(int id)
    {
        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserId=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

}
