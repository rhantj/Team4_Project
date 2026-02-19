using System;

public class BuildingInputVM
{
    private readonly Building _building;
    public event Action<string> OnTextChanged;

    public BuildingInputVM(Building building)
    {
        _building = building;
    }

    public void Refresh()
    {
        var text = $"{_building.CurrentStepItems} / {_building.CurrentRequire}";
        OnTextChanged?.Invoke(text);
    }
}