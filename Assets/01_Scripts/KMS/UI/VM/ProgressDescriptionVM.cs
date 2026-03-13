using System;

public class ProgressDescriptionVM
{
    private readonly Building _building;
    public event Action<string> OnTextChanged;

    private SOBuilding data;
    int right = 0;
    int cnt;

    public ProgressDescriptionVM(Building building)
    {
        _building = building;

        data = _building.BuildingData;
        right = 0;
        foreach (var d in data.Steps)
        {
            right += d.RequierAmount;
        }

        cnt = 0;
    }

    public void Refresh(bool increaseCnt)
    {
        if (increaseCnt) cnt++;
        var text = $"{cnt} / {right}";

        OnTextChanged?.Invoke(text);
    }
}