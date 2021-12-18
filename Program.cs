using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;
using MySqlConnector;

namespace abortionCounter
{
    internal class Program
    {
        static String date;
        static String previousDate;
        static String timeDiff;
        static String abortionsForDB;
        static double abortionCount;
        static double abortionCountDB;

        static void Main(string[] args)
        {
            getAbortionCountFromDB();
            getElapsedTimeSinceLastExecution();
            calculateTotalAbortionsSinceLastExecution();
            legalAbortionsCounter(abortionCount, abortionCountDB);
        }
        static void legalAbortionsCounter(double abortionCountSinceLastRunTime, double totalAbortionCount)
        {

            double babiesAbortedSoFar = abortionCountSinceLastRunTime + totalAbortionCount;
            while (babiesAbortedSoFar < 1000000000)
            {
                double abortionRate = babiesAbortedSoFar + .00085;
                babiesAbortedSoFar = abortionRate;
                Console.Write(abortionRate + "\n");
                getDate();
                setDateIntoDataBase();
                abortionsForDB = Convert.ToString(babiesAbortedSoFar);
                setAbortionCountIntoDataBase();
                Thread.Sleep(8);
            }
        }
        static void getDate()
        {
            DateTime localDate = DateTime.Now;
            date = localDate.ToString("yyyy-MM-dd HH:mm:ss");
        }

        static void setDateIntoDataBase()
        {
            using (var conn = new MySqlConnection("Server=localhost;User ID=root;Password=root;Database=abortionDate"))
            {
                conn.Open();
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE datetime SET date = '" + date + "'";
                    cmd.ExecuteNonQuery();
                }
            }

        }

        static void setAbortionCountIntoDataBase()
        {
            using (var conn = new MySqlConnection("Server=localhost;User ID=root;Password=root;Database=abortionDate"))
            {
                conn.Open();
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE datetime SET abortions = '" + abortionsForDB + "'";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        static void getDateFromDataBase()
        {
            using (var conn = new MySqlConnection("Server=localhost;User ID=root;Password=root;Database=abortionDate"))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT date FROM datetime", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        previousDate = (reader.GetString(0));

            }
        }

        static void getAbortionCountFromDB()
        {
            using (var conn = new MySqlConnection("Server=localhost;User ID=root;Password=root;Database=abortionDate"))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT abortions FROM datetime", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        abortionCountDB = (reader.GetDouble(0));

            }
        }

        static void getElapsedTimeSinceLastExecution()
        {
            getDate();
            getDateFromDataBase();
            timeDiff = (Convert.ToDateTime(date) - Convert.ToDateTime(previousDate)).TotalDays.ToString();
        }

        static void calculateTotalAbortionsSinceLastExecution()
        {
            getElapsedTimeSinceLastExecution();
            double totalSecondsSinceLastExecution = Convert.ToDouble(timeDiff) * 24 * 60 * 60;
            abortionCount = totalSecondsSinceLastExecution * .05;

        }

    }
}