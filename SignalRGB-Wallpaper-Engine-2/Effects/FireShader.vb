Imports System.Numerics
Imports System.Runtime.CompilerServices

Module FireShader

    <Extension>
    Public Sub RenderFireShader(snapshot As Color(), w As Integer, h As Integer)
        Dim color4 As Color = Color.FromRgb(64, 5, 0) ' Base
        Dim color1 As Color = Color.FromRgb(255, 77, 0) ' Body
        Dim color2 As Color = Color.FromRgb(255, 154, 0) ' Heat
        Dim color3 As Color = Color.FromRgb(255, 213, 128) ' Core
        Dim speed1 As Double = 120
        Dim speed2 As Double = 10
        Dim distance As Double = 353
        Dim amplitude As Double = 47
        Dim itime As Double = DateTime.Now.TimeOfDay.TotalSeconds * 2

        Dim speed = New Vector2(speed1 / 100.0, speed2 / 100.0)
        Dim distBase = (distance / 100.0) - Math.Sin(itime * 0.4) / 1.89
        Dim shift = 1.327 + Math.Sin(itime * 2.0) / 2.4
        Dim amp = amplitude

        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                Dim p = New Vector2(x * distBase / w, y * distBase / w)
                p.X -= itime / 1.1

                Dim q = FBM(p - New Vector2(itime * 0.01 + Math.Sin(itime) / 10.0, 0), amp)
                Dim qb = FBM(p - New Vector2(itime * 0.002 + Math.Cos(itime) / 5.0, 0), amp)
                Dim q2 = FBM(p - New Vector2(itime * 0.44 + 5.0 * Math.Cos(itime) / 7.0, 0), amp) - 6.0
                Dim q3 = FBM(p - New Vector2(itime * 0.9 + 10.0 * Math.Cos(itime) / 30.0, 0), amp) - 4.0
                Dim q4 = FBM(p - New Vector2(itime * 2.0 + 20.0 * Math.Sin(itime) / 20.0, 0), amp) + 2.0

                Dim finalQ = (q + qb - 0.4 * q2 - 2.0 * q3 + 0.6 * q4) / 3.8

                Dim r = New Vector2(
                FBM(p + New Vector2(finalQ / 2.0 + itime * speed.X - p.X - p.Y, 0), amp),
                FBM(p + New Vector2(finalQ - itime * speed.Y, 0), amp)
            )
                Dim fbmPR = FBM(p + r, amp)
                Dim resR = Lerp(color1.R, color2.R, fbmPR) + Lerp(color3.R, color4.R, r.X) - Lerp(25, 230, r.Y)
                Dim resG = Lerp(color1.G, color2.G, fbmPR) + Lerp(color3.G, color4.G, r.X) - Lerp(25, 230, r.Y)
                Dim resB = Lerp(color1.B, color2.B, fbmPR) + Lerp(color3.B, color4.B, r.X) - Lerp(25, 230, r.Y)

                Dim intensity = Math.Cos(shift * y / h)

                snapshot(y * w + x) = Color.FromRgb(
                CByte(Math.Clamp(resR * intensity, 0, 255)),
                CByte(Math.Clamp(resG * intensity, 0, 255)),
                CByte(Math.Clamp(resB * intensity, 0, 255)))
            Next
        Next
    End Sub

    Private Function Rand(n As Vector2) As Double
        Dim dotProduct = Vector2.Dot(n, New Vector2(12.9898, 12.1414))
        Return (Math.Sin(Math.Cos(dotProduct)) * 83758.5453) Mod 1.0
    End Function

    Private Function Noise(n As Vector2) As Double
        Dim b = New Vector2(Math.Floor(n.X), Math.Floor(n.Y))
        Dim f = n - b

        f = New Vector2(f.X * f.X * (3 - 2 * f.X), f.Y * f.Y * (3 - 2 * f.Y))

        Dim d = New Vector2(0.0, 1.0)
        Return Lerp(Lerp(Rand(b), Rand(b + New Vector2(d.Y, d.X)), f.X),
                    Lerp(Rand(b + New Vector2(d.X, d.Y)), Rand(b + New Vector2(d.Y, d.Y)), f.X), f.Y)
    End Function

    Private Function FBM(n As Vector2, amplitudeSetting As Double) As Double
        Dim total As Double = 0.0
        Dim amp As Double = 1.0
        Dim p = n

        For i As Integer = 0 To 4
            total += Noise(p) * amp
            p *= 1.7
            amp *= (amplitudeSetting / 100.0)
        Next
        Return total
    End Function

    Private Function Lerp(a As Double, b As Double, t As Double) As Double
        Return a + (b - a) * t
    End Function

End Module
