using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TaskManager.Utils;

namespace TaskManager.ViewModels;

public class StartProcessViewModel
{
    private string _processName;
    
    public Process? AddedProcess;
    public string ProcessName
    {
        get => _processName;

        set
        {
            _processName = value;
            AcceptCommand.RaiseCanExecuteChanged();
        }
    }

    public RelayCommand AcceptCommand { get; }
    public RelayCommand CancelCommand { get; }

    public StartProcessViewModel()
    {
        AcceptCommand = new RelayCommand(StartProcess, () => !string.IsNullOrEmpty(ProcessName));
    }

    private void StartProcess()
    {
        try
        {
            AddedProcess = null;
            
            AddedProcess = Process.Start(ProcessName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel()
    {
    }
}