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
    public class CriteriaDAL
    {

        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        //Constructor
        public CriteriaDAL()
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

        public List<Criteria> GetAllCriteria()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Criteria ORDER BY CriteriaID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();

            //Read all records until the end, save data into a criteria list
            List<Criteria> criteriaList = new List<Criteria>();
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

            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return criteriaList;
        }


        public int Add(Criteria criteria)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            
            cmd.CommandText = @"INSERT INTO Criteria (CompetitionID, CriteriaName, Weightage)
            OUTPUT INSERTED.CriteriaID
            VALUES(@CompetitionID, @CriteriaName, @Weightage)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@CompetitionID", criteria.CompetitionID);
            cmd.Parameters.AddWithValue("@CriteriaName", criteria.CriteriaName);
            cmd.Parameters.AddWithValue("@Weightage", criteria.Weightage);

            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //CriteriaID after executing the INSERT SQL statement
            criteria.CriteriaID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return criteria.CriteriaID;
        }

        public Criteria GetDetails(int criteriaID)
        {
            Criteria criteria = new Criteria();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a Criteriea record.
            cmd.CommandText = @"SELECT * FROM Criteria
                    WHERE CriteriaID = @selectedCriteriaID";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “criteriaID”.
            cmd.Parameters.AddWithValue("@selectedCriteriaID", criteriaID);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    // Fill criteria object with values from the data reader.
                    criteria.CriteriaID = criteriaID;
                    criteria.CompetitionID = reader.GetInt32(1);
                    criteria.CriteriaName = reader.GetString(2);
                    criteria.Weightage = reader.GetInt32(3);

                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return criteria;
        }

        public int Update(Criteria criteria)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"UPDATE Criteria SET CriteriaName=@criterianame,
            Weightage=@weightage
            WHERE CriteriaID = @selectedCriteriaID";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@criterianame", criteria.CriteriaName);
            cmd.Parameters.AddWithValue("@weightage", criteria.Weightage);
            cmd.Parameters.AddWithValue("@selectedCriteriaID", criteria.CriteriaID);


            //Open a database connection
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE
            int count = cmd.ExecuteNonQuery();
            //Close the database connection
            conn.Close();
            return count;
        }

        public int Delete(int criteriaId)
        {
            //Instantiate a SqlCommand object, supply it with a DELETE SQL statement
            //to delete a staff record specified by a Criteria ID
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM Criteria
            WHERE CriteriaID = @selectCriteriaID";
            cmd.Parameters.AddWithValue("@selectCriteriaID", criteriaId);
            //Open a database connection
            conn.Open();
            int rowAffected = 0;
            //Execute the DELETE SQL to remove the Criteria record
            rowAffected += cmd.ExecuteNonQuery();
            //Close database connection
            conn.Close();
            //Return number of row of criteria record updated or deleted
            return rowAffected;








        }

        public bool checkIfCriteriaExists(int criId)
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Criteria ORDER BY CriteriaID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Criteria> criList = new List<Criteria>();
            while (reader.Read())
            {
                criList.Add(
                new Criteria
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitionID = reader.GetInt32(1),
                    CriteriaName = reader.GetString(2),
                    Weightage = reader.GetInt32(3),
                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();

            bool itExists = false;
            foreach (Criteria criteria in criList)
            {
                if (criteria.CriteriaID == criId)
                {
                    itExists = true;
                }
            }

            return itExists;
        }
    }
}
