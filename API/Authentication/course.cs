using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Data;

namespace RegisterAndLogin.Authentication
{
    public class Course
    {

        public int CourseId { get; set; }
        public string Course_Category { get; set; }

        public string Course_Name { get; set; }

        public DateTime Course_Start_Date { get; set; }

        public string Course_Description { get; set; }

        public string Format { get; set; }

        public string Level { get; set; }

        public float Price { get; set; }

        public Course()
        {

        }

        public Course(int course_id, string course_category, string course_name, DateTime Course_start_date, string course_description, string format, string level, float price)
        {
            CourseId = course_id;
            Course_Category = course_category;
            Course_Name = course_name;
            Course_Description = course_description;
            Format = format;
            Level = level;
            Price = price;
        }


        public static SqlConnection con;
        public static SqlCommand cmd;

        public static void getcon()
        {
            con = new SqlConnection("Data Source= MONISHA-LAPTOP\\SQLSERVER2019;Initial Catalog=learningportal;Integrated Security =true");
            con.Open();
        }


        public static List<Course> GetAllCourses()
        {
            List<Course> courses = new List<Course>();
            Course.getcon();
            cmd = new SqlCommand("selectcourses");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Course c = new Course();

                c.CourseId = Convert.ToInt32(dr[0]);
                c.Course_Category = dr[1].ToString();
                c.Course_Name = dr[2].ToString();
                c.Course_Start_Date = Convert.ToDateTime(dr[3]);
                c.Course_Description = dr[4].ToString();
                c.Format = dr[5].ToString();
                c.Level = dr[6].ToString();
                c.Price = float.Parse(dr[7].ToString());

                courses.Add(c);
            }

            return courses;
        }


        public static void InsertCourses(Course C)
        {
            Course.getcon();
            cmd = new SqlCommand("insertcourse");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@course_category", C.Course_Category);
            cmd.Parameters.AddWithValue("@course_name", C.Course_Name);
            cmd.Parameters.AddWithValue("@course_start_date", C.Course_Start_Date);
            cmd.Parameters.AddWithValue("@course_Description", C.Course_Description);
            cmd.Parameters.AddWithValue("@course_format", C.Format);
            cmd.Parameters.AddWithValue("@course_level", C.Level);
            cmd.Parameters.AddWithValue("@course_price", C.Price);
            cmd.ExecuteNonQuery();

        }
    }

}
