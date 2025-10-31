namespace Domain.Work.Timing;

/// <summary>
/// Tiempos (horas) para dibujar la barra de un día. Invariantes:
/// TimeInit=TimeInitTR+TimeInitSP; TimeWork=TimeWorkWK+TimeWorkTR+TimeWorkSP;
/// TimeEnd=TimeEndTR+TimeEndSP; TimeTotalReg=TimeInit+TimeWork+TimeEnd.
/// </summary>
public sealed record TimingInfoHours
{
    /// <summary>Horas totales del bloque de INICIO (arriba).</summary>
    public required double TimeInit { get; init; }

    /// <summary>Sub-banda de INICIO en transporte (TR, amarillo).</summary>
    public required double TimeInitTR { get; init; }

    /// <summary>Sub-banda de INICIO parado (SP, rojo). Suele ser TimeInit - TimeInitTR.</summary>
    public required double TimeInitSP { get; init; }

    /// <summary>Horas totales del bloque de TRABAJO (centro). También se muestra en el bracket chico.</summary>
    public required double TimeWork { get; init; }

    /// <summary>Sub-banda de TRABAJO efectivo (WK, verde).</summary>
    public required double TimeWorkWK { get; init; }

    /// <summary>Sub-banda de TRABAJO en transporte (TR, amarillo).</summary>
    public required double TimeWorkTR { get; init; }

    /// <summary>Sub-banda de TRABAJO parado (SP, rojo). Suele ser TimeWork - (WK + TR).</summary>
    public required double TimeWorkSP { get; init; }

    /// <summary>Horas totales del bloque de FIN (abajo).</summary>
    public required double TimeEnd { get; init; }

    /// <summary>Sub-banda de FIN en transporte (TR, amarillo).</summary>
    public required double TimeEndTR { get; init; }

    /// <summary>Sub-banda de FIN parado (SP, rojo). Suele ser TimeEnd - TimeEndTR.</summary>
    public required double TimeEndSP { get; init; }

    /// <summary>
    /// Total del registro del día (bracket grande). Denominador para escalar horas→px.
    /// Debe ser TimeInit + TimeWork + TimeEnd.
    /// </summary>
    public required double TimeTotalReg { get; init; }

    public static TimingInfoHours CreateAuto(
        double timeInit, double timeInitTR,
        double timeWork, double timeWorkWK, double timeWorkTR,
        double timeEnd, double timeEndTR)
    {
        var t = new TimingInfoHours
        {
            TimeInit = timeInit,
            TimeInitTR = timeInitTR,
            TimeInitSP = Math.Max(0, timeInit - timeInitTR),

            TimeWork = timeWork,
            TimeWorkWK = timeWorkWK,
            TimeWorkTR = timeWorkTR,
            TimeWorkSP = Math.Max(0, timeWork - timeWorkWK - timeWorkTR),

            TimeEnd = timeEnd,
            TimeEndTR = timeEndTR,
            TimeEndSP = Math.Max(0, timeEnd - timeEndTR),

            TimeTotalReg = timeInit + timeWork + timeEnd,
        };

        t.EnsureValid();
        return t;
    }

    public void EnsureValid(double tol = 1e-6)
    {
        foreach (var pair in new (string Name, double Value)[]
        {
            (nameof(TimeInit), TimeInit), (nameof(TimeInitTR), TimeInitTR), (nameof(TimeInitSP), TimeInitSP),
            (nameof(TimeWork), TimeWork), (nameof(TimeWorkWK), TimeWorkWK), (nameof(TimeWorkTR), TimeWorkTR), (nameof(TimeWorkSP), TimeWorkSP),
            (nameof(TimeEnd), TimeEnd), (nameof(TimeEndTR), TimeEndTR), (nameof(TimeEndSP), TimeEndSP),
            (nameof(TimeTotalReg), TimeTotalReg),
        })
        {
            if (pair.Value < -tol) throw new TimingValidationException($"{pair.Name} < 0.");
        }

        if (Math.Abs(TimeInit - (TimeInitTR + TimeInitSP)) > tol)
            throw new TimingValidationException("TimeInit != TimeInitTR + TimeInitSP.");

        if (Math.Abs(TimeWork - (TimeWorkWK + TimeWorkTR + TimeWorkSP)) > tol)
            throw new TimingValidationException("TimeWork != TimeWorkWK + TimeWorkTR + TimeWorkSP.");

        if (Math.Abs(TimeEnd - (TimeEndTR + TimeEndSP)) > tol)
            throw new TimingValidationException("TimeEnd != TimeEndTR + TimeEndSP.");

        if (Math.Abs(TimeTotalReg - (TimeInit + TimeWork + TimeEnd)) > tol)
            throw new TimingValidationException("TimeTotalReg != TimeInit + TimeWork + TimeEnd.");
    }
}

public sealed class TimingValidationException : Exception
{
    public TimingValidationException(string message) : base(message) { }
}
