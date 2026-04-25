Imports System.Runtime.CompilerServices
Imports NAudio.CoreAudioApi
Imports NAudio.Dsp
Imports NAudio.Wave

Module OfflineEffects

    Private _animStep As Double = 0
    Public ReadOnly neonScheme As New ColorScheme With {
    .C1 = Color.FromRgb(255, 0, 255),  ' Magenta
    .C2 = Color.FromRgb(255, 0, 129),  ' Deep Pink
    .C3 = Color.FromRgb(0, 117, 255),  ' Azure Blue
    .C4 = Color.FromRgb(0, 255, 255)   ' Cyan
    }
    Public ReadOnly sunsetScheme As New ColorScheme With {
    .C1 = Color.FromRgb(255, 80, 0),   ' Orange
    .C2 = Color.FromRgb(255, 0, 100),  ' Pink/Red
    .C3 = Color.FromRgb(80, 0, 200),   ' Purple
    .C4 = Color.FromRgb(0, 0, 50)      ' Deep Night Blue
    }

    <Extension>
    Public Sub RenderAurora(snapshot As Color(), w As Integer, h As Integer)
        Dim speed As Double = 0.05
        _animStep += speed

        For count As Integer = 0 To snapshot.Length - 1
            Dim x As Integer = count Mod w
            Dim y As Integer = count \ w

            Dim noiseH As Double = FastNoise.GetNoise(x * 0.1, y * 0.1, _animStep)
            Dim noiseB As Double = FastNoise.GetNoise(x * 0.05, y * 0.2, _animStep * 0.5)

            Dim hue As Double = (220 + (noiseH * 40)) / 360.0
            Dim intensity As Double = Math.Clamp((noiseB + 1.0) / 2.0, 0, 1)

            snapshot(count) = ColorFromHsl(hue, 0.8, intensity * 0.5)
        Next
    End Sub

    <Extension>
    Public Sub RenderSolidColor(snapshot As Color(), w As Integer, h As Integer, targetColor As Color)
        Dim totalLeds = w * h

        If snapshot.Length <> totalLeds Then
            snapshot = New Color(totalLeds - 1) {}
        End If

        For i As Integer = 0 To snapshot.Length - 1
            snapshot(i) = targetColor
        Next
    End Sub

    <Extension>
    Public Sub RenderBreathingColor(snapshot As Color(), w As Integer, h As Integer, targetColor As Color)
        Dim time As Double = DateTime.Now.TimeOfDay.TotalSeconds
        Dim pulse As Double = (Math.Sin(time * 2) + 1.0) / 2.0

        Dim breathingColor = Color.FromRgb(
            CByte(targetColor.R * pulse),
            CByte(targetColor.G * pulse),
            CByte(targetColor.B * pulse)
        )

        RenderSolidColor(snapshot, w, h, breathingColor)
    End Sub

    <Extension>
    Public Sub RenderSunsetNeon(snapshot As Color(), w As Integer, h As Integer, scheme As ColorScheme, Optional direction As String = "left")
        _animStep += 0.01

        For count As Integer = 0 To snapshot.Length - 1
            Dim x As Integer = count Mod w
            Dim y As Integer = count \ w

            Dim progress As Double = 0
            Select Case direction.ToLower()
                Case "left" : progress = (x / w) + _animStep
                Case "right" : progress = (1.0 - (x / w)) + _animStep
                Case "up" : progress = (y / h) + _animStep
                Case "down" : progress = (1.0 - (y / h)) + _animStep
            End Select

            progress = progress Mod 1.0
            If progress < 0 Then progress += 1.0

            snapshot(count) = GetLerpedColor(scheme, progress)
        Next
    End Sub

    Private Function GetLerpedColor(s As ColorScheme, t As Double) As Color
        ' Maps 0.0-1.0 to the 4-color gradient loop
        If t < 0.25 Then
            Return LerpColor(s.C1, s.C2, t / 0.25)
        ElseIf t < 0.5 Then
            Return LerpColor(s.C2, s.C3, (t - 0.25) / 0.25)
        ElseIf t < 0.75 Then
            Return LerpColor(s.C3, s.C4, (t - 0.5) / 0.25)
        Else
            Return LerpColor(s.C4, s.C1, (t - 0.75) / 0.25)
        End If
    End Function

    Private Function LerpColor(c1 As Color, c2 As Color, amount As Double) As Color
        ' Use a helper function to safely interpolate each channel
        Dim r = CalcChannel(c1.R, c2.R, amount)
        Dim g = CalcChannel(c1.G, c2.G, amount)
        Dim b = CalcChannel(c1.B, c2.B, amount)

        Return Color.FromRgb(r, g, b)
    End Function

    Private Function CalcChannel(startVal As Byte, endVal As Byte, amount As Double) As Byte
        Dim startD As Double = CDbl(startVal)
        Dim endD As Double = CDbl(endVal)

        Dim result As Double = startD + (endD - startD) * amount

        If result < 0 Then result = 0
        If result > 255 Then result = 255

        Return CByte(Math.Round(result))
    End Function

End Module

Public Class FastNoise
    ' Simple Sine-based noise to replicate the "Aurora" feel without a 500-line library
    Public Shared Function GetNoise(x As Double, y As Double, z As Double) As Double
        Return (Math.Sin(x * 0.5 + z) + Math.Cos(y * 0.3 - z) + Math.Sin((x + y) * 0.2 + z * 0.5)) / 3.0
    End Function
End Class

Public Structure ColorScheme
    Public C1, C2, C3, C4 As Color
End Structure