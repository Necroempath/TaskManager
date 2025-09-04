using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TaskManager.Models;

public class ProcessInfo : INotifyPropertyChanged
{
    private double _cpuPercent;
    private double _memoryMb;
    private int _priority;

    public int Id { get; init; }
    public string Name { get; init; }
    public Process WrappedProcess { get; init; }

    public double CpuPercent
    {
        get => _cpuPercent;

        set
        {
            _cpuPercent = value;
            OnPropertyChanged();
        }
    }

    public double MemoryMb
    {
        get => _memoryMb;

        set
        {
            _memoryMb = value;
            OnPropertyChanged();
        }
    }

    public int Priority
    {
        get => _priority;

        set
        {
            _priority = value;
            OnPropertyChanged();
        }
    }

    public ProcessInfo(Process process)
    {
        Id = process.Id;
        Name = process.ProcessName;
        WrappedProcess = process;
        Priority = process.BasePriority;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}