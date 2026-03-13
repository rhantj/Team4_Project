using System;

public class NextProgressVM
{
    private Building _building;
    public event Action<string> OnTextChanged;

    public NextProgressVM(Building building)
    {
        _building = building;
    }

    public void Refresh()
    {
        var text = "";
        OnTextChanged?.Invoke(text);
    }
}