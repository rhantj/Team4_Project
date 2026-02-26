using System;

public class InputVM
{
    private readonly ProductionFacility _facility;
    public event Action<string> OnTextChanged;

    public InputVM(ProductionFacility fac)=> _facility = fac;

    public void Refresh()
    {
        var text = $"{_facility.InputCount} / {_facility.InputLimit}";
        OnTextChanged?.Invoke(text);
    }
}