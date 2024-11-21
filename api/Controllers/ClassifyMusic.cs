using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers;

public class Music
{
    public string Path { get; set; }
    public int    NumberOfPlays { get; set; }

    public Music(string path)
    {
        this.Path = path;
        NumberOfPlays = 1;
    }
}


public class SqlHelper
{
    
    public string query { get; set; }
    public SqlHelper(string query)
    {
        this.query = query;
    }

    public int getSongCount(Music song)
    {
        // Validate input
        if (song == null || string.IsNullOrWhiteSpace(song.Path))
            throw new ArgumentException("Invalid song object or path.");

        // SQL query with a parameterized placeholder
        string query = "SELECT CountOfPlays FROM musicPlayed WHERE SongName = @SongName;";

        using (SqlConnection connection = new SqlConnection(Constants.connectionString))
        {
            try
            {
                connection.Open();

                // Use parameterized query to prevent SQL injection and elite hax0rs
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SongName", song.Path);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Retrieve the value from the reader and return it
                            if (reader["CountOfPlays"] != DBNull.Value)
                            {
                                return Convert.ToInt32(reader["CountOfPlays"]);
                            }
                        }
                    }
                }

                // If no records are found, return 0 (or a suitable fallback)
                return 0;
            }
            catch (Exception e)
            {
                // Log the exception (can use a logging framework here)
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
        }
    }
    
    public bool addSong(Music song)
    {
        bool bRet = false;
        this.query = $"INSERT INTO musicPlayed (SongName, CountOfPlays) VALUES ('{song.Path.Replace("'", "''")}', {song.NumberOfPlays});";
        using (SqlConnection connection = new SqlConnection(Constants.connectionString))
        {
            try
            {
                // open
                connection.Open();
                string tempQuery = $"Select * from musicPlayed where SongName = \'{song.Path}\';";
                Console.WriteLine(tempQuery);
                SqlCommand tq = new SqlCommand(tempQuery, connection);
                SqlDataReader readTemp = tq.ExecuteReader();
                if (readTemp.Read())
                {
                    song.NumberOfPlays += 1;
                    this.query =
                        $"UPDATE musicPlayed SET CountOfPlays = {song.NumberOfPlays} where SongName = \'{song.Path}\';";
                }
                readTemp.Close();
                SqlCommand command = new SqlCommand(this.query, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader == null)
                {
                    bRet = false;
                }
                else
                {
                    bRet = true;
                }
                while (reader.Read())
                {
                    Console.WriteLine($"SongName:  {reader["SongName"]}");
                    Console.WriteLine($"Play Count:  {reader["CountOfPlays"]}");
                    
                }
                reader.Close();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        return bRet;
    }
    
}

[ApiController]
[Route("[controller]")]
public class ClassifyMusic : ControllerBase
{
    [HttpPost(Name = "AddRecord")]
    public async Task<HttpResponseMessage> Post(string path)
    {
        if (path.Length == 0)
        {
            return new HttpResponseMessage(HttpStatusCode.Gone)
            {
                Content = new StringContent("No Path to file!")
            };
        }
        // declare music
        Music song = new Music(path);
        SqlHelper sqlHelper = new SqlHelper("");
        bool sqlHelperBool = sqlHelper.addSong(song);
        song.NumberOfPlays = sqlHelper.getSongCount(song);
        if (!sqlHelperBool)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Failed to add song to sql database")
            };
        }
        return new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = new StringContent("Song added!")
        };
    }
}