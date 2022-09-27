# Metrics

The metrics project is the method by which we gain visibility into the operation of running processes and services. Metrics are collected at regular intervals and ingested by the metrics platform. These are used to power dashboards, alerts and answer business intelligence questions. The metrics system is built upon [prometheus-net](https://github.com/prometheus-net/prometheus-net).

# Usage

Begin by referencing the Metrics nuget package. Create a singleton instance of the `IMetricFactory` interface implemented by the `MetricFactory` class. This singleton factory should be shared by all code in your process by injecting the `IMetricFactory` interface into the constructors of any class that will be recording metrics. The `MetricFactory` takes the name of your application so DI registration may look like:

```csharp
For<IMetricFactory>()
    .Singleton()
    .Use(new MetricFactory("MyService", AllSettings.Environment));
```

In the class into which you injected the factory you may now create and use metrics. Best practice is to create your metrics as a readonly local field in your constructor. For example:

```csharp
public class MyInstrumentedClass
{
    private readonly ICounter _messagesReceived;
    
    public MyInstrumentedClass(IMetricFactory metricFactory)
    {
        _messagesReceived = metricFactory.Counter("messages_received_total", 
            "Count of the total number of messages received by this class");
    }
    
    public void ReceiveMessage(object message)
    {
        _messagesReceived.Increment();
        // ... code to handle message ...
    }
}
```

Finally you need to expose the HTTP endpoint that your metrics will be exposed over. We operate a pull based metrics gathering system using Prometheus. We expose metrics on client machines and a process pulls that data into prometheus for ingestion. This avoids the "thundering herd" issue and reduces the risk of overwhelming the Prometheus server. It does however require more configuration in your process. Configuration varies depending on whether you are hosting your metrics in a standalone console app (i.e. a Service) or via an existing web app (i.e. the API).

## Standalone App

If you are creating a console app you first need to install the Microsoft.Extensions.Hosting nuget package:

`install-package Microsoft.Extensions.Hosting`

Next create a Host - this will contain the context of our metrics server. This shoould happen in your program's `Main` method.

```csharp
var host = Host
    .CreateDefaultBuilder(args)
    .UseConsoleLifetime()
    .Build();
```

Finally create a `MetricServer` listening to the desired port on the desired path.

```csharp
using (host) 
using(var ms = new MetricServer(MetricPort))
using(var server = ms.Start())
{
    /* Your main loop/await host.WaitForShutdownAsync(); goes here
     * something which blocks execution of the Stop method until your
     * program is finished. /

    await server.StopAsync();
    await ms.StopAsync();
    await host.StopAsync(TimeSpan.FromSeconds(30));
}
```

By default the path used will be `/metrics` so if you use port 50,000 you can view your metrics at `http://localhost:50000/metrics`

## Existing Web App

Add the metric server to your endpoint configuration. This varies depending on whether you are using .Net Core or the full .Net Framework.

### .Net Core (3.1+) or .Net 5+

Install the AspNet Prometheus Package:

`install-package prometheus-net.AspNetCore`

Add the metrics endpoint in your app configuration

```csharp
app.UseEndpoints(endpoints =>
{
    // ... your existing endpoints config ...

    endpoints.MapMetrics();
});
```

### .Net Framework - Web API

Install the AspNet Prometheus Package:

`install-package prometheus-net.NetFramework.AspNet`

Configure the metric server to host on a route for your application in Application_Start

```csharp
protected void Application_Start(object sender, EventArgs e)
{
    AspNetMetricServer.RegisterRoutes(GlobalConfiguration.Configuration);
    
    // ...
}
```

## Gathering Metrics

With your app now collecting and publishing metrics you can configure your scraper to get your metrics pulled into prometheus.

# Supported Metric Types

## Counter

A counter keeps an incrementing total of observations for the current process. Counters start at zero and only ever increase. Counters may be incremented by 1 or more for each observation. Counters are reset to zero on process restart. Counters should not be used for metrics that can decrease (i.e. number of active processes), use a gauge instead. A counter has a maximum value of around 1.797E+308. For reference scientists estimate that there are roughly 1E+80 atoms in the universe. 

You can use the following prometheus functions to gain insight into your counters:

* [rate](https://prometheus.io/docs/prometheus/latest/querying/functions/#rate) - The per second average rate of increase. Use for slow moving counters and alerting
* [increase](https://prometheus.io/docs/prometheus/latest/querying/functions/#increase) - The amount that the counter has increased over a time period
* [irate](https://prometheus.io/docs/prometheus/latest/querying/functions/#irate) - The per-second instant increase of a rate between two points in time. Useful for fast moving, volatile counters such as graphing HTTP throughput

## Gauge

Tracks a single numerical value which may increase or decrease. Gauges may track positive or negative numbers. Gauges should be used for tracking metrics that move between values such as the number of items on a queue, available free memory, disk space or temperature.

You can use the following prometheus functions to gain insight into your gauges:

* [delta](https://prometheus.io/docs/prometheus/latest/querying/functions/#delta) - Calculates the difference between the first and last values of a gauge over a time period. An example use would be to track the difference in available memory between now and an hour ago. This method is susceptible to outliers - spurious high and low values.
* [deriv](https://prometheus.io/docs/prometheus/latest/querying/functions/#deriv) - Calculate the per-second derivative of the time series, this helps you determine how quickly the value is changing over time accounting for outliers
* [holt_winters](https://prometheus.io/docs/prometheus/latest/querying/functions/#holt_winters) - Generates a smoothed value for the time series applying weighting based on prior observations.
* [idelta](https://prometheus.io/docs/prometheus/latest/querying/functions/#idelta) - calculates the difference between the last two samples in the range, returning an instant vector with the given deltas
* [predict_linear](https://prometheus.io/docs/prometheus/latest/querying/functions/#predict_linear) - predicts the future value of a metric based on past observations

## Histogram

A histogram samples observations (usually things like request durations or response sizes) and counts them in configurable buckets. It also provides a sum of all observed values. Histograms can be aggregated meaning that we can compute a value from many sources of measurement (i.e. average response time for an API endpoint across all machines in the fleet)

Useful prometheus functions:

* [histogram_quantile](https://prometheus.io/docs/prometheus/latest/querying/functions/#histogram_quantile) - Calculates quantiles (p-values) from a histogram.

## Summary

Similar to a histogram, a summary samples observations (usually things like request durations and response sizes). While it also provides a total count of observations and a sum of all observed values, it calculates configurable quantiles (p-values) over a sliding time window.

Summaries should be used when you don't know the ranges of what you are measuring. If you know the ranges of the data that you care about you should prefer a Histogram. Because summaries calculate their quantile values on the client their data cannot be aggregated meaning it is not suitable for tracking a value over a fleet of machines.

## Timer

A timer is based on a histogram. It makes it easy to measure response times or time taken to execute a method. The underlying histogram is bucketed in the following steps: 

under 10ms, 25ms, 50ms, 100ms, 250ms, 500ms, 1s, 2.5s, 5s, 10s, 15s, 20s, 25s, 30s, 1 min, 2.5 mins, 5 mins, 10 mins and over 10 mins

# What to Measure & Best Practices

You should measure anything of any value that you will use to base decisions on. This can include - 

* Invocations of a method (using counters) i.e. cache hits or misses
* Time taken to execute a method (using timers)
* Number of items in flight on a queue (using gauges)
* Size of a request (using histograms)

If you gather a metric you should show it on a Grafana dashboard. If you don't use the metric on a dashboard, remove it from code. Metrics should always include a meaningful description at the point of creation. Note that renaming a metric will mean that any dashboards that rely upon it will have to be updated. Try not to record performance with timers in tight loops, prefer measuring over the loop in aggregate.

# Implementation

LeapingGorilla metrics uses a facade to hide the underlying implementation details of the system that gathers the metrics however there will always be some leak through in supported metric types and implementation details. 

Currently the library has an implementation for Prometheus with pull-based reporting via an exposed HTTP endpoint. The metrics system is built upon [prometheus-net](https://github.com/prometheus-net/prometheus-net).