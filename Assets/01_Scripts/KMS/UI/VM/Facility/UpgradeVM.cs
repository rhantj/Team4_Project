using System;

public class UpgradeVM
{
    private readonly ProductionFacility _facility;
    public event Action<string> OnTextChanged;
    public event Action<float> OnFillChanged;
    public event Action<bool> OnUpgradeChanged;

    public UpgradeVM(ProductionFacility facility) => _facility = facility;

    public void Refresh(int cost)
    {
        var fill = (_facility.UpgradeCost <= 0f) ? 0f : _facility.UpgradeProgress / _facility.UpgradeCost;
        var text = $"{cost} / {_facility.UpgradeCost}";

        OnFillChanged?.Invoke(fill);
        OnTextChanged?.Invoke(text);
        OnUpgradeChanged?.Invoke(_facility.IsUpgraded);
    }
}