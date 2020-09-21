# Wavefront Proxy Reporter Sample

Two ways to push metrics into Wavefront:
* Wavefront proxy (uses Telegraf to push .NET performance info to Wavefront)
* direct ingestion.

The .NET Framework provides tools for building networked applications, distributed web services and web
applications. This integration installs and configures Telegraf to send .NET performance metrics into
Wavefront. Telegraf is a light-weight server process capable of collecting, processing, aggregating,
and sending metrics to a Wavefront proxy.

In addition to setting up the metrics flow, this integration also installs a dashboard.


This console app is configured with two methods to show both the Wavefront proxy and direct ingestion
approach to push metric data into Wavefront.
