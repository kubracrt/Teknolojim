using Prometheus;
using System.Diagnostics;
using System.Timers;

public class CpuMetricsCollector
{
    private static readonly Gauge CpuUsageGauge = Metrics
        .CreateGauge("system_cpu_usage", "Sistem Cpu kullanımı ölçüldü");

    private readonly PerformanceCounter cpuCounter;
    private readonly System.Timers.Timer timer;

    public CpuMetricsCollector()
    {
        cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        timer = new System.Timers.Timer(5000);
        timer.Elapsed += updateCpuMetric;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    public void updateCpuMetric(object sender, ElapsedEventArgs e)
    {
        float cpuUsage = cpuCounter.NextValue();
        CpuUsageGauge.Set(cpuUsage);
    }
}