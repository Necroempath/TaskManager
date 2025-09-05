using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.Utils;
using TaskManager.Views;

namespace TaskManager.ViewModels;

public class TaskManagerViewModel
{
    private readonly ProcessesMonitoringService _processesMonitoringService;

    private readonly StartProcessViewModel _startProcessViewModel;

    private string _searchText;

    private List<ProcessInfo> _allProcesses = new();
    public ObservableCollection<ProcessInfo> Processes { get; init; } = new();
    public ProcessInfo? SelectedProcess { get; set; }

    public ObservableCollection<ProcessInfo> BlacklistProcesses { get; set; } = new();

    public ProcessInfo SelectedBlacklistProcess { get; set; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            Search();
        }
    }

    private DispatcherTimer _timer;
    public ICommand KillProcessCommand { get; }
    public ICommand StartProcessCommand { get; }
    public ICommand ToBlacklistCommand { get; }
    public ICommand RemoveFromBlacklistCommand { get; }

    public TaskManagerViewModel(ProcessesMonitoringService processesMonitoringService,
        StartProcessViewModel startProcessViewModel)
    {
        _processesMonitoringService = processesMonitoringService;

        _startProcessViewModel = startProcessViewModel;

        Refresh();

        LoadProcesses();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };

        _timer.Tick += Timer_Tick!;

        _timer.Start();

        StartProcessCommand = new RelayCommand(StartProcess, () => true);
        ToBlacklistCommand = new RelayCommand(AddToBlacklist, () => SelectedProcess != null);
        RemoveFromBlacklistCommand = new RelayCommand(RemoveFromBlacklist, () => SelectedBlacklistProcess != null);
        KillProcessCommand = new RelayCommand(() =>
        {
            try
            {
                Kill();
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }, () => SelectedProcess != null);
    }

    private void LoadProcesses()
    {
        Processes.Clear();

        foreach (var process in _allProcesses)
        {
            Processes?.Add(process);
        }
    }

    private void LoadProcesses(IEnumerable<ProcessInfo> processes)
    {
        Processes.Clear();

        foreach (var process in processes)
        {
            Processes.Add(process);
        }
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        // Refresh();

        foreach (var process in Processes)
        {
            _processesMonitoringService.UpdateMemory(process);
            _processesMonitoringService.UpdateCpu(process);
        }
    }

    private void Refresh()
    {
        _allProcesses.Clear();

        var processes = Process.GetProcesses();

        foreach (var process in processes)
        {
            _allProcesses.Add(new(process));
        }

        LoadProcesses();
    }

    private void Kill()
    {
        try
        {
            SelectedProcess!.WrappedProcess.Kill();

            _allProcesses.Remove(SelectedProcess);
            Processes.Remove(SelectedProcess);
        }
        catch
        {
            throw new InvalidOperationException($"{SelectedProcess!.Name}: Access denied");
        }
    }

    private void StartProcess()
    {
        _startProcessViewModel.AddedProcess = null;

        var window = new StartProcessWindow { DataContext = _startProcessViewModel };

        window.ShowDialog();

        if (_startProcessViewModel.AddedProcess != null)
        {
            ProcessInfo process = new(_startProcessViewModel.AddedProcess);

            _allProcesses.Add(process);
            Processes.Add(process);
        }
    }

    private void Search()
    {
        if (string.IsNullOrEmpty(SearchText))
        {
            LoadProcesses();
            return;
        }

        var filteredProcesses = _allProcesses.Where(pi => pi.Name.ToLower().Contains(SearchText.ToLower())
                                                          || pi.Id.ToString().Contains(SearchText));

        LoadProcesses(filteredProcesses);
    }

    private void AddToBlacklist()
    {
        try
        {
            Kill();
        }
        catch(Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        
        var temp = SelectedProcess!;
        Processes.Remove(SelectedProcess!);
        BlacklistProcesses.Add(temp);
    }

    private void RemoveFromBlacklist()
    {
        BlacklistProcesses.Remove(SelectedProcess!);
    }
}