Imports System.IO

Public NotInheritable Class Logger

    Public Shared Sub Log(message As String)
        File.AppendAllText(Path.Combine($".\", $"SignalRGB-Wallpaper-Engine-{Now.ToString("dd-MM-yyyy")}.log"), $"{Now.ToShortTimeString}: {message}{Environment.NewLine}")
    End Sub

    Public Shared Sub Capture(message As String)
        File.AppendAllText(Path.Combine($".\", $"Captures.log"), $"{Now.ToShortTimeString}: {message}{Environment.NewLine}")
    End Sub

End Class