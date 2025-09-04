using TaskManager.Models;

namespace TaskManager.Services;

public class ProcessesMonitoringService
{
    private Dictionary<int, (DateTime lastTime, TimeSpan lastTotalCpu)> _cpuCache = new();

    public void UpdateMemory(ProcessInfo process)
    {
        try
        {
            process.MemoryMb = process.WrappedProcess.WorkingSet64 / 1024 / 1024d;
        }
        catch
        {
            process.MemoryMb = 0;
        }
    }

    public void UpdateCpu(ProcessInfo process)
    {
        try
        {
            if (!_cpuCache.TryGetValue(process.Id, out var last))
            {
                last = (DateTime.UtcNow, process.WrappedProcess.TotalProcessorTime);
            }
            
            var now = DateTime.UtcNow;
            var cpuUsedMs = (process.WrappedProcess.TotalProcessorTime - last.lastTotalCpu).TotalMilliseconds;
            var totalMs = (now - last.lastTime).TotalMilliseconds * Environment.ProcessorCount;

            process.CpuPercent = (cpuUsedMs / totalMs) * 100;

            _cpuCache[process.Id] = (now, process.WrappedProcess.TotalProcessorTime);
        }
        catch
        {
            process.CpuPercent = 0;
        }
    }
}