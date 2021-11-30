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
    public class JudgesDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        //Constructor
        public JudgesDAL()
        {
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

        public List<Judge> GetAllJudges()
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Judge ORDER BY JudgeID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Judge> judgesList = new List<Judge>();
            while (reader.Read())
            {
                judgesList.Add(
                new Judge
                {
                    JudgeID = reader.GetInt32(0),  //0: 1st column and + 1 per column 
                    JudgeName = reader.GetString(1),
                    Salutation = reader.GetString(2),
                    AreaInterestID = reader.GetInt32(3),
                    EmailAddr = reader.GetString(4),
                    Password = reader.GetString(5),
                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return judgesList;
        }

        public Judge Getlogin(string email)
        {
            Judge judge = new Judge();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a staff record.
            cmd.CommandText = @"SELECT * FROM Judge
                    WHERE EmailAddr = @selectedJudgeEmail";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “areainterestId”.
            cmd.Parameters.AddWithValue("@selectedJudgeEmail", email);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    // Fill staff object with values from the data reader
                    judge.JudgeID = reader.GetInt32(0);
                    judge.JudgeName = reader.GetString(1);
                    judge.Salutation = reader.GetString(2);
                    judge.AreaInterestID = reader.GetInt32(3);
                    judge.EmailAddr = email;
                    judge.Password = reader.GetString(5);
                    // (char) 0 - ASCII Code 0 - null value
                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return judge;
        }

        public bool checkIfJudgeExists(int judgeId)
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Judge ORDER BY JudgeID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Judge> judgeList = new List<Judge>();
            while (reader.Read())
            {
                judgeList.Add(
                new Judge
                {
                    JudgeID = reader.GetInt32(0),  //0: 1st column and + 1 per column 
                    JudgeName = reader.GetString(1),
                    Salutation = reader.GetString(2),
                    AreaInterestID = reader.GetInt32(3),
                    EmailAddr = reader.GetString(4),
                    Password = reader.GetString(5),
                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();

            bool itExists = false;
            foreach (Judge judges in judgeList)
            {
                if (judges.AreaInterestID == judgeId)
                {
                    itExists = true;
                }
            }

            return itExists;
        }

        public Judge GetDetails(int JudgeID)
        {
            Judge judge = new Judge();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a staff record.
            cmd.CommandText = @"SELECT * FROM Judge
                    WHERE JudgeID = @selectedJudgeID";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “areainterestId”.
            cmd.Parameters.AddWithValue("@selectedJudgeID", JudgeID);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    // Fill staff object with values from the data reader
                    judge.JudgeID = reader.GetInt32(0);
                    judge.JudgeName = reader.GetString(1);
                    judge.Salutation = reader.GetString(2);
                    judge.AreaInterestID = reader.GetInt32(3);
                    judge.EmailAddr = reader.GetString(4);
                    judge.Password = reader.GetString(5);
                    // (char) 0 - ASCII Code 0 - null value
                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return judge;
        }
        public int Add(Judge judge)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated JudgeID after insertion
            cmd.CommandText = @"INSERT INTO Judge (JudgeName, Salutation, AreaInterestID, EmailAddr, Password)
            OUTPUT INSERTED.JudgeID 
            VALUES(@judgename, @salutation, @aoi, @emailaddress, @password)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@judgename", judge.JudgeName);
            cmd.Parameters.AddWithValue("@salutation", judge.Salutation);
            cmd.Parameters.AddWithValue("@aoi", judge.AreaInterestID);
            cmd.Parameters.AddWithValue("@emailaddress", judge.EmailAddr);
            cmd.Parameters.AddWithValue("@password", judge.Password);

            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //CriteriaID after executing the INSERT SQL statement
           judge.JudgeID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return judge.JudgeID;
        }

        
    }
}
