using System;

public class OutputVM
{
    private readonly ProductionFacility _facility;
    public event Action<string> OnTextChanged;

    public OutputVM(ProductionFacility fac)=> _facility = fac;

    public void Refresh()
    {
        var text = $"{_facility.OutputCount} / {_facility.OutputLimit}";
        OnTextChanged?.Invoke(text);
    }
}