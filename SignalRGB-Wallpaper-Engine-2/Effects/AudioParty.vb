Imports System.Runtime.CompilerServices

Module AudioParty
    Private _animStep As Double = 0
    Private _xShift As Double = 0
    Private _colorShift As Double = 0
    Private _effectIdx As Integer = 0
    Private _lastTriggerTime As DateTime = DateTime.MinValue

    <Extension>
    Public Sub RenderAudioParty(snapshot As Color(), w As Integer, h As Integer)
        Dim divisions As Integer = 4
        Dim gain As Double = 65
        Dim speed As Double = 50
        Dim colorSpeed As Double = 25
        Dim threshold As Double = 20
        Dim motionStop As Integer = 50
        Dim colorStop As Integer = 150

        Dim filteredData = GetOpenRGBSpectrum(AudioEngine.AudioBins, gain)

        Dim bassLevel As Single = 0
        For i As Integer = 0 To 49 : bassLevel = Math.Max(bassLevel, filteredData(i)) : Next
        _xShift += bassLevel * (speed / 1000.0)

        If _animStep >= 1.0 AndAlso (DateTime.Now - _lastTriggerTime).TotalMilliseconds > 1500 Then
            Dim trebleLevel As Single = 0
            For i As Integer = 150 To 199 : trebleLevel = Math.Max(trebleLevel, filteredData(i)) : Next

            If (trebleLevel * (gain / 10.0)) > (threshold / 100.0) Then
                _effectIdx = New Random().Next(0, 7)
                _animStep = 0
                _lastTriggerTime = DateTime.Now
            End If
        End If

        Dim midLevel As Single = 0
        For i As Integer = 50 To 149 : midLevel = Math.Max(midLevel, filteredData(i)) : Next
        _colorShift += midLevel * (colorSpeed / 2000.0)

        If _animStep < 1.0 Then _animStep += 0.01 * (speed / 15.0)

        For y As Integer = 0 To h - 1
            Dim posY As Double = y / CDbl(h)
            For x As Integer = 0 To w - 1
                Dim posX As Double = x / CDbl(w)

                Dim s As Double = 0.5 * (1 + Math.Sin(posX * divisions * Math.PI + _xShift))
                Dim hue As Double = (180 + Math.Sin(posY + _colorShift) * 180) Mod 360
                Dim baseCol = ColorFromHsv(hue, 1.0, 1.0)
                Dim overlay = GetEffectOverlay(x, y, w, h)
                Dim idx = (y * w) + x
                snapshot(idx) = Color.FromRgb(
                    ScreenBlend(CByte(baseCol.R * s), CByte(overlay.R)),
                    ScreenBlend(CByte(baseCol.G * s), CByte(overlay.G)),
                    ScreenBlend(CByte(baseCol.B * s), CByte(overlay.B))
                )
            Next
        Next
    End Sub

    Private Function GetOpenRGBSpectrum(rawFreq As Single(), gain As Double) As Single()
        Dim spectrum = New Single(199) {}
        Dim maxVal As Single = 1.0F
        Dim sumVal As Single = 1.0F
        Dim linearLevel As Double = Math.Pow(10, (3 + gain - 50) * 0.1)

        For i As Integer = 0 To 199
            sumVal += rawFreq(i)
            If rawFreq(i) > maxVal Then maxVal = rawFreq(i)
        Next

        Dim fac As Single = 2.0F / (maxVal + sumVal)
        For i As Integer = 0 To 199
            spectrum(i) = CSng(rawFreq(i) * fac * linearLevel)
        Next
        Return spectrum
    End Function

    Private Function ScreenBlend(a As Byte, b As Byte) As Byte
        Return CByte(Math.Min(255, CSng(a) + b - (CSng(a) * b / 255.0F)))
    End Function

    Private Function GetEffectOverlay(x As Integer, y As Integer, w As Integer, h As Integer) As Color
        If _animStep >= 1.0 Then Return Colors.Black
        Select Case _effectIdx
            Case 0 ' Moving bar Left to Right
                Return If(Math.Abs(x - _animStep * w) <= 1, Colors.White, Colors.Black)
            Case 1 ' Moving bar Right to Left
                Return If(Math.Abs(x - (w - _animStep * w)) <= 1, Colors.White, Colors.Black)
            Case 2 ' Moving bar Top to Bottom
                Return If(Math.Abs(y - _animStep * h) <= 1, Colors.White, Colors.Black)
            Case 3 ' Moving bar Bottom to Top
                Return If(Math.Abs(y - (h - _animStep * h)) <= 1, Colors.White, Colors.Black)
            Case 4 ' Random RGB
                Return Color.FromRgb(CByte(Rnd() * 255), CByte(Rnd() * 255), CByte(Rnd() * 255))
            Case 5 ' Random White
                Dim br = CByte(Rnd() * 255)
                Return Color.FromRgb(br, br, br)
            Case 6 ' Blink/Flash
                Dim val = CByte(255 * Math.Max(0, 1.0 - _animStep))
                Return Color.FromRgb(val, val, val)
            Case Else
                Return Colors.Black
        End Select
    End Function
End Module