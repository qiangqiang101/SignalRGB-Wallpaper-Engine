Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices

Module Utils

    Public SaveFile As String = "usersave.json"
    Public MySave As UserSave = New UserSave()
    Public CompositingQuality As Integer = 0
    Public ShowFps As Integer = 0
    Public BlurIntensity As Integer = 0
    Public FPS As Integer = 60
    Public CpuUsagePauseValue As Integer = 60
    Public BackgroundImage As String = Nothing
    Public Stretch As Stretch = Stretch.Uniform
    Public BackgroundColor As String = "#FFFFFFFF"
    Public SignalRGBPort As Integer = My.Settings.UdpPort '8133/8134/8135

    Public Sub UpdateSRGBConfigValues(s As SignalRGBSettingsChangedEventArgs)
        Try
            CompositingQuality = s.CompositingQuality
            ShowFps = s.ShowFps
            BlurIntensity = s.BlurIntensity
            FPS = s.FPS
            Stretch = s.CoverImageStretch
            BackgroundColor = s.BackgroundColor.ConvertToHex()
            CpuUsagePauseValue = s.CPUUsagePauseValue
            BackgroundImage = s.CoverImage
        Catch ex As Exception
            Logger.Log($"{ex.Message} {ex.StackTrace}")
        End Try
    End Sub

    Public Sub ReadSaveValues(s As UserSave)
        Try
            CompositingQuality = s.CompositingQuality
            ShowFps = s.ShowFps
            BlurIntensity = s.BlurIntensity
            FPS = s.FPS
            Stretch = s.CoverImageStretch
            BackgroundColor = s.BackgroundColor.ConvertToHex()
            CpuUsagePauseValue = s.CpuUsagePauseValue
            BackgroundImage = s.CoverImage
        Catch ex As Exception
            Logger.Log($"{ex.Message} {ex.StackTrace}")
        End Try
    End Sub

    <Extension>
    Public Function ConvertToHex(color As Media.Color, Optional argb As Boolean = False) As String
        If argb Then
            Return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
        Else
            Return $"#{color.R:X2}{color.G:X2}{color.B:X2}"
        End If
    End Function

    <Extension>
    Public Function SetBrushColor(hex As String) As Media.Brush
        Try
            Return New SolidColorBrush(New BrushConverter().ConvertFrom(hex))
        Catch ex As Exception
            Return New SolidColorBrush(Media.Colors.Transparent)
        End Try
    End Function

    <Extension>
    Public Function TryParseCoverImage(ByVal src As String) As BitmapSource
        If String.IsNullOrWhiteSpace(src) Then Return Nothing

        Try
            Dim cleanSrc As String = src.Trim().Replace("""", "")

            Dim bmp As New BitmapImage()
            bmp.BeginInit()

            If cleanSrc.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
                bmp.UriSource = New Uri(cleanSrc, UriKind.Absolute)
            Else
                Dim finalPath As String
                If Path.IsPathRooted(cleanSrc) Then
                    finalPath = cleanSrc
                Else
                    finalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cleanSrc)
                End If

                If Not File.Exists(finalPath) Then
                    Debug.WriteLine($"File not found: {finalPath}")
                    Return Nothing
                End If

                bmp.UriSource = New Uri(finalPath, UriKind.Absolute)
            End If

            bmp.CacheOption = BitmapCacheOption.OnLoad
            bmp.EndInit()
            bmp.Freeze()

            Return bmp
        Catch ex As Exception
            Logger.Log($"{ex.Message} {ex.StackTrace}")
            Return Nothing
        End Try
    End Function

    Public Function IsSignalRGBRunning() As Boolean
        Dim SignalRGB As Process = Process.GetProcessesByName("SignalRgb").FirstOrDefault
        Return Not SignalRGB Is Nothing
    End Function

    <Extension>
    Public Sub SetBlurIntensity(ledcanvas As LedCanvas, intensity As Integer)
        Dim blur = TryCast(ledcanvas.Effect, Media.Effects.BlurEffect)
        If blur IsNot Nothing Then blur.Radius = intensity
    End Sub

    <Extension>
    Public Function FpsToFrametime(fps As Integer) As Integer
        If fps <= 0 Then Return 1
        Return CInt(1000 / fps)
    End Function

End Module

Public Enum LEDShape
    Rectangle
    RoundedRectangle
    Sphere
End Enum

Public Enum MatrixSizeType
    Landscape4_3
    Portrait4_3
    Landscape5_4
    Portrait5_4
    Landscape16_9
    Portrait16_9
    Landscape16_10
    Portrait16_10
    Landscape21_9
    Portrait21_9
    Landscape32_9
    Portrait32_9
    Landscape4_1
    Portrait4_1
End Enum

Public Enum MatrixSizeTier
    Small
    Normal
    Large
    XLarge
End Enum