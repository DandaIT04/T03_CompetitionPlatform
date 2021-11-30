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
    public class CompetitionDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        //Constructor
        public CompetitionDAL()
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

        public List<Competition> GetAllCompetitions()
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Competition ORDER BY CompetitionID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Competition> competitionList = new List<Competition>();
            while (reader.Read())
            {
                competitionList.Add(
                new Competition
                {
                    CompetitionID = reader.GetInt32(0), //0: 1st column and + 1 per column 
                    AreaInterestID = reader.GetInt32(1),
                    CompetitionName = reader.GetString(2),
                    StartDate = !reader.IsDBNull(3) ?
                    reader.GetDateTime(3) : (DateTime?)null,
                    EndDate = !reader.IsDBNull(4) ?
                    reader.GetDateTime(4) : (DateTime?)null,
                    ResultReleasedDate = !reader.IsDBNull(5) ?
                    reader.GetDateTime(5) : (DateTime?)null,

                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();

            return competitionList;
        }

        public int Add(Competition competitions)
        {
            if (competitions.StartDate == null && competitions.EndDate == null && competitions.ResultReleasedDate == null)
            {
                //Create a SqlCommand object from connection object
                SqlCommand cmd1 = conn.CreateCommand();
                //Specify an INSERT SQL statement which will
                //return the auto-generated StaffID after insertion
                cmd1.CommandText = @"INSERT INTO Competition (AreaInterestID,CompetitionName)
                                OUTPUT INSERTED.CompetitionID
                                VALUES(@AreaInterestID,@competitionName)";
                //Define the parameters used in SQL statement, value for each parameter
                //is retrieved from respective class's property.
                cmd1.Parameters.AddWithValue("@AreaInterestID", competitions.AreaInterestID);
                cmd1.Parameters.AddWithValue("@competitionName", competitions.CompetitionName);
                //A connection to database must be opened before any operations made.
                conn.Open();
                //ExecuteScalar is used to retrieve the auto-generated
                //StaffID after executing the INSERT SQL statement
                competitions.CompetitionID = (int)cmd1.ExecuteScalar();
                //A connection should be closed after operations.
                conn.Close();
                //Return id when no error occurs.
                return competitions.CompetitionID;
            }
            else if (competitions.StartDate == null && competitions.EndDate == null && competitions.ResultReleasedDate != null)
            {
                //Create a SqlCommand object from connection object
                SqlCommand cmd2 = conn.CreateCommand();
                //Specify an INSERT SQL statement which will
                //return the auto-generated StaffID after insertion
                cmd2.CommandText = @"INSERT INTO Competition (AreaInterestID,CompetitionName,ResultReleasedDate)
                                OUTPUT INSERTED.CompetitionID
                                VALUES(@AreaInterestID,@competitionName,@resultReleasedDate)";
                //Define the parameters used in SQL statement, value for each parameter
                //is retrieved from respective class's property.
                cmd2.Parameters.AddWithValue("@AreaInterestID", competitions.AreaInterestID);
                cmd2.Parameters.AddWithValue("@competitionName", competitions.CompetitionName);
                cmd2.Parameters.AddWithValue("@resultReleasedDate", competitions.ResultReleasedDate);
                //A connection to database must be opened before any operations made.
                conn.Open();
                //ExecuteScalar is used to retrieve the auto-generated
                //StaffID after executing the INSERT SQL statement
                competitions.CompetitionID = (int)cmd2.ExecuteScalar();
                //A connection should be closed after operations.
                conn.Close();
                //Return id when no error occurs.
                return competitions.CompetitionID;
            }

            else if (competitions.StartDate == null && competitions.EndDate != null && competitions.ResultReleasedDate == null)
            {
                //Create a SqlCommand object from connection object
                SqlCommand cmd3 = conn.CreateCommand();
                //Specify an INSERT SQL statement which will
                //return the auto-generated StaffID after insertion
                cmd3.CommandText = @"INSERT INTO Competition (AreaInterestID,CompetitionName,EndDate)
                                OUTPUT INSERTED.CompetitionID
                                VALUES(@AreaInterestID,@competitionName,@endDate)";
                //Define the parameters used in SQL statement, value for each parameter
                //is retrieved from respective class's property.
                cmd3.Parameters.AddWithValue("@AreaInterestID", competitions.AreaInterestID);
                cmd3.Parameters.AddWithValue("@competitionName", competitions.CompetitionName);
                cmd3.Parameters.AddWithValue("@endDate", competitions.EndDate);
                //A connection to database must be opened before any operations made.
                conn.Open();
                //ExecuteScalar is used to retrieve the auto-generated
                //StaffID after executing the INSERT SQL statement
                competitions.CompetitionID = (int)cmd3.ExecuteScalar();
                //A connection should be closed after operations.
                conn.Close();
                //Return id when no error occurs.
                return competitions.CompetitionID;
            }

            else if (competitions.StartDate != null && competitions.EndDate == null && competitions.ResultReleasedDate == null)
            {
                //Create a SqlCommand object from connection object
                SqlCommand cmd4 = conn.CreateCommand();
                //Specify an INSERT SQL statement which will
                //return the auto-generated StaffID after insertion
                cmd4.CommandText = @"INSERT INTO Competition (AreaInterestID,CompetitionName,StartDate)
                                OUTPUT INSERTED.CompetitionID
                                VALUES(@AreaInterestID,@competitionName,@startDate)";
                //Define the parameters used in SQL statement, value for each parameter
                //is retrieved from respective class's property.
                cmd4.Parameters.AddWithValue("@AreaInterestID", competitions.AreaInterestID);
                cmd4.Parameters.AddWithValue("@competitionName", competitions.CompetitionName);
                cmd4.Parameters.AddWithValue("@startDate", competitions.StartDate);
                //A connection to database must be opened before any operations made.
                conn.Open();
                //ExecuteScalar is used to retrieve the auto-generated
                //StaffID after executing the INSERT SQL statement
                competitions.CompetitionID = (int)cmd4.ExecuteScalar();
                //A connection should be closed after operations.
                conn.Close();
                //Return id when no error occurs.
                return competitions.CompetitionID;
            }

            else if (competitions.StartDate == null && competitions.EndDate != null && competitions.ResultReleasedDate != null)
            {
                //Create a SqlCommand object from connection object
                SqlCommand cmd5 = conn.CreateCommand();
                //Specify an INSERT SQL statement which will
                //return the auto-generated StaffID after insertion
                cmd5.CommandText = @"INSERT INTO Competition (AreaInterestID,CompetitionName,EndDate,ResultReleasedDate)
                                OUTPUT INSERTED.CompetitionID
                                VALUES(@AreaInterestID,@competitionName,@endDate,@resultReleasedDate)";
                //Define the parameters used in SQL statement, value for each parameter
                //is retrieved from respective class's property.
                cmd5.Parameters.AddWithValue("@AreaInterestID", competitions.AreaInterestID);
                cmd5.Parameters.AddWithValue("@competitionName", competitions.CompetitionName);
                cmd5.Parameters.AddWithValue("@endDate", competitions.EndDate);
                cmd5.Parameters.AddWithValue("@resultReleasedDate", competitions.ResultReleasedDate);
                //A connection to database must be opened before any operations made.
                conn.Open();
                //ExecuteScalar is used to retrieve the auto-generated
                //StaffID after executing the INSERT SQL statement
                competitions.CompetitionID = (int)cmd5.ExecuteScalar();
                //A connection should be closed after operations.
                conn.Close();
                //Return id when no error occurs.
                return competitions.CompetitionID;
            }

            else if (competitions.StartDate != null && competitions.EndDate != null && competitions.ResultReleasedDate == null)
            {
                //Create a SqlCommand object from connection object
                SqlCommand cmd6 = conn.CreateCommand();
                //Specify an INSERT SQL statement which will
                //return the auto-generated StaffID after insertion
                cmd6.CommandText = @"INSERT INTO Competition (AreaInterestID,CompetitionName,StartDate,EndDate)
                                OUTPUT INSERTED.CompetitionID
                                VALUES(@AreaInterestID,@competitionName,@startDate,@endDate)";
                //Define the parameters used in SQL statement, value for each parameter
                //is retrieved from respective class's property.
                cmd6.Parameters.AddWithValue("@AreaInterestID", competitions.AreaInterestID);
                cmd6.Parameters.AddWithValue("@competitionName", competitions.CompetitionName);
                cmd6.Parameters.AddWithValue("@startDate", competitions.StartDate);
                cmd6.Parameters.AddWithValue("@endDate", competitions.EndDate);
                //A connection to database must be opened before any operations made.
                conn.Open();
                //ExecuteScalar is used to retrieve the auto-generated
                //StaffID after executing the INSERT SQL statement
                competitions.CompetitionID = (int)cmd6.ExecuteScalar();
                //A connection should be closed after operations.
                conn.Close();
                //Return id when no error occurs.
                return competitions.CompetitionID;
            }

            else
            {
                //Create a SqlCommand object from connection object
                SqlCommand cmd = conn.CreateCommand();
                //Specify an INSERT SQL statement which will
                //return the auto-generated StaffID after insertion
                cmd.CommandText = @"INSERT INTO Competition (AreaInterestID,CompetitionName,StartDate,EndDate,ResultReleasedDate)
                                OUTPUT INSERTED.CompetitionID
                                VALUES(@areaInterestID,@competitionName,@startDate,@endDate,@resultReleasedDate)";
                //Define the parameters used in SQL statement, value for each parameter
                //is retrieved from respective class's property.
                cmd.Parameters.AddWithValue("@areaInterestID", competitions.AreaInterestID);
                cmd.Parameters.AddWithValue("@competitionName", competitions.CompetitionName);
                cmd.Parameters.AddWithValue("@startDate", competitions.StartDate);
                cmd.Parameters.AddWithValue("@endDate", competitions.EndDate);
                cmd.Parameters.AddWithValue("@resultReleasedDate", competitions.ResultReleasedDate);
                //A connection to database must be opened before any operations made.
                conn.Open();
                //ExecuteScalar is used to retrieve the auto-generated
                //StaffID after executing the INSERT SQL statement
                competitions.CompetitionID = (int)cmd.ExecuteScalar();
                //A connection should be closed after operations.
                conn.Close();
                //Return id when no error occurs.
                return competitions.CompetitionID;
            }
        }

        // For DeleteArea if ID is not null!
        public bool checkIfCompetitionExists(int theId)
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Competition ORDER BY CompetitionID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Competition> competitionList = new List<Competition>();
            while (reader.Read())
            {
                competitionList.Add(
                new Competition
                {
                    CompetitionID = reader.GetInt32(0), //0: 1st column and + 1 per column 
                    AreaInterestID = reader.GetInt32(1),
                    CompetitionName = reader.GetString(2),
                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();

            bool itExists = false;
            foreach (Competition competitions in competitionList)
            {
                if (competitions.AreaInterestID == theId)
                {
                    itExists = true;
                }
            }

            return itExists;
        }

        // For CreateCompetition itself!!
        public bool IsCompetitionNameExist(string competitionName, int competitionID)
        {
            bool nameFound = false;
            //Create a SqlCommand object and specify the SQL statement
            //to get a areaInterest record with the name to be validated
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT CompetitionID FROM Competition
                                WHERE CompetitionName=@selectedName";
            cmd.Parameters.AddWithValue("@selectedName", competitionName);
            //Open a database connection and execute the SQL statement
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            { //Records found
                while (reader.Read())
                {
                    if (reader.GetInt32(0) != competitionID)
                        //The name is used
                        nameFound = true;
                }
            }
            else
            { //No record
                nameFound = false; // The name given does not exist
            }
            reader.Close();
            conn.Close();

            return nameFound;
        }

        public Competition GetDetails(int competitionID)
        {
            Competition competitions = new Competition();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a staff record.
            cmd.CommandText = @"SELECT * FROM Competition
                    WHERE CompetitionID = @selectedCompetitionID";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “areainterestId”.
            cmd.Parameters.AddWithValue("@selectedCompetitionID", competitionID);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    // Fill staff object with values from the data reader.
                    competitions.CompetitionID = competitionID;
                    competitions.AreaInterestID = reader.GetInt32(1);
                    competitions.CompetitionName = !reader.IsDBNull(2) ? reader.GetString(2) : null;
                    competitions.StartDate = !reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null;
                    competitions.EndDate = !reader.IsDBNull(4) ? reader.GetDateTime(4) : (DateTime?)null;
                    competitions.ResultReleasedDate = !reader.IsDBNull(5) ? reader.GetDateTime(5) : (DateTime?)null;
                    // (char) 0 - ASCII Code 0 - null value
                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return competitions;
        }

        public int DeleteCompetition(int CompetitionID)
        {
            //Instantiate a SqlCommand object, supply it with a DELETE SQL statement
            //to delete a staff record specified by a Staff ID
            SqlCommand cmd = conn.CreateCommand();

            //Step 1 update record in staff where StaffID = supervisorID in Staff
            cmd.CommandText = @"DELETE FROM Competition
            WHERE CompetitionID = @selectcompetitionID";

            cmd.Parameters.AddWithValue("@selectcompetitionID", CompetitionID);

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

        //Return number of row updated
        public int Update(Competition competition)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();

            //Specify an UPDATE SQL statement
            cmd.CommandText = @"UPDATE Competition SET CompetitionName=@competitionName,
            StartDate=@startDate, EndDate=@endDate, ResultReleasedDate =@resultReleasedDate
            WHERE CompetitionID = @selectedCompetitionID";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@selectedCompetitionID", competition.CompetitionID);
            cmd.Parameters.AddWithValue("@competitionName", competition.CompetitionName);

            if (competition.StartDate != null && competition.EndDate != null && competition.ResultReleasedDate != null)
            {
                cmd.Parameters.AddWithValue("@startDate", competition.StartDate);
                cmd.Parameters.AddWithValue("@endDate", competition.EndDate);
                cmd.Parameters.AddWithValue("@resultReleasedDate", competition.ResultReleasedDate);
            }

            else if (competition.StartDate != null && competition.EndDate != null && competition.ResultReleasedDate == null)
            {
                cmd.Parameters.AddWithValue("@startDate", competition.StartDate);
                cmd.Parameters.AddWithValue("@endDate", competition.EndDate);
                cmd.Parameters.AddWithValue("@resultReleasedDate", DBNull.Value);
            }

            else if (competition.StartDate != null && competition.EndDate == null && competition.ResultReleasedDate != null)
            {
                cmd.Parameters.AddWithValue("@startDate", competition.StartDate);
                cmd.Parameters.AddWithValue("@endDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@resultReleasedDate", competition.ResultReleasedDate);
            }

            else if (competition.StartDate == null && competition.EndDate != null && competition.ResultReleasedDate != null)
            {
                cmd.Parameters.AddWithValue("@startDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@endDate", competition.EndDate);
                cmd.Parameters.AddWithValue("@resultReleasedDate", competition.ResultReleasedDate);
            }

            else if (competition.StartDate == null && competition.EndDate == null && competition.ResultReleasedDate != null)
            {
                cmd.Parameters.AddWithValue("@startDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@endDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@resultReleasedDate", competition.ResultReleasedDate);
            }

            else if (competition.StartDate != null && competition.EndDate == null && competition.ResultReleasedDate == null)
            {
                cmd.Parameters.AddWithValue("@startDate", competition.StartDate);
                cmd.Parameters.AddWithValue("@endDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@resultReleasedDate", DBNull.Value);
            }

            else if (competition.StartDate == null && competition.EndDate != null && competition.ResultReleasedDate == null)
            {
                cmd.Parameters.AddWithValue("@startDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@endDate", competition.EndDate);
                cmd.Parameters.AddWithValue("@resultReleasedDate", DBNull.Value);
            }


            else // No branch is assigned
            {
                cmd.Parameters.AddWithValue("@startDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@endDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@resultReleasedDate", DBNull.Value);
            }
            //Open a database connection
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE
            int count = cmd.ExecuteNonQuery();
            //Close the database connection
            conn.Close();
            return count;

        }

        public List<Criteria> GetCompCriteria(int compId)
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a Criteria record.
            cmd.CommandText = @"SELECT * FROM Criteria
                              WHERE CompetitionID = @selectedCompID";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “compId”.
            cmd.Parameters.AddWithValue("@selectedCompID", compId);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();

            List<Criteria> criteriaList = new List<Criteria>();
            //Read the record from database
            while (reader.Read())
            {
                criteriaList.Add(
                    new Criteria
                    {

                        CriteriaID = reader.GetInt32(0),
                        CompetitionID = reader.GetInt32(1),
                        CriteriaName = reader.GetString(2),
                        Weightage = reader.GetInt32(3),



                    });

            }
            //Close data reader
            reader.Close();

            //Close database connection
            conn.Close();
            return criteriaList;
        }

        public List<CompetitionSubmission> GetCompSub(int compid)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SQL statement that select all branches
            cmd.CommandText = @"SELECT * FROM CompetitionSubmission WHERE CompetitionID = @selectedComp";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “branchNo”.
            cmd.Parameters.AddWithValue("@selectedComp", compid);

            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            List<CompetitionSubmission> compsubList = new List<CompetitionSubmission>();
            while (reader.Read())
            {
                compsubList.Add(
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
            //Close database connection
            conn.Close();
            return compsubList;
        }
    }

}

