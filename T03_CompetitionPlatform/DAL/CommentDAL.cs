using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using T03_CompetitionPlatform.Models;

namespace T03_CompetitionPlatform.DAL
{
    public class CommentDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        //Constructor
        public CommentDAL(){
            //Read ConnectionString from appsettings.json file
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("NPBookConnectionString");
            //Instantiate a SqlConnection object with the
            //Connection String read.
            conn = new SqlConnection(strConn);
        }

        public List<Comment> GetAllComments()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Comment ORDER BY CommentID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Comment> commentList = new List<Comment>();

            while (reader.Read())
            {
                commentList.Add(
                new Comment
                {
                    CommentID = reader.GetInt32(0), //0: 1st column
                    CompetitionID = reader.GetInt32(1),
                    Description = !reader.IsDBNull(2) ?
                    reader.GetString(2) : null,
                    DateTimePosted = reader.GetDateTime(3)
                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return commentList;
        }

        public int Add(Comment comment)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO Comment (CompetitionID, Description, DateTimePosted)
            OUTPUT INSERTED.CommentID
            VALUES(@CompetitionID, @Description, @DateTimePosted)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@CompetitionID", comment.CompetitionID);
            cmd.Parameters.AddWithValue("@Description", comment.Description);
            cmd.Parameters.AddWithValue("@DateTimePosted", comment.DateTimePosted);

            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //CriteriaID after executing the INSERT SQL statement
            comment.CommentID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return comment.CommentID;
        }

    }
}
