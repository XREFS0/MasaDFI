Imports System.Data.SQLite
Imports System.IO

Public Class DatabaseManager
    Private ReadOnly _dbPath As String
    Private ReadOnly _connectionString As String

    Public Sub New(Optional dbFileName As String = "masadfi.db")
        _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbFileName)
        _connectionString = $"Data Source={_dbPath};Version=3;"
    End Sub

    Public Sub InitializeDatabase()
        If Not File.Exists(_dbPath) Then
            SQLiteConnection.CreateFile(_dbPath)
        End If

        Using connection As New SQLiteConnection(_connectionString)
            connection.Open()
            Dim commands As String() = {
                "CREATE TABLE IF NOT EXISTS Cases (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Examiner TEXT, DateCreated TEXT, Description TEXT)",
                "CREATE TABLE IF NOT EXISTS Evidence (ID INTEGER PRIMARY KEY AUTOINCREMENT, CaseID INTEGER, Type TEXT, Path TEXT, Hash TEXT, AcquiredDate TEXT)",
                "CREATE TABLE IF NOT EXISTS BrowserHistory (ID INTEGER PRIMARY KEY AUTOINCREMENT, Browser TEXT, URL TEXT, Title TEXT, VisitTime TEXT, VisitCount INTEGER)",
                "CREATE TABLE IF NOT EXISTS USBDevices (ID INTEGER PRIMARY KEY AUTOINCREMENT, DeviceName TEXT, SerialNumber TEXT, FirstUse TEXT, LastUse TEXT)",
                "CREATE TABLE IF NOT EXISTS Timeline (ID INTEGER PRIMARY KEY AUTOINCREMENT, EventTime TEXT, Category TEXT, Description TEXT)",
                "CREATE TABLE IF NOT EXISTS RecoveredFiles (ID INTEGER PRIMARY KEY AUTOINCREMENT, FileName TEXT, Size INTEGER, DeletedDate TEXT, OriginalPath TEXT)",
                "CREATE TABLE IF NOT EXISTS SuspiciousFiles (ID INTEGER PRIMARY KEY AUTOINCREMENT, FilePath TEXT, HashMD5 TEXT, HashSHA256 TEXT, Entropy REAL, Status TEXT)",
                "CREATE TABLE IF NOT EXISTS Reports (ID INTEGER PRIMARY KEY AUTOINCREMENT, CaseID INTEGER, GeneratedDate TEXT, ReportPath TEXT)"
            }

            Using command As New SQLiteCommand(connection)
                For Each cmdText In commands
                    command.CommandText = cmdText
                    command.ExecuteNonQuery()
                Next
            End Using
        End Using
    End Sub

    Public Function GetConnection() As SQLiteConnection
        Dim connection As New SQLiteConnection(_connectionString)
        connection.Open()
        Return connection
    End Function

End Class
