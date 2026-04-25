Imports System.IO
Imports System.Runtime.CompilerServices

Module Utils

    Public SaveFile As String = "usersave2.json"
    Public MySave As UserSave = New UserSave()
    Public ShutdownEffect As ShutdownEffect = ShutdownEffect.SolidColor
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
            ShutdownEffect = s.ShutdownEffect
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
            ShutdownEffect = s.ShutdownEffect
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

    Public Function ColorFromHsl(h As Double, s As Double, l As Double) As Color
        Dim r, g, b As Double
        If s = 0 Then
            r = l : g = l : b = l
        Else
            Dim q = If(l < 0.5, l * (1 + s), l + s - l * s)
            Dim p = 2 * l - q
            r = HueToRgb(p, q, h + 1 / 3)
            g = HueToRgb(p, q, h)
            b = HueToRgb(p, q, h - 1 / 3)
        End If
        Return Color.FromRgb(CByte(r * 255), CByte(g * 255), CByte(b * 255))
    End Function

    Private Function HueToRgb(p As Double, q As Double, t As Double) As Double
        If t < 0 Then t += 1
        If t > 1 Then t -= 1
        If t < 1 / 6 Then Return p + (q - p) * 6 * t
        If t < 1 / 2 Then Return q
        If t < 2 / 3 Then Return p + (q - p) * (2 / 3 - t) * 6
        Return p
    End Function

    Public Function ColorFromHsv(h As Double, s As Double, v As Double) As Color
        ' Ensure h is between 0 and 360, s and v between 0 and 1
        h = ((h Mod 360) + 360) Mod 360
        s = Math.Clamp(s, 0, 1)
        v = Math.Clamp(v, 0, 1)

        Dim c As Double = v * s
        Dim x As Double = c * (1 - Math.Abs(((h / 60.0) Mod 2) - 1))
        Dim m As Double = v - c

        Dim r1, g1, b1 As Double

        If h < 60 Then
            r1 = c : g1 = x : b1 = 0
        ElseIf h < 120 Then
            r1 = x : g1 = c : b1 = 0
        ElseIf h < 180 Then
            r1 = 0 : g1 = c : b1 = x
        ElseIf h < 240 Then
            r1 = 0 : g1 = x : b1 = c
        ElseIf h < 300 Then
            r1 = x : g1 = 0 : b1 = c
        Else
            r1 = c : g1 = 0 : b1 = x
        End If

        Return Color.FromRgb(
            CByte((r1 + m) * 255),
            CByte((g1 + m) * 255),
            CByte((b1 + m) * 255)
        )
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

Public Enum ShutdownEffect
    SolidColor
    Aurora
    Breathing
    RainbowLeft
    RainbowRight
    NeonLeft
    NeonRight
    SunsetLeft
    SunsetRight
    AudioParty
    RainbowCycle
    RainbowPinWheel
    Fire
End Enum