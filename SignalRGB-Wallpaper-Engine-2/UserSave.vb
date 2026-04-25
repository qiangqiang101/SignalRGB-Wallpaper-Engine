
Imports Newtonsoft.Json

Public Class UserSave

    Public MatrixSizeType As MatrixSizeType
    Public MatrixSizeTier As MatrixSizeTier
    Public ShutdownEffect As ShutdownEffect
    Public ShowFps As Boolean
    Public BlurIntensity As Integer
    Public LedShape As LEDShape
    Public RoundedRectangleCornerRadius As Integer
    Public LedPadding As Single
    Public FPS As Integer
    Public CoverImageStretch As Stretch
    Public BackgroundColor As Color
    Public ShutdownColor As Color
    Public CpuUsagePauseValue As Integer
    Public CoverImage As String

    Public Function Load(filename As String) As UserSave
        Return JsonConvert.DeserializeObject(Of UserSave)(IO.File.ReadAllText(filename))
    End Function

    Public Sub Save(filename As String)
        IO.File.WriteAllText(filename, JsonConvert.SerializeObject(Me, Formatting.Indented))
    End Sub

End Class
