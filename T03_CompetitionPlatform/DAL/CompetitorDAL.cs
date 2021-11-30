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
    public class CompetitorDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        //Constructor
        public CompetitorDAL()
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

        public List<Competitor> GetAllCompetitors()
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Competitor ORDER BY CompetitorID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Competitor> competitorList = new List<Competitor>();
            while (reader.Read())
            {
                competitorList.Add(
                new Competitor
                {
                    CompetitorID = reader.GetInt32(0),  //0: 1st column and + 1 per column 
                    CompetitorName = reader.GetString(1),
                    Salutation = reader.GetString(2),
                    EmailAddr = reader.GetString(3),
                    Password = reader.GetString(4),
                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return competitorList;
        }

        public bool checkIfCompetitorExists(int theId)
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

        public bool IsEmailExist(string email)
        {
            bool emailFound = false;

            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT EmailAddr FROM Competitor 
                                WHERE EmailAddr=@selectedEmail";
            cmd.Parameters.AddWithValue("@selectedEmail", email);

            //Open a database connection
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            { //Records found
                while (reader.Read())
                {
                    if (reader.GetString(0) == email)
                        emailFound = true;
                }
            }
            else
            {
                emailFound = false;
            }
            reader.Close();
            conn.Close();

            return emailFound;
        }

        public int CompetitorRegister(Competitor compt)
        {
            //TEST CODE
            //compt.CompetitorID = 0;
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO Competitor (CompetitorName, Salutation, EmailAddr, Password) 
            OUTPUT INSERTED.CompetitorID 
            VALUES(@competitorName, @salutation, @emailAddr, @password)";

            cmd.Parameters.AddWithValue("@competitorName", compt.CompetitorName);
            cmd.Parameters.AddWithValue("@salutation", compt.Salutation);
            cmd.Parameters.AddWithValue("@emailAddr", compt.EmailAddr);
            cmd.Parameters.AddWithValue("@password", compt.Password);

            // Open connection to database
            conn.Open();

            // Generate competitor ID
            compt.CompetitorID = (int)cmd.ExecuteScalar();

            // Close connection to database
            conn.Close();

            return compt.CompetitorID;
        }

        public Competitor GetDetails(int competitorID)
        {
            Competitor compt = new Competitor();

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM Competitor
                    WHERE CompetitorID = @selectedCompetitorID";

            cmd.Parameters.AddWithValue("@selectedCompetitorID", competitorID);

            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    // Fill Competitor object with values from the data reader


                    compt.CompetitorID = reader.GetInt32(0);
                    compt.CompetitorName = reader.GetString(1);
                    compt.Salutation = reader.GetString(2);
                    compt.EmailAddr = reader.GetString(3);
                    compt.Password = reader.GetString(4);
                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return compt;
        }

    }
}
