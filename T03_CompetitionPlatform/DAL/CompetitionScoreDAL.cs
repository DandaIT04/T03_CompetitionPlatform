using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using T03_CompetitionPlatform.Models;
using System.Collections.Generic;

namespace T03_CompetitionPlatform.DAL
{
    public class CompetitionScoreDAL
    {

        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        //Constructor
        public CompetitionScoreDAL()
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

        public List<CompetitionScore> GetAllScore()
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM CompetitionScore ORDER BY CompetitionID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<CompetitionScore> scoreList = new List<CompetitionScore>();
            while (reader.Read())
            {
                scoreList.Add(
                new CompetitionScore
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitorID = reader.GetInt32(1),
                    CompetitionID = reader.GetInt32(2),
                    Score = reader.GetInt32(3)



                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return scoreList;
        }

        public int Add(CompetitionScore score)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            
            cmd.CommandText = @"INSERT INTO CompetitionScore (CriteriaID, CompetitorID, CompetitionID, Score)
            VALUES(@criteriaid, @competitiorid, @competitionid, @score)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@criteriaid", score.CriteriaID);
            cmd.Parameters.AddWithValue("@competitiorid", score.CompetitorID);
            cmd.Parameters.AddWithValue("@competitionid", score.CompetitionID);
            cmd.Parameters.AddWithValue("@score", score.Score);

            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            cmd.ExecuteScalar();

            

            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return score.CriteriaID;
        }

        public CompetitionScore GetDetails(int criteriaID, int competitorid)
        {
            CompetitionScore score = new CompetitionScore();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a CompetitionScore record.
            cmd.CommandText = @"SELECT * FROM CompetitionScore
                    WHERE CriteriaID = @selectedCriteriaID AND CompetitorID = @selectedCompetitorID";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “criteriaID”.
            cmd.Parameters.AddWithValue("@selectedCriteriaID", criteriaID);
            //parameter is retrieved from the method parameter “competitorid”.
            cmd.Parameters.AddWithValue("@selectedCompetitorID", competitorid);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    // Fill CompetitionScore object with values from the data reader.
                    score.CriteriaID = reader.GetInt32(0);
                    score.CompetitorID = reader.GetInt32(1);
                    score.CompetitionID = reader.GetInt32(2);
                    score.Score = reader.GetInt32(3);

                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return score;
        }

        public int Update(CompetitionScore score)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"UPDATE CompetitionScore SET Score=@score
            WHERE CriteriaID = @selectedCriteriaID AND CompetitorID = @selectedCompetitorID AND CompetitionID = @selectedCompetitionID";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@score", score.Score);
            cmd.Parameters.AddWithValue("@selectedCompetitorID", score.CompetitorID);
            cmd.Parameters.AddWithValue("@selectedCompetitionID", score.CompetitionID);
            cmd.Parameters.AddWithValue("@selectedCriteriaID", score.CriteriaID);


            //Open a database connection
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE
            int count = cmd.ExecuteNonQuery();
            //Close the database connection
            conn.Close();
            return count;
        }

        public bool checkIfJudgeExists(int scoreId, int criId)
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM CompetitionScore  ORDER BY CompetitorID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<CompetitionScore> scoreList = new List<CompetitionScore>();
            while (reader.Read())
            {
                scoreList.Add(
                new CompetitionScore
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitorID = reader.GetInt32(1),
                    CompetitionID = reader.GetInt32(2),
                    Score = reader.GetInt32(3)
                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();

            bool itExists = false;
            foreach (CompetitionScore score in scoreList)
            {
                if ( score.CompetitorID == scoreId && score.CriteriaID == criId)
                {
                    itExists = true;
                }
            }

            return itExists;
        }
    }

}
