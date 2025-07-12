Imports Newtonsoft.Json
Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Runtime

Public Class UserSave

    Public MatrixSizeType As MatrixSizeType
    Public SmoothingMode As SmoothingMode
    Public CompositingQuality As CompositingQuality
    Public InterpolationMode As InterpolationMode
    Public PixelOffsetMode As PixelOffsetMode
    Public LedShape As LEDShape
    Public RoundedRectangleCornerRadius As Integer
    Public LedPadding As Single
    Public LedUpdateInterval As Integer
    Public CoverImageSizeMode As Integer
    Public BackgroundColor As Color
    Public CpuUsagePauseValue As Integer
    Public CoverImage As String

    Public Function Load(filename As String) As UserSave
        Return JsonConvert.DeserializeObject(Of UserSave)(IO.File.ReadAllText(filename))
    End Function

    Public Sub Save(filename As String)
        IO.File.WriteAllText(filename, JsonConvert.SerializeObject(Me, Formatting.Indented))
    End Sub

End Class
