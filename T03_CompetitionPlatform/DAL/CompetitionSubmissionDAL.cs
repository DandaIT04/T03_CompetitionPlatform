using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using T03_CompetitionPlatform.Models;
using Microsoft.AspNetCore.Http;

namespace T03_CompetitionPlatform.DAL
{
    // <!-- Edited this 27/6/2021 8pm Delete Judges Function-->
    public class CompetitionSubmissionDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        //Constructor
        public CompetitionSubmissionDAL()
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

        public List<CompetitionSubmission> GetAllSubmissions()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM CompetitionSubmission ORDER BY CompetitionID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<CompetitionSubmission> competitionSubmissionList = new List<CompetitionSubmission>();
            while (reader.Read())
            {
                competitionSubmissionList.Add(
                new CompetitionSubmission
                {
                    CompetitionID = reader.GetInt32(0),
                    CompetitorID = reader.GetInt32(1),
                    FileSubmitted = !reader.IsDBNull(2) ?
                    reader.GetString(2) : null,
                    DateTimeFileUpload = !reader.IsDBNull(3) ?
                    reader.GetDateTime(3) : (DateTime?)null,
                    Appeal = !reader.IsDBNull(4) ?
                    reader.GetString(4) : null,
                    VoteCount = reader.GetInt32(5),
                    Ranking = !reader.IsDBNull(6) ?
                    reader.GetInt32(6) : (int?)null
                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();

            return competitionSubmissionList;
        }

        public List<CompetitionSubmission> GetDetails(int competitiorID)
        {
            List<CompetitionSubmission> comptSubList = new List<CompetitionSubmission>();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a competitionSubmission record.
            cmd.CommandText = @"SELECT * FROM CompetitionSubmission
                    WHERE CompetitorID = @selectedCompetitiorID";

            cmd.Parameters.AddWithValue("@selectedCompetitiorID", competitiorID);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    comptSubList.Add(
                         new CompetitionSubmission
                         {
                             CompetitionID = reader.GetInt32(0),
                             CompetitorID = reader.GetInt32(1),
                             FileSubmitted = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                             DateTimeFileUpload = !reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null,
                             Appeal = !reader.IsDBNull(4) ? reader.GetString(4) : null,
                             VoteCount = reader.GetInt32(5),
                             Ranking = !reader.IsDBNull(6) ? reader.GetInt32(6) : (int?)null,
                         }
                     );
                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return comptSubList;
        }

        public CompetitionSubmission GetSingleDetails(int competitionID, int competitorID)
        {
            CompetitionSubmission comptSub = new CompetitionSubmission();

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a competitionSubmission record.
            cmd.CommandText = @"SELECT * FROM CompetitionSubmission
                    WHERE CompetitorID = @selectedCompetitiorID";

            cmd.Parameters.AddWithValue("@selectedCompetitiorID", competitorID);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    comptSub.CompetitionID = reader.GetInt32(0);
                    comptSub.CompetitorID = reader.GetInt32(1);
                    comptSub.FileSubmitted = !reader.IsDBNull(2) ? reader.GetString(2) : null;
                    comptSub.DateTimeFileUpload = !reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null;
                    comptSub.Appeal = !reader.IsDBNull(4) ? reader.GetString(4) : null;
                    comptSub.VoteCount = reader.GetInt32(5);
                    comptSub.Ranking = !reader.IsDBNull(6) ? reader.GetInt32(6) : (int?)null;
                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return comptSub;
        }

        public int AddVote(int id, int compID)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE CompetitionSubmission SET VoteCount = (VoteCount + 1) where (CompetitionID=@selcompID and CompetitorID=@selID)";
            cmd.Parameters.AddWithValue("@selcompID", compID);
            cmd.Parameters.AddWithValue("@selID", id);
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE
            int count = cmd.ExecuteNonQuery();
            //Close the database connection
            conn.Close();
            return count;
        }

        public int UpdateRank(CompetitionSubmission compsub)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"UPDATE  CompetitionSubmission SET Ranking = @rank
            WHERE CompetitionID=@selcompID AND CompetitorID=@selID";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@selcompID", compsub.CompetitionID);
            cmd.Parameters.AddWithValue("@selID", compsub.CompetitorID); ;
            cmd.Parameters.AddWithValue("@rank", compsub.Ranking ?? (object)DBNull.Value);



            //Open a database connection
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE
            int count = cmd.ExecuteNonQuery();
            //Close the database connection
            conn.Close();
            return count;
        }

        public bool checkIfCompetitorSubmissionExists(int theId)
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM CompetitionSubmission ORDER BY CompetitionID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<CompetitionSubmission> competitionList = new List<CompetitionSubmission>();
            while (reader.Read())
            {
                competitionList.Add(
                new CompetitionSubmission
                {
                    CompetitionID = reader.GetInt32(0), //0: 1st column and + 1 per column 
                    CompetitorID = reader.GetInt32(1),
                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();

            bool itExists = false;

            foreach (CompetitionSubmission competitions in competitionList)
            {
                if (competitions.CompetitionID == theId)
                {
                    if (competitions.CompetitorID != null)
                    {
                        itExists = true;
                    }
                }
            }

            return itExists;
        }

        public void JoinCompetition(CompetitionSubmission comptSub)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO CompetitionSubmission (CompetitionID, CompetitorID, VoteCount) 
            VALUES(@competitionID, @competitorID, @voteCount)";

            cmd.Parameters.AddWithValue("@competitionID", comptSub.CompetitionID);
            cmd.Parameters.AddWithValue("@competitorID", comptSub.CompetitorID);
            cmd.Parameters.AddWithValue("@voteCount", comptSub.VoteCount);

            // Open connection to database
            conn.Open();

            cmd.ExecuteNonQuery();

            // Close connection to database
            conn.Close();
        }
    }
}
