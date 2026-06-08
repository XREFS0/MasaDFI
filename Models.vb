Imports System.Collections.Generic

Namespace Models
    Public Class CaseInfo
        Public Property ID As Integer
        Public Property Name As String
        Public Property Examiner As String
        Public Property DateCreated As String
        Public Property Description As String
    End Class

    Public Class Evidence
        Public Property ID As Integer
        Public Property CaseID As Integer
        Public Property Type As String
        Public Property Path As String
        Public Property Hash As String
        Public Property AcquiredDate As String
    End Class

    Public Class BrowserHistoryEntry
        Public Property ID As Integer
        Public Property Browser As String
        Public Property URL As String
        Public Property Title As String
        Public Property VisitTime As String
        Public Property VisitCount As Integer
    End Class

    Public Class USBDeviceEntry
        Public Property ID As Integer
        Public Property DeviceName As String
        Public Property SerialNumber As String
        Public Property FirstUse As String
        Public Property LastUse As String
    End Class

    Public Class TimelineEvent
        Public Property ID As Integer
        Public Property EventTime As String
        Public Property Category As String
        Public Property Description As String
    End Class

    Public Class RecoveredFileEntry
        Public Property ID As Integer
        Public Property FileName As String
        Public Property Size As Long
        Public Property DeletedDate As String
        Public Property OriginalPath As String
    End Class

    Public Class SuspiciousFileEntry
        Public Property ID As Integer
        Public Property FilePath As String
        Public Property HashMD5 As String
        Public Property HashSHA256 As String
        Public Property Entropy As Double
        Public Property Status As String
    End Class
End Namespace
