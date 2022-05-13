using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RegisterAndLogin.Authentication
{
    public class RegisterModel
    {

        [Required(ErrorMessage = "USer Name is required")]

        public string UserName { get; set; }
        [Key]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is requires")]
        public string Password { get; set; }

        public static List<RegisterModel> GetUsers()
        {
            List<RegisterModel> us = new List<RegisterModel>();
            SqlConnection con = new SqlConnection("Data Source= MONISHA-LAPTOP\\SQLSERVER2019;Initial Catalog=learningportal;Integrated Security =true");
            SqlCommand cmd = new SqlCommand("Select UserName,Email from AspNetUsers");
            con.Open();
            cmd.Connection = con;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                RegisterModel u = new RegisterModel();
                u.UserName = dr[0].ToString();
                u.Email = dr[1].ToString();
                us.Add(u);
            }
            return us;
        }
    }
}
