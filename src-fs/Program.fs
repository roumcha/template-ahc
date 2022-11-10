// ---------- Lib --------------
[<AutoOpen>]
module CIn =
    open System
    open System.Globalization

    let private s = Console.OpenStandardInput()
    let private u = Array.zeroCreate 1024
    let mutable private l, private p, private e = 0, 0, false
    let eos () = e

    let private r () =
        if e then IO.EndOfStreamException() |> raise
        if p >= l then
            p <- 0
            l <- s.Read(u, 0, 1024)
            if l <= 0 then
                e <- true
                u.[0] <- 0uy
        let q = p
        p <- p + 1
        u.[q]

    type I() =
        static member op_Explicit(_: I) : char =
            let mutable b = r ()
            while b < 33uy || 126uy < b do
                b <- r ()
            char b
        override x.ToString() =
            let t = Text.StringBuilder()
            let mutable b = char x
            while char 33 <= b && b <= char 126 do
                t.Append b |> ignore
                b <- r () |> char
            t.ToString()
        static member op_Explicit(_: I) : int64 =
            let mutable t, b, n, c = 0L, r (), false, true
            while b <> byte '-' && (b < byte '0' || byte '9' < b) do
                b <- r ()
            if b = byte '-' then
                n <- true
                b <- r ()
            while c do
                if b < byte '0' || byte '9' < b then
                    c <- false
                else
                    t <- t * 10L + int64 b - int64 '0'
                    b <- r ()
            if n then -t else t
        static member op_Explicit(x: I) : int32 = int32 (int64 x)
        static member op_Explicit(x: I) : float =
            Double.Parse(string x, CultureInfo.InvariantCulture)
        static member op_Explicit(x: I) : float32 =
            Single.Parse(string x, CultureInfo.InvariantCulture)
        static member op_Explicit(x: I) : decimal =
            Decimal.Parse(string x, CultureInfo.InvariantCulture)
        static member op_Explicit(x: I) : bigint = bigint.Parse(string x)

    let private c = I()
    let cin _ = c

module COut =
    open System

    type O(s) =
        inherit IO.StreamWriter(s, Text.UTF8Encoding(false, true))
        override _.FormatProvider =
            Globalization.CultureInfo.InvariantCulture :> IFormatProvider

    let noflush () =
        let w = new O(Console.OpenStandardOutput())
        w.AutoFlush <- false
        Console.SetOut w

// ---------- End Lib ----------

COut.noflush ()



stdout.Flush()
