using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Core.Models;

public class Product : INotifyPropertyChanged
{
    private int _id;
    private string _name;
    private int _quantity;
    private string _code = "";
    private string _unit = "";
    private decimal _price;
    private bool _isUsed = true;

    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (SetField(ref _quantity, value))
            {
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    public string Code
    {
        get => _code;
        set => SetField(ref _code, value);
    }

    public string Unit
    {
        get => _unit;
        set => SetField(ref _unit, value);
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (SetField(ref _price, value))
            {
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    public bool IsUsed
    {
        get => _isUsed;
        set => SetField(ref _isUsed, value);
    }

    public decimal Total => Quantity * Price;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}