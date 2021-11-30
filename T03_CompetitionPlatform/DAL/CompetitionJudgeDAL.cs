using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using T03_CompetitionPlatform.Models;

//< !--Edited this 27 / 6 / 2021 8pm-- Delete Judges Function> DAN

namespace T03_CompetitionPlatform.DAL
{
    public class CompetitionJudgeDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        //Constructor
        public CompetitionJudgeDAL()
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

        public List<CompetitionJudge> GetAllCompetitionJudge()
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM CompetitionJudge ORDER BY CompetitionID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<CompetitionJudge> competitionJudgeList = new List<CompetitionJudge>();
            while (reader.Read())
            {
                competitionJudgeList.Add(
                new CompetitionJudge
                {
                    CompetitionID = reader.GetInt32(0),  //0: 1st column and + 1 per column 
                    JudgeID = reader.GetInt32(1),  //0: 1st column and + 1 per column 
                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return competitionJudgeList;
        }

        public int AddJudges(CompetitionJudge compJudge)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO CompetitionJudge (CompetitionID,JudgeID)
                                OUTPUT INSERTED.CompetitionID
                                VALUES(@competitionID,@judgeID)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@competitionID", compJudge.CompetitionID);
            cmd.Parameters.AddWithValue("@judgeID", compJudge.JudgeID);
            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            compJudge.CompetitionID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return compJudge.CompetitionID;
        }

        public int RemoveJudges(CompetitionJudge toDelete)
        {
            //Instantiate a SqlCommand object, supply it with a DELETE SQL statement
            //to delete a staff record specified by a Staff ID
            SqlCommand cmd = conn.CreateCommand();

            //Step 1 update record in staff where StaffID = supervisorID in Staff
            cmd.CommandText = @"DELETE FROM CompetitionJudge
            WHERE CompetitionID = @selectcompetitionID
            AND JudgeID = @selectjudgeID";

            cmd.Parameters.AddWithValue("@selectcompetitionID", toDelete.CompetitionID);
            cmd.Parameters.AddWithValue("@selectjudgeID", toDelete.JudgeID);

            //Open a database connection
            conn.Open();
            int rowAffected = 0;
            //Execute the DELETE SQL to remove the staff record
            rowAffected += cmd.ExecuteNonQuery();

            //Close database connection
            conn.Close();
            //Return number of row of staff record updated or deleted
            return rowAffected;
        }

    }
}
