using System;
using UnityEngine;

public class BuildingInputVM
{
    private readonly Building _building;
    public event Action<string> OnTextChanged;
    public event Action<Sprite> OnSpriteChanged;

    public BuildingInputVM(Building building)
    {
        _building = building;
    }

    public void Refresh()
    {
        var text = $"{_building.CurrentStepItems} / {_building.CurrentRequire}";
        var sprite = _building.CurrentItemSprite;

        OnTextChanged?.Invoke(text);
        OnSpriteChanged?.Invoke(sprite);
    }
}